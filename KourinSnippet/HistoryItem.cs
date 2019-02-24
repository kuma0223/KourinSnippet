using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KourinSnippet
{
    public class HistoryItem
    {
        public string Text {get; set;}

        public override string ToString()
        {
            var str = Text.Replace(Environment.NewLine, " "); // System.Text.RegularExpressions.Regex.Escape(Environment.NewLine));
            if(str.Length > 30) str = str.Substring(0, 30);
            return str;
        }
    }
}
