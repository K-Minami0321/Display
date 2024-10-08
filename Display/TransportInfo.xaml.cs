﻿using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class TransportInfo : UserControl
    {
        public static string InProcessCODE     //仕掛CODE
        { get; set; }

        //コンストラクター
        public TransportInfo()
        {
            DataContext = new ViewModelTransportInfo(InProcessCODE);
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportInfo : Common, IKeyDown, IWorker, ITimer
    {
        //変数
        ViewModelWindowMain windowMain;
        ViewModelControlWorker controlWorker;
        string processName;
        string inProcessCODE;
        string buttonName;
        bool regFlg;
        bool isEnable;
        bool visibleWorker;
        bool visibleCancel;
        bool isFocusWorker;

        //プロパティ
        public string ProcessName           //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);
                process = new ProcessCategory(value);
            }
        }
        public string InProcessCODE         //仕掛CODE
        {
            get { return inProcessCODE; }
            set
            {
                SetProperty(ref inProcessCODE, value);
                CopyProperty(new InProcess(inProcessCODE, ProcessName), inProcess);
                DisplayLot(inProcess.LotNumber);
            }
        }
        public string ButtonName            //登録ボタン名
        {
            get { return buttonName; }
            set { SetProperty(ref buttonName, value); }
        }
        public bool IsRegist                //新規・既存フラグ（true:新規、false:既存）
        {
            get { return regFlg; }
            set
            {
                SetProperty(ref regFlg, value);
                VisibleCancel = !value;
                ButtonName = value ? "登　録" : "修　正";
            }
        }
        public bool IsEnable                //表示・非表示（下部ボタン）
        {
            get { return isEnable; }
            set { SetProperty(ref isEnable, value); }
        }
        public bool VisibleWorker           //表示・非表示（作業者）
        {
            get { return visibleWorker; }
            set { SetProperty(ref visibleWorker, value); }
        }
        public bool VisibleCancel           //表示・非表示（取消ボタン）
        {
            get { return visibleCancel; }
            set { SetProperty(ref visibleCancel, value); }
        }
        public bool IsFocusWorker           //フォーカス（作業者）
        {
            get { return isFocusWorker; }
            set { SetProperty(ref isFocusWorker, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand gotFocus;
        public ICommand GotFocus => gotFocus ??= new ActionCommand(SetGotFocus);
        ActionCommand lostFocus;
        public ICommand LostFocus => lostFocus ??= new ActionCommand(SetLostFocus);

        //コンストラクター
        internal ViewModelTransportInfo(string code)
        {
            inProcess = new InProcess();
            management = new Management();

            //仕掛移動データ取得
            Initialize();
            InProcessCODE = code;

            //デフォルト値設定
            IsRegist = (inProcess.Status == "搬入");
            IsEnable = DATETIME.ToStringDate(inProcess.TransportDate) < SetVerificationDay(DateTime.Now) ? false : true;
        }

        //ロード時
        private void OnLoad()
        {
            SetInterface();
            DisplayCapution();
            SetGotFocus("Worker");
        }

        //インターフェース設定
        private void SetInterface()
        {
            windowMain = ViewModelWindowMain.Instance;
            controlWorker = ViewModelControlWorker.Instance;

            windowMain.Ikeydown = this;
            windowMain.Itimer = this;
            controlWorker.Iworker = this;
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            windowMain.VisiblePower = true;
            windowMain.VisibleList = true;
            windowMain.VisibleInfo = false;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = false;
            windowMain.VisiblePlan = true;
            windowMain.InitializeIcon();
            windowMain.ProcessWork = "仕掛引取";
            windowMain.IconPlan = "FileDocumentArrowRightOutline";
            windowMain.ProcessName = ProcessName;
        }

        //初期化
        public void Initialize()
        {
            ProcessName = IniFile.GetString("Page", "Process");
            inProcess.TransportWorker = IniFile.GetString("Page", "Worker");
            inProcess.TransportDate = SetToDay(DateTime.Now);
        }

        //ロット番号処理
        private void DisplayLot(string lotnumber)
        {
            //データ取得
            CopyProperty(new Management(management.GetLotNumber(lotnumber), ProcessName), management);

            //データ表示
            if (!string.IsNullOrEmpty(management.ProductName) && management.ProductName != inProcess.ProductName) { Sound.PlayAsync(SoundFolder + CONST.SOUND_LOT); }
            shape = new ProductShape(management.ShapeName);
        }

        //キーイベント
        public async void KeyDown(object value)
        {
            var result = false;
            switch (value)
            {
                case "Regist":
                    //登録
                    if (await IsRequiredRegist()) { RegistData(); } 
                    break;

                case "Cancel":
                    //取消（合板に戻す）
                    result = (bool)await DialogHost.Show(new ControlMessage("仕掛引取一覧に戻ります。", "※入力されたものが消去されます", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    CancelData();
                    SetGotFocus(Focus);
                    if (result)
                    {
                        CancelData();
                        windowMain.FramePage = new TransportList();
                    }
                    break;

                case "Enter":
                    //フォーカス移動
                    NextFocus();
                    break;

                case "DisplayInfo":
                    //引取登録画面
                    Initialize();
                    SetGotFocus("Worker");
                    break;

                case "DisplayList":
                    //引取履歴画面
                    windowMain.FramePage = new TransportHistory();
                    break;

                case "DisplayPlan":
                    //仕掛置場
                    windowMain.FramePage = new TransportList();
                    break;
            }
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus)
            {
                case "Worker":
                    inProcess.TransportWorker = value.ToString();
                    IsFocusWorker = false;
                    break;
            }
            NextFocus();
        }

        //登録処理
        private void RegistData()
        {
            //登録
            inProcess.Place = "プレス";
            inProcess.Status = "引取";
            inProcess.TransportResist(inProcess.InProcessCODE);
            windowMain.FramePage = new TransportList();
        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(inProcess.TransportWorker))
            {
                focus = "Worker";
                messege1 = "担当者を選択してください";
                messege2 = "※担当者は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (!result) 
            {
                var messege = (bool)await DialogHost.Show(new ControlMessage(messege1, messege2, messege3));
                await System.Threading.Tasks.Task.Delay(100);
                if (messege) { SetGotFocus(focus); }
            }
            return result;
        }

        //移動処理を元に戻す
        private void CancelData()
        {
            inProcess.DeleteLog();
            inProcess.Place = "合板";
            inProcess.Status = "搬入";
            inProcess.TransportDate = string.Empty;
            inProcess.TransportWorker = string.Empty;
            inProcess.TransportResist(inProcess.InProcessCODE);
        }

        //次のフォーカスへ
        private void NextFocus()
        {
            switch (Focus)
            {
                case "Worker":
                    IsFocusWorker = true;
                    break;

                default:
                    break;
            }
        }

        //フォーカス処理（GotFocus）
        private void SetGotFocus(object value)
        {
            Focus = value;
            switch (Focus)
            {
                case "Worker":
                    IsFocusWorker = true;
                    VisibleWorker = true;
                    break;

                default:
                    break;
            }
        }

        //フォーカス処理（LostFoucus）
        private void SetLostFocus(object value)
        {
            switch (value)
            {
                case "LotNumber":
                    DisplayLot(inProcess.LotNumber);
                    break;

                default :
                    break;
            }
        }

        //現在の日付設定
        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsRegist) { inProcess.TransportDate = SetToDay(DateTime.Now); }
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Left":
                    KeyDown("DisplayList");
                    break;
            }
        }
    }
}
