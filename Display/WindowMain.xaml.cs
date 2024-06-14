using ClassBase;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Reactive.Bindings;
using System.Reactive.Disposables;
using System;

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
        public static WindowMain Instance
        { get; set; }
        public WindowMain()
        {
            Instance = this;
            DataContext = ViewModelWindowMain.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelWindowMain : Common, IWindow, IDisposable
    {
        //変数
        CompositeDisposable Disposable                              //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelWindowMain Instance                  //インスタンス
        { get; set; } = new ViewModelWindowMain();
        public IKeyDown Ikeydown                                    //インターフェース
        { get; set; }




        public ReactivePropertySlim<Frame> FramePage                //FramePage
        { get; set; } = new ReactivePropertySlim<Frame>();
        public ReactivePropertySlim<WindowState> DisplayState       //最大化・Window化
        { get; set; } = new ReactivePropertySlim<WindowState>();
        public ReactivePropertySlim<WindowStyle> DisplayStyle       //最大化・最小化・閉じるボタン
        { get; set; } = new ReactivePropertySlim<WindowStyle>();
        public ReactivePropertySlim<double> WindowLeft              //Windowの位置（Left）
        { get; set; } = new ReactivePropertySlim<double>();
        public ReactivePropertySlim<double> WindowTop               //Windowの位置（Top）
        { get; set; } = new ReactivePropertySlim<double>();
        public ReactivePropertySlim<double> WindowWidth             //Windowの大きさ（Width）
        { get; set; } = new ReactivePropertySlim<double>();
        public ReactivePropertySlim<double> WindowHeight            //Windowの大きさ（Height）
        { get; set; } = new ReactivePropertySlim<double>();
        public ReactivePropertySlim<string> ProcessName             //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ProcessWork             //工程区分表示
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> FunctionColor           //ページ名色
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> VisiblePower              //表示・非表示（電源ボタン）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleList               //表示・非表示（一覧ボタン）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleInfo               //表示・非表示（登録ボタン）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleDefect             //表示・非表示（不良ボタン）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleArrow              //表示・非表示（矢印ボタン）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisiblePlan               //表示・非表示（予定ボタン）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<string> IconPlan                //アイコン（計画一覧）
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> IconList                //アイコン（計画一覧）
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<int> IconSize                   //アイコンサイズ
        { get; set; } = new ReactivePropertySlim<int>();

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
            DisplayState.Value = INI.GetBool("System", "WindowwStateMax") ? WindowState.Maximized : WindowState.Normal;
            DisplayStyle.Value = WindowStyle.None;

            //データベース接続文字列
            SQL.DB = INI.GetString("Database", "Database");
            SQL.ConnectString = INI.GetString("Database", "ConnectString");
            SQL.DatabaseOpen();
            FunctionColor.Value = (SQL.ConnectString.Contains("DEV")) ? "0.5" : "1";
        }

        //開始処理
        private void OnLoad()
        {
            //設定
            WindowBehavior.Instance.iWindow = this;

            //ページの表示（Iniファイルから最初のページを取得する）
            StartPage();

            //ボタン設定
            VisiblePower.Value = false;
            VisibleList.Value = false;
            VisibleInfo.Value = false;
            VisibleDefect.Value = false;
            VisibleArrow.Value = false;
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
                    FramePage.Value.Navigate(new PlanList());
                    INI.WriteString("Page", "Initial", "PlanList");
                    break;

                case "F2":
                    //搬入登録画面
                    ViewModelInProcessInfo.Instance.InProcessCODE = null;
                    FramePage.Value.Navigate(new InProcessInfo());
                    INI.WriteString("Page", "Initial", "InProcessInfo");
                    break;

                case "F3":
                    //搬出登録画面
                    ViewModelTransportInfo.Instance.InProcessCODE.Value = null;
                    FramePage.Value.Navigate(new TransportList());
                    INI.WriteString("Page", "Initial", "TransportList");
                    break;

                case "F4":
                    //実績登録画面
                    ViewModelManufactureList.Instance.ManufactureCODE.Value = null;
                    FramePage.Value.Navigate(new ManufactureInfo());
                    INI.WriteString("Page", "Initial", "ManufactureInfo");
                    break;

                case "F5":
                    //作業マニュアル画面

                    break;

                case "F11":
                    //不良登録画面（Debug）
                    FramePage.Value.Navigate(new DefectInfo());
                    break;

                case "F12":
                    //設定画面
                    ViewModelManufactureInfo.Instance.Status = null;
                    FramePage.Value.Navigate(new Setting());
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
            if (DisplayStyle.Value == WindowStyle.None)
            {
                WindowLeft.Value = 0;
                WindowTop.Value = 0;
                WindowWidth.Value = 0;
                WindowHeight.Value = 0;
            }
            else
            {
                //Windowのサイズ・位置を復元 
                WindowLeft.Value = Properties.Settings.Default.WindowLeft;
                WindowTop.Value = Properties.Settings.Default.WindowTop;
                WindowWidth.Value = Properties.Settings.Default.WindowWidth;
                WindowHeight.Value = Properties.Settings.Default.WindowHeight;
            }

            WindowLeft.Value = 0;
            WindowTop.Value = 0;
            WindowWidth.Value = 0;
            WindowHeight.Value = 0;

            //Width・Heightのデフォルト値
            if (WindowWidth.Value <= 0) { WindowWidth.Value = 1280; }
            if (WindowHeight.Value <= 0) { WindowHeight.Value = 880; }
        }

        //Windowのサイズ・位置を記憶
        private void SaveWindowProperty()
        {
            if (DisplayState.Value == WindowState.Maximized) { return; }
            Properties.Settings.Default.WindowLeft = WindowLeft.Value;
            Properties.Settings.Default.WindowTop = WindowTop.Value;
            Properties.Settings.Default.WindowWidth = WindowWidth.Value;
            Properties.Settings.Default.WindowHeight = WindowHeight.Value;
            Properties.Settings.Default.Save();
        }

        //スワイプ処理
        public void ManipulationDelta(object? sender, ManipulationDeltaEventArgs e)
        {
            var window = WindowMain.Instance;
            var matrix = window.RenderTransform.Value;
            var delta = e.DeltaManipulation;
            var offsetFromParent = VisualTreeHelper.GetOffset(window);

            var offsetX = delta.Translation.X;
            var offsetY = delta.Translation.Y;
            matrix.Translate(offsetX, offsetY);

            //スワイプ処理
            if (offsetY > 80) { KeyDown("ESC"); }               //上から下（電源を切る）
            if (offsetY < -80) { KeyDown("F12"); }              //下から上（設定画面表示）
            if (offsetX > 80) { Ikeydown.Swipe("Left"); }       //左から右（一覧画面表示）
            if (offsetX < -80) { Ikeydown.Swipe("Right"); }     //右から左（入力画面表示）
        }

        //アイコン初期化
        public void InitializeIcon()
        {
            IconList.Value = "ViewList";
            IconPlan.Value = "FileClockOutline";
            IconSize.Value = 30;
        }

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
