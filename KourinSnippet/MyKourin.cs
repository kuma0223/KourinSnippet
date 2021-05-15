using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Karin;

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

        public static KarinEngine CreateEngine()
        {
            var kourin = new KarinEngine();

            foreach (var dll in System.IO.Directory.GetFiles(Shared.MyPath + "\\Plugin")) {
                kourin.LoadPluginDll(dll);
            }

            //追加関数

            //エコー状態の取得または設定をします。
            //引数[0]: true/false 
            kourin.SetFunction(new KarinFunction("ECHO", (args)=>{
                if(args.Length==0) return Echo;
                else{ Echo = (bool)args[0]; return null; }
            }));

            //クリップボードのテキストを取得します。
            kourin.SetFunction(new KarinFunction("CLIPBOARD", (args)=>{
                return System.Windows.Clipboard.GetText();
            }));
            
            //出力に1行追加します。
            kourin.SetFunction(new KarinFunction("PRINT", (args)=>{
                if(Output == null) return null;
                if(args.Length == 0) return null;

                if(Output.Length>0) Output += Environment.NewLine;
                Output += args[0].ToString();
                return null;
            }));

            //外部プロセスを実行します。
            kourin.SetFunction(new KarinFunction("START", (args)=>{
                if(args.Length == 0) return null;
                var filename = args[0].ToString();
                var argument = args.Length > 1 ? args[1].ToString() : null;
                var waitexit = args.Length > 2 ? (bool)args[2] : false;
                var administ = args.Length > 3 ? (bool)args[3] : false;

                var info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = filename;
                info.Arguments = argument;
                info.UseShellExecute = true;
                if (administ) {
                    info.Verb = "RunAs";
                }
                var p = System.Diagnostics.Process.Start(info);
                if (waitexit) {
                    p.WaitForExit();
                }
                return null;
            }));
            
            //入力ダイアログを表示します。
            kourin.SetFunction(new KarinFunction("InputDialog", (args) => {
                if(args.Length == 0) return null;
                var wind = new InputDialog();
                wind.Label1 = args.Length > 0 ? args[0].ToString() : null;
                wind.Label2 = args.Length > 1 ? args[1].ToString() : null;
                wind.Label3 = args.Length > 2 ? args[2].ToString() : null;

                var ret = wind.ShowDialog();
                kourin.SetVariable("Input1", wind.Text1);
                kourin.SetVariable("Input2", wind.Text2);
                kourin.SetVariable("Input3", wind.Text3);

                return ret.Value;
            }));

            //コレクションのインデクス位置のデータを返します。
            kourin.SetFunction(new KarinFunction("ElementAt", (args)=>{
                return (args[0] as IEnumerable<object>).ElementAt<object>((int)args[1]);
            }));
            
            //コレクションの要素数を返します。
            kourin.SetFunction(new KarinFunction("COUNT", (args)=>{
                return (args[0] as IEnumerable<object>).Count();
            }));

            //文字列操作関連

            //TRIMします。
            kourin.SetFunction(new KarinFunction("TRIM", (args)=>{
                return args[0].ToString().Trim();
            }));
            
            //Regex.Splitします。
            kourin.SetFunction(new KarinFunction("SPLIT", (args)=>{
                var r = new System.Text.RegularExpressions.Regex((string)args[1].ToString());
                return r.Split((string)args[0]);
            }));

            //Regex.Replacecします。
            kourin.SetFunction(new KarinFunction("REPLACE", (args)=>{
                var r = new System.Text.RegularExpressions.Regex((string)args[1].ToString());
                return r.Replace((string)args[0], (string)args[2]);
            }));
            
            //Substringします。
            kourin.SetFunction(new KarinFunction("SUBSTRING", (args)=>{
                return ((string)args[0]).Substring((int)args[1], (int)args[2]);
            }));

            //左Substringします。
            kourin.SetFunction(new KarinFunction("LEFT", (args)=>{
                var s = (string)args[0];
                return s.Substring(0, (int)args[1]);
            }));

            //右Substringします。
            kourin.SetFunction(new KarinFunction("RIGHT", (args)=>{
                var s = (string)args[0];
                return s.Substring(s.Length-(int)args[1], (int)args[1]);
            }));
            
            //Indexofします。
            kourin.SetFunction(new KarinFunction("INDEXOF", (args)=>{
                return ((string)args[0]).IndexOf((string)args[1]);
            }));

            //文字列の文字数を返します。
            kourin.SetFunction(new KarinFunction("LEN", (args)=>{
                return ((string)args[0]).Length;
            }));

            return kourin;
        }
    }
}
