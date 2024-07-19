﻿using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class TransportInfo : UserControl
    {
        public TransportInfo()
        {
            DataContext = ViewModelTransportInfo.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportInfo : Common, IKeyDown, IWorker
    {
        //変数
        string inProcessCODE;
        string lotNumber;
        string processName;
        int amountLength = 6;
        bool visibleTenKey;
        bool visibleWorker;
        bool isFocusWorker;

        //プロパティ
        public static ViewModelTransportInfo Instance       //インスタンス
        { get; set; } = new ViewModelTransportInfo();
        public override string ProcessName                  //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);
                if (value == null) { return; }
                iProcess = ProcessCategory.SetProcess(value);
            }
        }
        public override string InProcessCODE                //仕掛コード
        {
            get { return inProcessCODE; }
            set 
            {
                SetProperty(ref inProcessCODE, value);
                if (value == null) { return; }
                DisplayData();
            }
        }
        public override string LotNumber                    //ロット番号
        {
            get { return lotNumber; }
            set { SetProperty(ref lotNumber, value); }
        }
        public int AmountLength                             //文字数（数量）
        {
            get { return amountLength; }
            set { SetProperty(ref amountLength, value); }
        }
        public bool VisibleTenKey                           //表示・非表示（テンキー）
        {
            get { return visibleTenKey; }
            set { SetProperty(ref visibleTenKey, value); }
        }
        public bool VisibleWorker                           //表示・非表示（作業者）
        {
            get { return visibleWorker; }
            set { SetProperty(ref visibleWorker, value); }
        }
        public bool IsFocusWorker                           //フォーカス（作業者）
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
        internal ViewModelTransportInfo()
        {
            inProcess = new InProcess();
            management = new Management();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            ViewModelControlWorker.Instance.Iworker = this;
            ViewModelWindowMain.Instance.ProcessName = INI.GetString("Page", "Process");
            DisplayCapution();
            InProcessCODE = ViewModelTransportList.Instance.InProcessCODE;
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            ViewModelWindowMain.Instance.ProcessWork = "仕掛引取";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = false;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconPlan = "FileDocumentArrowRightOutline";
            Initialize();
        }

        //初期化
        public void Initialize()
        {
            //入力データ初期化
            inProcess.TransportDate = SetToDay(DateTime.Now);
            inProcess.ProductName = string.Empty;
            inProcess.LotNumber = string.Empty;
            inProcess.Amount = string.Empty;
            inProcess.TransportWorker = INI.GetString("Page", "Worker");
            IsFocusWorker = true;
        }

        //ロット番号処理
        private void DisplayLot()
        {
            //データ表示
            iShape = Shape.SetShape(management.ShapeName);
            inProcess.LotNumber = LotNumber;

            //サウンド再生
            if (!string.IsNullOrEmpty(management.ProductName) && management.ProductName != inProcess.ProductName) { SOUND.PlayAsync(SoundFolder + CONST.SOUND_LOT); }
        }

        //データ表示
        private void DisplayData()
        {
            //前工程の仕掛取得
            inProcess.TransportSelect(InProcessCODE);
            LotNumber = management.Display(inProcess.LotNumber);
            SetGotFocus("Worker");
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
                    //取消
                    result = (bool)await DialogHost.Show(new ControlMessage("仕掛引取一覧に戻ります。", "※入力されたものが消去されます", "警告"));
                    ViewModelWindowMain.Instance.FramePage = new TransportList();
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
                    ViewModelWindowMain.Instance.FramePage = new TransportHistory();
                    break;

                case "DisplayPlan":
                    //仕掛置場
                    ViewModelWindowMain.Instance.FramePage = new TransportList();
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
            inProcess.Status = "引取";
            inProcess.Place = ProcessName;
            inProcess.TransportResist(InProcessCODE);
            ViewModelWindowMain.Instance.FramePage = new TransportList();
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
                    VisibleTenKey = false;
                    VisibleWorker = true;
                    break;

                default:
                    break;
            }
        }

        //フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            LotNumber = management.Display(manufacture.LotNumber);
            DisplayLot();
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
