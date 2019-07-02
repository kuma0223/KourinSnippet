using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bank
{
    public enum LogTypes { WORK, ERROR, SEND, RECEIVE }

    public class LogFileWriter
    {

        public string folder = "log";
        public string name = "mylog";
        public int deleteTime = 7;  //ログ保存期間(日)
        
        //ログ書き込み
        public void write(string msg) {
            write(LogTypes.WORK, msg);
        }

        //ログ書き込み
        public void write(LogTypes type, string msg)
        {
            lock (this)
            {
                string filePass = getFilePath();
                string message = DateTime.Now.ToString("HH:mm:ss") + " | " + logTypeStr(type) + " | " + msg;

                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filePass)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePass));

                using (FileStream fs = new FileStream(filePass, FileMode.Append, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fs, Encoding.Unicode))
                {
                    writer.WriteLine(message);
                }
            }
        }

        //ログ削除
        public void deleteLog()
        {
            lock (this)
            {
                string[] files;

                try
                {
                    files = System.IO.Directory.GetFiles(folder, "*.log", System.IO.SearchOption.TopDirectoryOnly);
                    foreach (string filePath in files)
                    {
                        try
                        {
                            string fileName = filePath.Substring(filePath.Length - 12, 12);
                            DateTime fileTime = Convert.ToDateTime(fileName.Substring(0, 4) + "/" + fileName.Substring(4, 2) + "/" + fileName.Substring(6, 2));
                            if (fileTime.AddDays(deleteTime) < DateTime.Now.Date) System.IO.File.Delete(filePath);
                        }
                        catch { }
                    }
                }
                catch
                {
                    //MessageBox.Show("ログ削除エラー", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        //パス取得
        public string getFilePath()
        {
            return folder + "\\" + name + DateTime.Now.ToString("yyyyMMdd") + ".log";
        }

        private string logTypeStr(LogTypes type)
        {
            switch(type)
            {
                case LogTypes.WORK:
                    return "動作";
                case LogTypes.ERROR:
                    return "異常";
                case LogTypes.SEND:
                    return "送信";
                case LogTypes.RECEIVE:
                    return "受信";
                default:
                    return "";
            }
        }
    }
}
