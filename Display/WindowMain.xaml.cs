using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System.Timers;
using System.Windows;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //インターフェース
    public interface IWindowBase
    {
        string ReceivedData                         //COMポートからの値
        { get; set; }
        void KeyDown(object value);                 //キー押下処理
        void Swipe(object value);                   //スワイプ処理
    }

    //インターフェース
    public interface ITimer
    {
        //タイマー処理
        void OnTimerElapsed(object sender, ElapsedEventArgs e);
    }

    //画面クラス
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            DataContext = ViewModelWindowMain.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelWindowMain : Common, IWindow, ISerialPort
    {
        //変数
        INIFile IniFile = new INIFile(CONST.SETTING_INI);   //設定ファイル
        SQL sql = new SQL();                                //データベース
        ComPort comPort;                                    //COMポート
        WindowState displayState;
        WindowStyle displayStyle;
        double windowLeft = 0;
        double windowTop = 0;
        double windowWidth = 0;
        double windowHeight = 0;
        string processName = string.Empty;
        string processWork = string.Empty;
        string functionColor = string.Empty;
        string receivedData;
        bool visiblePower = false;
        bool visibleList = false;
        bool visibleInfo = false;
        bool visibleDefect = false;
        bool visibleArrow = false;
        bool visiblePlan = false;
        string iconPlan = string.Empty;
        string iconList = string.Empty;
        int iconSize = 30;

        //プロパティ
        public static ViewModelWindowMain Instance          //インスタンス
        { get; set; } = new ViewModelWindowMain();
        public IWindowBase Interface                        //インターフェース
        { get; set; }
        public ITimer Itimer                                //インターフェース
        { get; set; }
        public WindowState DisplayState                     //最大化・Window化
        {
            get => displayState;
            set => SetProperty(ref displayState, value);
        }
        public WindowStyle DisplayStyle                     //最大化・最小化・閉じるボタン
        {
            get => displayStyle;
            set => SetProperty(ref displayStyle, value);
        }
        public double WindowLeft                            //Windowの位置（Left）
        {
            get => windowLeft;
            set => SetProperty(ref windowLeft, value);
        }
        public double WindowTop                             //Windowの位置（Top）
        {
            get => windowTop;
            set => SetProperty(ref windowTop, value);
        }
        public double WindowWidth                           //Windowの大きさ（Width）
        {
            get => windowWidth;
            set => SetProperty(ref windowWidth, value);
        }
        public double WindowHeight                          //Windowの大きさ（Height）
        {
            get => windowHeight;
            set => SetProperty(ref windowHeight, value);
        }
        public string ProcessName                           //工程区分
        {
            get => processName;
            set => SetProperty(ref processName, value);
        }
        public string ProcessWork                           //工程区分表示
        {
            get => processWork;
            set => SetProperty(ref processWork, value);
        }
        public string FunctionColor                         //ページ名色
        {
            get => functionColor;
            set => SetProperty(ref functionColor, value);
        }
        public string ReceivedData                          //COMポートから取得した値
        {
            get => receivedData;
            set
            {
                receivedData = value;
                if (Interface != null) { Interface.ReceivedData = receivedData; }
            }
        }
        public bool VisiblePower                            //表示・非表示（電源ボタン）
        {
            get => visiblePower;
            set => SetProperty(ref visiblePower, value);
        }
        public bool VisibleList                             //表示・非表示（一覧ボタン）
        {
            get => visibleList;
            set => SetProperty(ref visibleList, value);
        }
        public bool VisibleInfo                             //表示・非表示（登録ボタン）
        {
            get => visibleInfo;
            set => SetProperty(ref visibleInfo, value);
        }
        public bool VisibleDefect                           //表示・非表示（不良ボタン）
        {
            get => visibleDefect;
            set => SetProperty(ref visibleDefect, value);
        }
        public bool VisibleArrow                            //表示・非表示（矢印ボタン）
        {
            get => visibleArrow;
            set => SetProperty(ref visibleArrow, value);
        }
        public bool VisiblePlan                             //表示・非表示（予定ボタン）
        {
            get => visiblePlan;
            set => SetProperty(ref visiblePlan, value);
        }
        public string IconPlan                              //アイコン（計画一覧）
        {
            get => iconPlan;
            set => SetProperty(ref iconPlan, value);
        }
        public string IconList                              //アイコン（計画一覧）
        {
            get => iconList;
            set => SetProperty(ref iconList, value);
        }
        public int IconSize                                 //アイコンサイズ
        {
            get => iconSize;
            set => SetProperty(ref iconSize, value);
        }

        //イベント
        ActionCommand commandClosing;
        public ICommand CommandClosing => commandClosing ??= new ActionCommand(OnClosing);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand commandKey;
        public ICommand CommandKey => commandKey ??= new ActionCommand(KeyDown);

        //コンストラクター
        internal ViewModelWindowMain()
        {
            Instance = this;
            WindowBehavior.Instance.iWindow = this;

            //初期設定
            LoadWindowProperty();       //Windowのサイズ・位置を復元
            ReadIniFile();              //設定ファイル読み込み
            StartTimer();               //時刻設定スタート

            //表示項目
            FunctionColor = IsServer ? "1" : "0.5";
            StartPage(IniFile.GetString("Page", "Initial"));
            VisiblePower = false;
            VisibleList = false;
            VisibleInfo = false;
            VisibleDefect = false;
            VisibleArrow = false;
        }

        //終了処理
        private void OnClosing()
        {
            SaveWindowProperty();   //Windowのサイズ・位置を記憶
        }

        //シャットダウン
        private async void PowerOff()
        {
            try
            {
                var result = (bool)await DialogHost.Show(new ControlMessage("システム終了", "※登録を破棄してシステムを終了します。", "警告"));
                if (result) 
                {
                    sql.DatabaseClose();
                    comPort.PortClose();
                    Application.Current.Shutdown(); 
                }
            }
            catch
            {
                //DialogHost is already open
            }
        }

        //Windowサイズ・位置復元
        private void LoadWindowProperty()
        {
            //最大化設定
            DisplayState = IniFile.GetBool("System", "WindowwStateMax") ? WindowState.Maximized : WindowState.Normal;
            DisplayStyle = WindowStyle.None;

            if (DisplayStyle == WindowStyle.None)
            {
                WindowLeft = 0;
                WindowTop = 0;
                if (WindowWidth <= 0) { WindowWidth = 1280; }
                if (WindowHeight <= 0) { WindowHeight = 740; }
            }
            else
            {
                //Windowのサイズ・位置を復元 
                WindowLeft = Properties.Settings.Default.WindowLeft;
                WindowTop = Properties.Settings.Default.WindowTop;
                WindowWidth = Properties.Settings.Default.WindowWidth;
                WindowHeight = Properties.Settings.Default.WindowHeight;
            }

            //マルチモニタ対応
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (!screen.Primary)
                {
                    WindowLeft = screen.Bounds.Left;
                    WindowTop = screen.Bounds.Top;
                }
            }
        }

        //Windowのサイズ・位置を記憶
        private void SaveWindowProperty()
        {
            if (DisplayState == WindowState.Maximized) { return; }
            Properties.Settings.Default.WindowLeft = WindowLeft;
            Properties.Settings.Default.WindowTop = WindowTop;
            Properties.Settings.Default.WindowWidth = WindowWidth;
            Properties.Settings.Default.WindowHeight = WindowHeight;
            Properties.Settings.Default.Save();
        }

        //iniファイル読み込み
        private void ReadIniFile()
        {
            //データベース接続
            var db = IniFile.GetString("Database", "Database");
            var connect = IniFile.GetString("Database", "ConnectString");
            sql.DatabaseOpen(db, connect);

            //シリアルポート接続
            comPort = new ComPort();
            comPort.PortOpen(IniFile.GetString("Common", "SerialPort", ""));
            comPort.Interface = this;
        }

        //アイコン初期化
        public void InitializeIcon()
        {
            IconList = "ViewList";
            IconPlan = "FileClockOutline";
            IconSize = 30;
        }

        //1秒間隔でタイマーを設定
        private void StartTimer()
        {
            var timer = new Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        //タイマー処理
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Itimer != null) { Itimer.OnTimerElapsed(sender,e); }
        }

        //スワイプ処理
        public void ManipulationDelta(object? sender, ManipulationDeltaEventArgs e)
        {
            var delta = e.DeltaManipulation;
            var offsetX = delta.Translation.X;
            var offsetY = delta.Translation.Y;

            //スワイプ処理
            if (offsetY > 80) { KeyDown("ESC"); }               //上から下（電源を切る）
            if (offsetY < -80) { KeyDown("F12"); }              //下から上（設定画面表示）
            if (offsetX > 80) { Interface.Swipe("Left"); }       //左から右（一覧画面表示）
            if (offsetX < -80) { Interface.Swipe("Right"); }     //右から左（入力画面表示）
        }

        //キー処理
        private void KeyDown(object value)
        {
            switch (value)
            {
                case "ESC":
                    //終了処理
                    PowerOff();
                    break;

                case "F1":
                    //実績登録画面
                    ManufactureInfo.ManufactureCODE = string.Empty;
                    ManufactureInfo.LotNumber = string.Empty;
                    FramePage = new ManufactureInfo();
                    IniFile.WriteString("Page", "Initial", "ManufactureInfo");
                    break;

                case "F2":
                    //搬入登録画面
                    if (ProcessName == "検査" || ProcessName == "梱包") { return; }
                    InProcessInfo.InProcessCODE = string.Empty;
                    InProcessInfo.LotNumber = string.Empty;
                    FramePage = new InProcessInfo();
                    IniFile.WriteString("Page", "Initial", "InProcessInfo");
                    break;

                case "F3":
                    //搬出登録画面
                    if (ProcessName == "梱包") { return; }
                    FramePage = new Transport();
                    IniFile.WriteString("Page", "Initial", "Transport");
                    break;

                case "F4":
                    //計画一覧画面
                    FramePage = new PlanList();
                    IniFile.WriteString("Page", "Initial", "PlanList");
                    break;

                case "F5":
                    //梱包仕様書
                    FramePage = new PackSpecification();
                    IniFile.WriteString("Page", "Initial", "PackSpecification");
                    break;

                case "F6":
                    //作業マニュアル画面
                    FramePage = new Manual();
                    break;

                case "F12":
                    //設定画面
                    FramePage = new Setting();
                    break;

                case "DisplayInfo": case "DisplayList": case "DefectInfo": case "DisplayPlan":
                case "PreviousDate": case "NextDate": case "Today":
                    //画面遷移
                    if (Interface != null) { Interface.KeyDown(value); }
                    break;

                case "Grid":
                    if (Interface != null) { Interface.KeyDown(value); }
                    break;
            }
        }
    }
}
