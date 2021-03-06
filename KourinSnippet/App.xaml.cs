﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KourinSnippet
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private System.Threading.Mutex mutex = null;
        public static MyNotifyIcon NofityIcon { private set; get; }

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.DispatcherUnhandledException += this.Application_DispatcherUnhandledException;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            NofityIcon = new MyNotifyIcon();
            
            // 多重起動チェック
            var mutexName = this.GetType().Assembly.GetName().Name;
            mutex = new System.Threading.Mutex(false, mutexName);

            if (mutex.WaitOne(0, false) == false)
            {
                MessageBox.Show("既に起動しています。", this.GetType().Assembly.GetName().Name);
                mutex.Close();
                mutex = null;
                this.Shutdown();
                return;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Shared.Logger != null) Shared.Logger.write("ApplicationExit");
            if (mutex != null) mutex.Close();
            if (NofityIcon != null) NofityIcon.Dispose();
        }

        /// <summary>
        /// 例外処理
        /// </summary>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs args)
        {
            try {
                Shared.Logger.write(Bank.LogTypes.ERROR, "未処理例外/" + args.Exception.ToString());
            }
            catch (Exception) { }
		    args.Handled = true;
        }
    }
}
