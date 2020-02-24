using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Kourin;
using Bank;

namespace KourinSnippet
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int HotkeyId1 = 0x1000;
        private const int HotkeyId2 = 0x1001;
        private const int HotkeyId3 = 0x1002;
        private const int ClipHistoryNum = 50;
        
        private KourinEngine kourin;
        private HotkeySetter HKSetter = new HotkeySetter();
        private ClipboardChecker CBChecker = new ClipboardChecker();
        
        /// <summary>
        /// スニペット要素コレクション
        /// </summary>
        private List<SnippetItem> SnippetItems;
        
        /// <summary>
        /// クリップボード履歴リスト
        /// </summary>
        private LinkedList<HistoryItem> ClipbordHistory = new LinkedList<HistoryItem>();

        //◆━━━━━━━━━━━━━━━━━━━━━━━━━━━━◆

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Shared.MyPath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            var handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            //設定読み込み
            Shared.Setting = (Setting)Bank.XMLReader.readXML(Shared.MyPath + "/Setting.xml", typeof(Setting));

            //ロガー
            Shared.Logger = new Bank.LogFileWriter();
            Shared.Logger.folder = Shared.MyPath + "/log";
            Shared.Logger.name = "";
            Shared.Logger.deleteTime = 7;
            Shared.Logger.deleteLog();

            //アイテム読み込み
            ReloadItems();

            //エンジン初期化
            kourin = MyKourin.CreateEngine();
            
            //ホットキー設定
            HKSetter.HotKeyPressed += HotKeyPressed;
            if(Shared.Setting.Snippet.enable) //スニペット用
                HKSetter.SetHotKey(handle, HotkeyId1, Shared.Setting.Snippet.Hotkey.ModKey, Shared.Setting.Snippet.Hotkey.key);
            if(Shared.Setting.History.enable) //履歴用
                HKSetter.SetHotKey(handle, HotkeyId2, Shared.Setting.History.Hotkey.ModKey, Shared.Setting.History.Hotkey.key);

            //クリップボード監視設定
            if (Shared.Setting.History.enable) { 
                CBChecker.ClipbordAppended += ClipbordAppended; 
                CBChecker.SetViewer(handle);
            }
            
            this.Top = Shared.Setting.PosY + (Shared.Setting.PosY<0 ? System.Windows.SystemParameters.PrimaryScreenHeight : 0);
            this.Left = Shared.Setting.PosX + (Shared.Setting.PosX<0 ? System.Windows.SystemParameters.PrimaryScreenWidth : 0);
            this.Height = 25;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CBChecker.RemoveViewer();
            HKSetter.RemoveHotKey(HotkeyId1);
            HKSetter.RemoveHotKey(HotkeyId2);
        }
        
        private void Close_Executed(object sender, RoutedEventArgs e)  
        {
            var ret = MessageBox.Show("KourinSnippetを終了します", "終了", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (ret != MessageBoxResult.OK) return;
            this.Close();
        }

        /// <summary>
        /// アイテム設定を読み込み
        /// </summary>
        private void ReloadItems()
        {
            SnippetItems = new List<SnippetItem>();
            //固定コマンド
            SnippetItems.Add(new SnippetItem(){
                Type = SnippetItem.ItemType.Command, Name = "◆Scripter", Text = "ExecScript"
            });
            //設定ファイル
            var root = Shared.MyPath + "/Items";
            var paths = new List<string>();
            paths.AddRange(Directory.EnumerateDirectories(root));
            paths.AddRange(Directory.EnumerateFiles(root));
            paths.Sort();
            foreach(var child in paths) SnippetItems.Add(CreateNode(child));
        }

        /// <summary>
        /// アイテムファイル読み込み
        /// </summary>
        private SnippetItem CreateNode(string path)
        {
            Func<string, string> ReadText = (filePath)=>{
                using(var reader = new StreamReader(filePath, Encoding.GetEncoding("SJIS")))
                    return reader.ReadToEnd();
            };
            Func<string, string[]> ReadTexts = (filePath)=>{
                var list = new List<string>();
                using(var reader = new StreamReader(filePath, Encoding.GetEncoding("SJIS"))){
                    string str = null;
                    while ((str =reader.ReadLine()) != null) list.Add(str);
                    return list.ToArray();
                }
            };
            //--------------------
            var ret = new SnippetItem();
            //ディレクトリ
            if (Directory.Exists(path))
            {
                ret.Name = Path.GetFileName(path);
                ret.Type = SnippetItem.ItemType.Directory;
                ret.Children = new List<SnippetItem>();
                var paths = new List<string>();
                paths.AddRange(Directory.EnumerateDirectories(path));
                paths.AddRange(Directory.EnumerateFiles(path));
                paths.Sort();
                foreach(var child in paths) ret.Children.Add(CreateNode(child));
            }
            //単一テキスト
            else if(Path.GetExtension(path) == ".txt")
            {
                ret.Type = SnippetItem.ItemType.Text;
                ret.Name = Path.GetFileNameWithoutExtension(path);
                ret.Text = ReadText(path);
            }
            //テキストリスト
            else if(Path.GetExtension(path) == ".list")
            {
                ret.Type = SnippetItem.ItemType.Directory;
                ret.Name = Path.GetFileNameWithoutExtension(path);
                ret.Children = new List<SnippetItem>();
                foreach (var txt in ReadTexts(path))
                {
                    var child = new SnippetItem();
                    child.Type = SnippetItem.ItemType.Text;
                    child.Name = txt;
                    child.Text = txt;
                    ret.Children.Add(child);
                }
            }
            //スクリプト
            else if(Path.GetExtension(path) == ".scr" || Path.GetExtension(path) == ".ks")
            {
                ret.Type = SnippetItem.ItemType.Script;
                ret.Name = Path.GetFileNameWithoutExtension(path);
                ret.Text = ReadText(path);
            }
            return ret;
        }

        /// <summary>
        /// 画面ドラッグ
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try{ this.DragMove(); }
            catch (Exception){ }
        }
        
        /// <summary>
        /// リロードボタン
        /// </summary>
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            ReloadItems();
            MessageBox.Show("アイテムファイルを更新しました。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// フォルダーボタン
        /// </summary>
        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Shared.MyPath+"/Items");
        }
        /// <summary>
        /// 設定ボタン
        /// </summary>
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            var wind = new SettingWindow();
            wind.DataContext = Shared.Setting;
            wind.ShowDialog();

            XMLReader.writeXML(Shared.MyPath + "/Setting.xml", Shared.Setting, typeof(Setting));
        }
        /// <summary>
        /// 履歴クリアボタン
        /// </summary>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClipbordHistory.Clear();
            MessageBox.Show("クリップボード履歴をクリアしました。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// クリップボード監視ハンドラ
        /// </summary>
        /// <param name="obj"></param>
        void ClipbordAppended(string text)
        {
            if(ClipbordHistory.Count > 0 && text == ClipbordHistory.First().Text) return;
            
            if (Shared.Setting.History.RemoveSameValue)
            {
                var del = ClipbordHistory.FirstOrDefault((item)=>{ return item.Text == text; });
                if(del != null) ClipbordHistory.Remove(del);
            }

            this.ClipbordHistory.AddFirst(new HistoryItem(){ Text=text });
            if(ClipbordHistory.Count > ClipHistoryNum) ClipbordHistory.RemoveLast();
        }

        /// <summary>
        /// ホットキーイベントハンドラ
        /// </summary>
        private void HotKeyPressed(int id)
        {
            if (id == HotkeyId1) { 
                var point = GetCuretPosition();
                var frm = new Popup();
                frm.Top = point.y;
                frm.Left = point.x;
                frm.Owner = this;
                frm.DataContext = new {TopItems= SnippetItems};
                frm.Closed += (sender, e)=>{
                    if(frm.SelectedItem != null) Execute(frm.SelectedItem);
                };
                frm.Show();
                frm.Activate();
                frm.Focus();
            }
            else if (id == HotkeyId2)
            {
                var point = GetCuretPosition();
                var frm = new Popup2();
                frm.Top = point.y;
                frm.Left = point.x;
                frm.Owner = this;
                frm.DataContext = new {TopItems= ClipbordHistory};
                frm.Closed += (sender, e)=>{
                    if(frm.SelectedItem != null)
                        Execute(new SnippetItem(){ Type=SnippetItem.ItemType.Text, Text=frm.SelectedItem.Text });
                };
                frm.Show();
                frm.Activate();
                frm.Focus();
            }
        }

        /// <summary>
        /// キャレットの位置取得
        /// </summary>
        private WPOINT GetCuretPosition()
        {
            var point = new WPOINT();

            var cthred = GetCurrentThreadId();
            var awnd = GetForegroundWindow();
            var prc = GetWindowThreadProcessId(awnd, IntPtr.Zero);
            
            AttachThreadInput(cthred, prc, true);
            var isOk = (!Shared.Setting.PopupCenter) ? GetCaretPos(ref point) : false;
            //var focusWHnd = GetFocus(); //フォーカスのあるハンドルを取っておく
            AttachThreadInput(cthred, prc, false);

            var rect = new WRECT();
            GetWindowRect(awnd, ref rect);

            //変わらず。これで取れる奴はGetCaretPosでも取れる。
            //var tinfo = new GUIThreadInfo();
            //tinfo.cbSize = Marshal.SizeOf(tinfo);
            //GetGUIThreadInfo(0, ref tinfo);
            
            if(isOk && point.x!=0 && point.y!=0) //キャレット位置が取れればその位置
                return new WPOINT(){ x=rect.l+point.x, y=rect.t+point.y};
            else //取れなければ適当な位置
                return new WPOINT(){ x=rect.l + (rect.r-rect.l)/2 -100, y=rect.t + (rect.b-rect.t)/2 -175 };
        }
        
        /// <summary>
        /// 処理実行
        /// </summary>
        private void Execute(SnippetItem target)
        {
            string text = null;
            try {
                switch (target.Type)
                {
                    case SnippetItem.ItemType.Command:
                        text = ExecuteCommand(target); break;
                    case SnippetItem.ItemType.Script:
                        text = ExecuteScript(target); break;
                    default:
                        text = target.Text; break;
                }
            }catch(KourinException ex){
                Shared.Logger.write(LogTypes.ERROR, "スクリプトエラー/" + ex.Message + Environment.NewLine + ex.ScriptStackTrace);
                MessageBox.Show("スクリプトエラー/" + ex.Message + Environment.NewLine + ex.ScriptStackTrace);
            }catch(Exception ex) {
                Shared.Logger.write(LogTypes.ERROR, "実行時エラー/" + ex.ToString());
                MessageBox.Show("実行時エラー/" + ex.GetType().Name + "/" + ex.Message);
            }
            if(text == null) return;

            //クリップボードに設定
            try { System.Windows.Clipboard.SetText(text); }
            catch (COMException ex) {
                //COMExceptionが出てもできてる場合が多い
                Shared.Logger.write(LogTypes.ERROR, "Clipboard.SetText異常/" + ex.ToString());
            }
            //ウィンドウのフォーカスが戻るまで一応少し間を空ける
            System.Threading.Thread.Sleep(Shared.Setting.Interval);

            //貼り付け
            Func<int, int, Input> CreateKeyInput = (vkey, flag)=>{
                var input = new Input();
                input.dwType = 1;
                input.ki = new KeyboardInput();
                input.ki.wVk = (short)vkey;
                input.ki.wScan = (short)MapVirtualKey(input.ki.wVk, 0);
                input.ki.dwFlags =flag;
                return input;
            };

            var inputs = new List<Input>();
            inputs.Add(CreateKeyInput(0x11, 0x0 | 0x4)); //Ctrl
            inputs.Add(CreateKeyInput(Shared.Setting.PasteKey/*0x56*/, 0x0 | 0x4)); //v
            inputs.Add(CreateKeyInput(Shared.Setting.PasteKey/*0x56*/, 0x2 | 0x4));
            inputs.Add(CreateKeyInput(0x11, 0x2 | 0x4));

            var ed = inputs.ToArray();
            SendInput(ed.Length, ed, Marshal.SizeOf(ed[0]));

            //駄目。秀丸やメモ帳はできるが
            //ChromeやVisualStudioは受け付けない。
            //SendMessage(focusWHnd, WM_PASTE, 0, 0);
        }

        private string ExecuteScript(SnippetItem target)
        {
            if(target.Text == null) return null;

            MyKourin.Echo = true; //スクリプト開始時はエコーON
            MyKourin.Output = ""; //出力初期化
            //実行
            //結果値がnullでなくエコー設定がONの場合のみ出力に追加される。
            var ret = kourin.execute(target.Text);
            if (ret != null) { 
                if(MyKourin.Output.Length>0) MyKourin.Output += Environment.NewLine;
                MyKourin.Output += ret;
            }
            var output = MyKourin.Output;
            MyKourin.Output = null;
            return output;
        }

        private string ExecuteCommand(SnippetItem target)
        {
            if (target.Text == "ExecScript")
            {   //クリップボードの値を対象にスクリプト実行して結果を返す
                if(!System.Windows.Clipboard.ContainsText()) return null;
                var text = System.Windows.Clipboard.GetText();
                return ExecuteScript(new SnippetItem(){ Type=SnippetItem.ItemType.Script, Text=text });
            }
            return null;
        }
        
        //◆━━━━━━━━━━━━━━━━━━━━━━━━━━━━◆
        
        private const int WM_PASTE = 0x302;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        [DllImport("user32.dll")]
        extern static long SendMessage(IntPtr HWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        extern static bool PostMessage(IntPtr HWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint="PostMessage")]
        extern static bool PostMessageStr(IntPtr HWnd, int msg, int wParam, string lParam);
        [DllImport("user32.dll")]
        extern static IntPtr GetFocus();
        [DllImport("user32.dll")]
        extern static IntPtr SetFocus(IntPtr HWnd);
        [DllImport("user32.dll")]
        extern static IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        extern static IntPtr SetActiveWindow(IntPtr HWnd);
        [DllImport("user32.dll")]
        extern static bool GetWindowRect(IntPtr HWnd, ref WRECT rect);
        [DllImport("user32.dll")]
        extern static int GetCursorPos(ref WPOINT ptr);
        [DllImport("user32.dll")]
        extern static bool GetCaretPos(ref WPOINT ptr);
        [DllImport("user32.dll")]
        extern static IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        extern static bool SetForegroundWindow(IntPtr HWnd);
        [DllImport("user32.dll")]
        extern static IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        extern static IntPtr GetWindowThreadProcessId(IntPtr HWnd, IntPtr lpdwProcessId);
        [DllImport("kernel32.dll")]
        extern static IntPtr GetCurrentThreadId ();
        [DllImport("user32.dll")]
        extern static void SendInput(int nInputs, Input[] pInputs, int cbsize);
        [DllImport("user32.dll")]
        extern static int MapVirtualKey(int wCode, int wMapType);
        [DllImport("user32.dll")]
        static extern uint GetGUIThreadInfo(uint dwthreadid, ref GUIThreadInfo lpguithreadinfo);
        
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct WPOINT { public int x, y;}
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct WRECT { public int l, t, r, b;}
        
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct Input
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public int dwType;
            [System.Runtime.InteropServices.FieldOffset(4)]
            public KeyboardInput ki;
        }
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
            public int dmy1;
            public int dmy2;
        }
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct GUIThreadInfo
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public WRECT rcCaret;
        }
    }
}
