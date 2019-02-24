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

        private IntPtr HWnd = IntPtr.Zero;
        private IntPtr HWndNext = IntPtr.Zero;
        private bool procSet =false;
        
        [DllImport("user32.dll")]
        extern static IntPtr SetClipboardViewer(IntPtr HWnd);
        [DllImport("user32.dll")]
        extern static bool ChangeClipboardChain(IntPtr HWndRemove, IntPtr HWndNewNext);
        [DllImport("user32.dll")]
        extern static long SendMessage(IntPtr HWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// クリップボード監視セット
        /// </summary>
        /// <param name="HWnd">ウィンドウハンドル</param>
        public bool SetViewer(IntPtr HWnd)
        {
            HWndNext = SetClipboardViewer(HWnd);

            //メッセージハンドラ
            if (!procSet) { 
                var source = System.Windows.Interop.HwndSource.FromHwnd(HWnd);
                source.AddHook(new System.Windows.Interop.HwndSourceHook(WndProc));
                procSet = true;
            }
            return true;
        }

        /// <summary>
        /// 監視解除
        /// </summary>
        public bool RemoveViewer()
        {
            return ChangeClipboardChain(this.HWnd, this.HWndNext);
        }

        /// <summary>
        /// ウィンドウメッセージキャプチャ
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DRAWCLIPBOARD)
            {
                try { 
                    if(System.Windows.Clipboard.ContainsText())
                        if(ClipbordAppended != null) ClipbordAppended(System.Windows.Clipboard.GetText());
                }
                catch (Exception) { }
                try { if(HWndNext != IntPtr.Zero) SendMessage(this.HWndNext, msg, wParam, lParam); }
                catch (Exception) { }
            }
            else if (msg == WM_CHANGECBCHAIN)
            {
                try { 
                    if (wParam == HWndNext) HWndNext = lParam;
                    else if(HWndNext != IntPtr.Zero) SendMessage(HWndNext, msg, wParam, lParam);
                }
                catch (Exception) { }
            }
            return IntPtr.Zero;
        }
    }
}
