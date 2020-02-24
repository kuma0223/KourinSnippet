using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kourin;

namespace KourinSnippet
{
    public static class MyKourin
    {
        /// <summary>
        /// Trueのとき結果値を出力文字として採用するものとします。
        /// </summary>
        public static bool Echo { get; set; }

        /// <summary>
        /// 出力テキスト
        /// </summary>
        public static string Output;

        static MyKourin()
        {
            Echo = true;
        }

        public static KourinEngine CreateEngine()
        {
            var kourin = new KourinEngine();

            foreach (var dll in System.IO.Directory.GetFiles(Shared.MyPath + "\\Plugin")) {
                kourin.loadPluginDll(dll);
            }

            //追加関数

            //エコー状態の取得または設定をします。
            //引数[0]: true/false 
            kourin.setFunction(new KourinFunction("ECHO", (args)=>{
                if(args.Length==0) return Echo;
                else{ Echo = (bool)args[0]; return null; }
            }));

            //クリップボードのテキストを取得します。
            kourin.setFunction(new KourinFunction("CLIPBOARD", (args)=>{
                return System.Windows.Clipboard.GetText();
            }));
            
            //出力に1行追加します。
            kourin.setFunction(new KourinFunction("PRINT", (args)=>{
                if(Output == null) return null;
                if(args.Length == 0) return null;

                if(Output.Length>0) Output += Environment.NewLine;
                Output += args[0].ToString();
                return null;
            }));

            //外部プロセスを実行します。
            kourin.setFunction(new KourinFunction("START", (args)=>{
                if(args.Length == 0) return null;
                var info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = args[0].ToString();
                info.Arguments = args.Length > 1 ? args[1].ToString() : null;
                info.UseShellExecute = true;
                var p = System.Diagnostics.Process.Start(info);

                if (args.Length > 2 && (bool)args[2]) {
                    p.WaitForExit();
                }
                return null;
            }));
            
            //コレクションのインデクス位置のデータを返します。
            kourin.setFunction(new KourinFunction("ElementAt", (args)=>{
                return (args[0] as IEnumerable<object>).ElementAt<object>((int)args[1]);
            }));
            
            //コレクションの要素数を返します。
            kourin.setFunction(new KourinFunction("COUNT", (args)=>{
                return (args[0] as IEnumerable<object>).Count();
            }));

            //文字列操作関連

            //TRIMします。
            kourin.setFunction(new KourinFunction("TRIM", (args)=>{
                return args[0].ToString().Trim();
            }));
            
            //Regex.Splitします。
            kourin.setFunction(new KourinFunction("SPLIT", (args)=>{
                var r = new System.Text.RegularExpressions.Regex((string)args[1].ToString());
                return r.Split((string)args[0]);
            }));

            //Regex.Replacecします。
            kourin.setFunction(new KourinFunction("REPLACE", (args)=>{
                var r = new System.Text.RegularExpressions.Regex((string)args[1].ToString());
                return r.Replace((string)args[0], (string)args[2]);
            }));
            
            //Substringします。
            kourin.setFunction(new KourinFunction("SUBSTRING", (args)=>{
                return ((string)args[0]).Substring((int)args[1], (int)args[2]);
            }));

            //左Substringします。
            kourin.setFunction(new KourinFunction("LEFT", (args)=>{
                var s = (string)args[0];
                return s.Substring(0, (int)args[1]);
            }));

            //右Substringします。
            kourin.setFunction(new KourinFunction("RIGHT", (args)=>{
                var s = (string)args[0];
                return s.Substring(s.Length-(int)args[1], (int)args[1]);
            }));
            
            //Indexofします。
            kourin.setFunction(new KourinFunction("INDEXOF", (args)=>{
                return ((string)args[0]).IndexOf((string)args[1]);
            }));

            //文字列の文字数を返します。
            kourin.setFunction(new KourinFunction("LEN", (args)=>{
                return ((string)args[0]).Length;
            }));

            return kourin;
        }
    }
}
