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
    /// ホットキー登録
    /// </summary>
    public class HotkeySetter
    {
        /// <summary>
        /// ホットキー呼び出しイベント
        /// </summary>
        public event Action<int> HotKeyPressed;

        private const int WM_HOTKEY = 0x0312;
        private const int AKEY = 65;

        private IntPtr HWnd = IntPtr.Zero;
        private bool procSet =false;
        
        [DllImport("user32.dll")]
        extern static int RegisterHotKey(IntPtr HWnd, int ID, MOD_KEY ModKey, int KEY);
        [DllImport("user32.dll")]
        extern static int UnregisterHotKey(IntPtr HWnd, int ID);

        /// <summary>
        /// ホットキー登録
        /// </summary>
        /// <param name="HWnd">ウィンドウハンドル</param>
        /// <param name="id">キーID</param>
        /// <param name="modKey">修飾キー</param>
        /// <param name="key">キー（アルファベットのみ）</param>
        /// <returns>true/false</returns>
        public bool SetHotKey(IntPtr HWnd, int id, MOD_KEY modKey, Key key)
        {
            if(this.HWnd != IntPtr.Zero && this.HWnd != HWnd) return false;

            this.HWnd = HWnd;
            var ret = RegisterHotKey(HWnd, id, modKey, AKEY + key - Key.A);
            if(ret == 0) return false;

            //メッセージハンドラ
            if (!procSet) { 
                var source = System.Windows.Interop.HwndSource.FromHwnd(HWnd);
                source.AddHook(new System.Windows.Interop.HwndSourceHook(WndProc));
                procSet = true;
            }
            return true;
        }

        /// <summary>
        /// ホットキー削除
        /// </summary>
        /// <returns></returns>
        public bool RemoveHotKey(int id)
        {
            return UnregisterHotKey(this.HWnd, id)!=0;
        }

        /// <summary>
        /// ウィンドウメッセージキャプチャ
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                if(HotKeyPressed != null) HotKeyPressed(wParam.ToInt32());
            }
            return IntPtr.Zero;
        }

        //◆━━━━━━━━━━━━━━━━━━━━━━━━━━━━◆
        
        /// <summary>
        /// 修飾キー
        /// </summary>
        public enum MOD_KEY :int {
            NON = 0x0000,
            ALT = 0x0001,
            CONTROL = 0x0002,
            SHIFT = 0x0004,
        }
    }
}
