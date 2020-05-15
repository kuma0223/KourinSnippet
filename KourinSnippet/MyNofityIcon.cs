using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KourinSnippet
{
    public partial class MyNofityIcon : Component
    {
        public Action Reload_Click;
        public Action Folder_Click;
        public Action Setting_Click;
        public Action Clear_Click;
        public Action Close_Click;
        public Action Open_Click;

        public MyNofityIcon() {
            InitializeComponent();

            var ass = Assembly.GetExecutingAssembly().GetName();
            var ver = ass.Version;
            notifyIcon1.Text = $"{ass.Name} {ver.Major}.{ver.Minor}";
        }

        public MyNofityIcon(IContainer container) {
            container.Add(this);
            InitializeComponent();
        }

        private void contextMenuStrip1_Click(object sender, EventArgs e) {
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e) {
            Open_Click();
        }

        private void contextMenuStrip1_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e) {
            if(e.ClickedItem == Reload) Reload_Click();
            if(e.ClickedItem == Folder) Folder_Click();
            if(e.ClickedItem == Setting) Setting_Click();
            if(e.ClickedItem == Close) Close_Click();
            if(e.ClickedItem == Open) Open_Click();
        }
    }
}
