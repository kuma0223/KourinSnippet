using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KourinSnippet
{
    public class Setting
    {
        public bool Minimum { get; set; } = true;
        public int PosX {get; set;}
        public int PosY {get; set;}
        public bool PopupCenter {get; set;} = true;
        public char PasteKey { get; set; } = 'V';
        public int Interval {get; set;} = 100;
        public SnippetSetting Snippet {get; set;}
        public HistorySetting History {get; set;}
    }

    public class SnippetSetting
    {
        [System.Xml.Serialization.XmlAttribute]
        public bool enable {get; set;}
        public Hotkey Hotkey {get; set;}
    }

    public class HistorySetting
    {
        [System.Xml.Serialization.XmlAttribute]
        public bool enable {get; set;}
        public Hotkey Hotkey {get; set;}
        public bool RemoveSameValue {get; set;}
        public int Count { get; set; } = 50;
    }

    public class Hotkey
    {
        [System.Xml.Serialization.XmlAttribute]
        public bool ctrl { get; set; }
        [System.Xml.Serialization.XmlAttribute]
        public bool alt { get; set; }
        [System.Xml.Serialization.XmlAttribute]
        public bool shift { get; set; }
        [System.Xml.Serialization.XmlAttribute]
        public System.Windows.Input.Key key { get; set; }
        
        [System.Xml.Serialization.XmlIgnore]
        public string keystr {
            get { return System.Enum.GetName(typeof(System.Windows.Input.Key), key); }
            set {
                try {
                    var c = value[0].ToString().ToUpper();
                    key = (System.Windows.Input.Key)System.Enum.Parse(typeof(System.Windows.Input.Key), c);
                } catch (Exception) {
                }
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public HotkeySetter.MOD_KEY ModKey
        {
            get
            {
                var ret = HotkeySetter.MOD_KEY.NON;
                if(this.ctrl) ret |= HotkeySetter.MOD_KEY.CONTROL;
                if(this.shift) ret |= HotkeySetter.MOD_KEY.SHIFT;
                if(this.alt) ret |= HotkeySetter.MOD_KEY.ALT;
                return ret;
            }
        }
    }
}
