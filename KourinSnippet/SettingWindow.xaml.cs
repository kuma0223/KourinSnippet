﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KourinSnippet
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow() {
            InitializeComponent();

            var ass = Assembly.GetExecutingAssembly().GetName();
            var ver = ass.Version;
            X_Version.Content = $"{ass.Name} {ver.Major}.{ver.Minor}";
        }
    }
}
