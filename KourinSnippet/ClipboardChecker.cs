using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace KourinSnippet
{
    /// <summary>
    /// クリップボード監視
    /// </summary>
    public class ClipboardChecker
    {
        /// <summary>
        /// クリップボード追加発生イベント
        /// </summary>
        public event Action<string> ClipbordAppended;
        
        private const int WM_DRAWCLIPBOARD = 0x0308;
        private const int WM_CHANGECBCHAIN = 0x030D;

        private const int WM_DRAWCLIPBOARD2 = 0x031D;

        private IntPtr HWnd = IntPtr.Zero;
        private IntPtr HWndNext = IntPtr.Zero;
        private bool procSet =false;
        
        [DllImport("user32.dll")]
        extern static IntPtr SetClipboardViewer(IntPtr HWnd);
        [DllImport("user32.dll")]
        extern static bool ChangeClipboardChain(IntPtr HWndRemove, IntPtr HWndNewNext);
        [DllImport("user32.dll")]
        extern static long SendMessage(IntPtr HWnd, int msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll")]
        extern static bool AddClipboardFormatListener(IntPtr HWnd);
        [DllImport("user32.dll")]
        extern static bool RemoveClipboardFormatListener(IntPtr HWnd);

        /// <summary>
        /// クリップボード監視セット
        /// </summary>
        /// <param name="HWnd">ウィンドウハンドル</param>
        public bool SetViewer(IntPtr HWnd)
        {
            //メッセージハンドラ
            if (!procSet) { 
                var source = System.Windows.Interop.HwndSource.FromHwnd(HWnd);
                source.AddHook(new System.Windows.Interop.HwndSourceHook(WndProc));
                procSet = true;
            }

            //HWndNext = SetClipboardViewer(HWnd);
            return AddClipboardFormatListener(HWnd);
        }

        /// <summary>
        /// 監視解除
        /// </summary>
        public bool RemoveViewer()
        {
            //return ChangeClipboardChain(this.HWnd, this.HWndNext);
            return RemoveClipboardFormatListener(HWnd);
        }

        /// ウィンドウメッセージキャプチャ
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DRAWCLIPBOARD2) {
                try { 
                    if(System.Windows.Clipboard.ContainsText())
                        if(ClipbordAppended != null) ClipbordAppended(System.Windows.Clipboard.GetText());
                }
                catch (Exception ex) {
                    Shared.Logger.write(Bank.LogTypes.ERROR, "WM_DRAWCLIPBOARD2/" + ex.ToString());
                }

                var log = string.Format("{0},{1},{2},{3}", hwnd, msg, wParam, lParam);
                Shared.Logger.write("WM_DRAWCLIPBOARD2(" + log + ")");
            }
            //クリップボードチェイン利用
            //if (msg == WM_DRAWCLIPBOARD)
            //{
            //    //処理より先に次へ通知する
            //    if(HWndNext != IntPtr.Zero){
            //        MySendMessage(this.HWndNext, msg, wParam, lParam);
            //    }

            //    try { 
            //        if(System.Windows.Clipboard.ContainsText())
            //            if(ClipbordAppended != null) ClipbordAppended(System.Windows.Clipboard.GetText());
            //    }
            //    catch (Exception ex) {
            //        Shared.Logger.write(Bank.LogTypes.ERROR, "WM_DRAWCLIPBOARD/" + ex.ToString());
            //    }

            //    var log = string.Format("{0},{1},{2},{3}", hwnd, msg, wParam, lParam);
            //    Shared.Logger.write("WM_DRAWCLIPBOARD(" + log + ")");
            //}
            //else if (msg == WM_CHANGECBCHAIN)
            //{
            //    if (wParam == HWndNext) HWndNext = lParam;
            //    else if(HWndNext != IntPtr.Zero) MySendMessage(HWndNext, msg, wParam, lParam);
                
            //    var log = string.Format("{0},{1},{2},{3}", hwnd, msg, wParam, lParam);
            //    Shared.Logger.write("WM_CHANGECBCHAIN(" + log + ")");
            //}
            return IntPtr.Zero;
        }

        private void MySendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam) {
            try {
                var ret = SendMessage(hwnd, msg, wParam, lParam);
            } catch (Exception ex) {
                var log = string.Format("{0},{1},{2},{3}", hwnd, msg, wParam, lParam);
                Shared.Logger.write(Bank.LogTypes.ERROR, "SendMessage Error(" + log + ")/" + ex.ToString());
            }
        }
    }
}
