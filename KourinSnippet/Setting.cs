using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KourinSnippet
{
    public class Setting
    {
        public int PosX;
        public int PosY;
        public bool PopupCenter;
        public SnippetSetting Snippet;
        public HistorySetting History;
    }

    public class SnippetSetting
    {
        [System.Xml.Serialization.XmlAttribute]
        public bool enable;
        public Hotkey Hotkey;
    }

    public class HistorySetting
    {
        [System.Xml.Serialization.XmlAttribute]
        public bool enable;
        public Hotkey Hotkey;
        public bool RemoveSameValue;
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
