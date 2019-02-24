using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KourinSnippet
{
    public class SnippetItem
    {
        public ItemType Type {get; set;}
        public string Name {get; set;}
        public string Text {get; set;}
        public List<SnippetItem> Children{get; set;}

        public override string ToString()
        {
            return Type==ItemType.Directory ? this.Name+"  >" : this.Name;
        }

        public enum ItemType { Directory, Text, Script, Command }
    }
}
