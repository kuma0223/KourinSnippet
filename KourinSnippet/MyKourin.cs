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

            return kourin;
        }
    }
}
