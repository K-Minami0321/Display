using ClassBase;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

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
        //プロパティ変数
        WindowState _DisplayState;
        WindowStyle _DisplayStyle;
        double _WindowLeft;
        double _WindowTop;
        double _WindowWidth;
        double _WindowHeight;
        string _ProcessName;
        string _ProcessWork;
        string _FunctionColor;
        bool _VisiblePower;
        bool _VisibleList;
        bool _VisibleInfo;
        bool _VisibleDefect;
        bool _VisibleArrow;
        bool _VisiblePlan;
        string _IconPlan;
        string _IconList;
        int _IconSize;

        //プロパティ
        public static ViewModelWindowMain Instance      //インスタンス
        { get; set; } = new ViewModelWindowMain();
        public IKeyDown Ikeydown                        //インターフェース
        { get; set; }
        public WindowState DisplayState                 //最大化・Window化
        {
            get { return _DisplayState; }
            set { SetProperty(ref _DisplayState, value); }
        }
        public WindowStyle DisplayStyle                 //最大化・最小化・閉じるボタン
        {
            get { return _DisplayStyle; }
            set { SetProperty(ref _DisplayStyle, value); }
        }
        public double WindowLeft                        //Windowの位置（Left）
        {
            get { return _WindowLeft; }
            set { SetProperty(ref _WindowLeft, value); }
        }
        public double WindowTop                         //Windowの位置（Top）
        {
            get { return _WindowTop; }
            set { SetProperty(ref _WindowTop, value); }
        }
        public double WindowWidth                       //Windowの大きさ（Width）
        {
            get { return _WindowWidth; }
            set { SetProperty(ref _WindowWidth, value); }
        }
        public double WindowHeight                      //Windowの大きさ（Height）
        {
            get { return _WindowHeight; }
            set { SetProperty(ref _WindowHeight, value); }
        }
        public string ProcessName                       //工程区分
        {
            get { return _ProcessName; }
            set { SetProperty(ref _ProcessName, value); }
        }
        public string ProcessWork                       //工程区分表示
        {
            get { return _ProcessWork; }
            set { SetProperty(ref _ProcessWork, value); }
        }
        public string FunctionColor                     //ページ名色
        {
            get { return _FunctionColor; }
            set { SetProperty(ref _FunctionColor, value); }
        }
        public bool VisiblePower                        //表示・非表示（電源ボタン）
        {
            get { return _VisiblePower; }
            set { SetProperty(ref _VisiblePower, value); }
        }
        public bool VisibleList                         //表示・非表示（一覧ボタン）
        {
            get { return _VisibleList; }
            set { SetProperty(ref _VisibleList, value); }
        }
        public bool VisibleInfo                         //表示・非表示（登録ボタン）
        {
            get { return _VisibleInfo; }
            set { SetProperty(ref _VisibleInfo, value); }
        }
        public bool VisibleDefect                       //表示・非表示（不良ボタン）
        {
            get { return _VisibleDefect; }
            set { SetProperty(ref _VisibleDefect, value); }
        }
        public bool VisibleArrow                        //表示・非表示（矢印ボタン）
        {
            get { return _VisibleArrow; }
            set { SetProperty(ref _VisibleArrow, value); }
        }
        public bool VisiblePlan                         //表示・非表示（予定ボタン）
        {
            get { return _VisiblePlan; }
            set { SetProperty(ref _VisiblePlan, value); }
        }
        public string IconPlan                          //アイコン（計画一覧）
        {
            get { return _IconPlan; }
            set { SetProperty(ref _IconPlan, value); }
        }
        public string IconList                          //アイコン（計画一覧）
        {
            get { return _IconList; }
            set { SetProperty(ref _IconList, value); }
        }
        public int IconSize                             //アイコンサイズ
        {
            get { return _IconSize; }
            set { SetProperty(ref _IconSize, value); }
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
            StartPage();
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
                    //計画一覧画面
                    FramePage = new PlanList();
                    INI.WriteString("Page", "Initial", "PlanList");
                    break;

                case "F2":
                    //搬入登録画面
                    ViewModelInProcessInfo.Instance.InProcessCODE = null;
                    FramePage = new InProcessInfo();
                    INI.WriteString("Page", "Initial", "InProcessInfo");
                    break;

                case "F3":
                    //搬出登録画面
                    ViewModelTransportInfo.Instance.InProcessCODE = null;
                    FramePage = new TransportList();
                    INI.WriteString("Page", "Initial", "TransportList");
                    break;

                case "F4":
                    //実績登録画面
                    ViewModelManufactureList.Instance.ManufactureCODE = null;
                    FramePage = new ManufactureInfo();
                    INI.WriteString("Page", "Initial", "ManufactureInfo");
                    break;

                case "F5":
                    //作業マニュアル画面

                    break;

                case "F11":
                    //不良登録画面（Debug）
                    FramePage = new DefectInfo();
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
