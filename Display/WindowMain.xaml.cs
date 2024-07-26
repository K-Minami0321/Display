using ClassBase;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //インターフェース
    public interface IKeyDown
    {
        void KeyDown(object value);    //キー押下処理
        void Swipe(object value);
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
    public class ViewModelWindowMain : Common, IWindow
    {
        //変数
        WindowState displayState;
        WindowStyle displayStyle;
        double windowLeft;
        double windowTop;
        double windowWidth;
        double windowHeight;
        string processName;
        string processWork;
        string functionColor;
        bool visiblePower;
        bool visibleList;
        bool visibleInfo;
        bool visibleDefect;
        bool visibleArrow;
        bool visiblePlan;
        string iconPlan;
        string iconList;
        int iconSize;

        //プロパティ
        public static ViewModelWindowMain Instance      //インスタンス
        { get; set; } = new ViewModelWindowMain();
        public IKeyDown Ikeydown                        //インターフェース
        { get; set; }
        public WindowState DisplayState                 //最大化・Window化
        {
            get { return displayState; }
            set { SetProperty(ref displayState, value); }
        }
        public WindowStyle DisplayStyle                 //最大化・最小化・閉じるボタン
        {
            get { return displayStyle; }
            set { SetProperty(ref displayStyle, value); }
        }
        public double WindowLeft                        //Windowの位置（Left）
        {
            get { return windowLeft; }
            set { SetProperty(ref windowLeft, value); }
        }
        public double WindowTop                         //Windowの位置（Top）
        {
            get { return windowTop; }
            set { SetProperty(ref windowTop, value); }
        }
        public double WindowWidth                       //Windowの大きさ（Width）
        {
            get { return windowWidth; }
            set { SetProperty(ref windowWidth, value); }
        }
        public double WindowHeight                      //Windowの大きさ（Height）
        {
            get { return windowHeight; }
            set { SetProperty(ref windowHeight, value); }
        }
        public string ProcessName                       //工程区分
        {
            get { return processName; }
            set { SetProperty(ref processName, value); }
        }
        public string ProcessWork                       //工程区分表示
        {
            get { return processWork; }
            set { SetProperty(ref processWork, value); }
        }
        public string FunctionColor                     //ページ名色
        {
            get { return functionColor; }
            set { SetProperty(ref functionColor, value); }
        }
        public bool VisiblePower                        //表示・非表示（電源ボタン）
        {
            get { return visiblePower; }
            set { SetProperty(ref visiblePower, value); }
        }
        public bool VisibleList                         //表示・非表示（一覧ボタン）
        {
            get { return visibleList; }
            set { SetProperty(ref visibleList, value); }
        }
        public bool VisibleInfo                         //表示・非表示（登録ボタン）
        {
            get { return visibleInfo; }
            set { SetProperty(ref visibleInfo, value); }
        }
        public bool VisibleDefect                       //表示・非表示（不良ボタン）
        {
            get { return visibleDefect; }
            set { SetProperty(ref visibleDefect, value); }
        }
        public bool VisibleArrow                        //表示・非表示（矢印ボタン）
        {
            get { return visibleArrow; }
            set { SetProperty(ref visibleArrow, value); }
        }
        public bool VisiblePlan                         //表示・非表示（予定ボタン）
        {
            get { return visiblePlan; }
            set { SetProperty(ref visiblePlan, value); }
        }
        public string IconPlan                          //アイコン（計画一覧）
        {
            get { return iconPlan; }
            set { SetProperty(ref iconPlan, value); }
        }
        public string IconList                          //アイコン（計画一覧）
        {
            get { return iconList; }
            set { SetProperty(ref iconList, value); }
        }
        public int IconSize                             //アイコンサイズ
        {
            get { return iconSize; }
            set { SetProperty(ref iconSize, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
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

            //Windowのサイズ・位置を復元
            LoadWindowProperty();
            DisplayState = INI.GetBool("System", "WindowwStateMax") ? WindowState.Maximized : WindowState.Normal;
            DisplayStyle = WindowStyle.None;

            //データベース接続文字列
            SQL.DB = INI.GetString("Database", "Database");
            SQL.ConnectString = INI.GetString("Database", "ConnectString");
            SQL.DatabaseOpen();
            FunctionColor = (SQL.ConnectString.Contains("DEV")) ? "0.5" : "1";
        }

        //開始処理
        private void OnLoad()
        {
            WindowBehavior.Instance.iWindow = this;
            StartPage(INI.GetString("Page", "Initial"));
            ProcessName = INI.GetString("Page", "Process");
            VisiblePower = false;
            VisibleList = false;
            VisibleInfo = false;
            VisibleDefect = false;
            VisibleArrow = false;
        }

        //終了処理
        private void OnClosing()
        {
            //Windowのサイズ・位置を記憶
            SaveWindowProperty();
        }

        //シャットダウン
        private async void PowerOff()
        {
            try
            {
                var result = (bool)await DialogHost.Show(new ControlMessage("システム終了", "※登録を破棄してシステムを終了します。", "警告"));
                if (result) { Application.Current.Shutdown(); }
            }
            catch
            {
                //DialogHost is already open
            }
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
                    ViewModelManufactureList.Instance.ManufactureCODE = null;
                    FramePage = new ManufactureInfo();
                    INI.WriteString("Page", "Initial", "ManufactureInfo");
                    break;

                case "F2":
                    //搬入登録画面
                    ViewModelInProcessInfo.Instance.InProcessCODE = null;
                    FramePage = new InProcessInfo();
                    INI.WriteString("Page", "Initial", "InProcessInfo");
                    break;

                case "F3":
                    //搬出登録画面
                    if (ProcessName != "プレス") { return; }
                    ViewModelTransportInfo.Instance.InProcessCODE = null;
                    FramePage = new TransportList();
                    INI.WriteString("Page", "Initial", "TransportList");
                    break;

                case "F4":
                    //計画一覧画面
                    FramePage = new PlanList();
                    INI.WriteString("Page", "Initial", "PlanList");
                    break;

                case "F5":
                    //作業マニュアル画面

                    break;

                case "F12":
                    //設定画面
                    ViewModelManufactureInfo.Instance.Status = null;
                    FramePage = new Setting();
                    break;

                case "DisplayInfo":
                case "DisplayList":
                case "DefectInfo":
                case "DisplayPlan":
                case "PreviousDate":
                case "NextDate":
                case "Today":
                    //画面遷移
                    if (Ikeydown != null) { Ikeydown.KeyDown(value); }
                    break;

                case "Grid":
                    if (Ikeydown != null) { Ikeydown.KeyDown(value); }
                    break;
            }
        }

        //Windowサイズ・位置復元
        private void LoadWindowProperty()
        {
            if (DisplayStyle == WindowStyle.None)
            {
                WindowLeft = 0;
                WindowTop = 0;
                WindowWidth = 0;
                WindowHeight = 0;
            }
            else
            {
                //Windowのサイズ・位置を復元 
                WindowLeft = Properties.Settings.Default.WindowLeft;
                WindowTop = Properties.Settings.Default.WindowTop;
                WindowWidth = Properties.Settings.Default.WindowWidth;
                WindowHeight = Properties.Settings.Default.WindowHeight;
            }

            WindowLeft = 0;
            WindowTop = 0;
            WindowWidth = 0;
            WindowHeight = 0;

            //Width・Heightのデフォルト値
            if (WindowWidth <= 0) { WindowWidth = 1280; }
            if (WindowHeight <= 0) { WindowHeight = 880; }
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


        //スワイプ処理
        public void ManipulationDelta(object? sender, ManipulationDeltaEventArgs e)
        {
            var delta = e.DeltaManipulation;
            var offsetX = delta.Translation.X;
            var offsetY = delta.Translation.Y;

            //スワイプ処理
            if (offsetY > 80) { KeyDown("ESC"); }               //上から下（電源を切る）
            if (offsetY < -80) { KeyDown("F12"); }              //下から上（設定画面表示）
            if (offsetX > 80) { Ikeydown.Swipe("Left"); }       //左から右（一覧画面表示）
            if (offsetX < -80) { Ikeydown.Swipe("Right"); }     //右から左（入力画面表示）
        }

        //アイコン初期化
        public void InitializeIcon()
        {
            IconList = "ViewList";
            IconPlan = "FileClockOutline";
            IconSize = 30;
        }
    }
}
