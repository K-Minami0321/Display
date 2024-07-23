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
    public partial class InProcessInfo : UserControl
    {
        public InProcessInfo()
        {
            DataContext = ViewModelInProcessInfo.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelInProcessInfo : Common, IKeyDown, ITenKey, IWorker
    {
        //変数
        string processName;
        string inProcessCODE;
        string inProcessDate;
        string lotNumber;
        bool regFlg;
        int amountWidth = 150;
        int amountRow = 5;
        string notice;
        string buttonName;
        int lotNumberLength = 10;
        int amountLength = 6;
        int weightLength = 6;
        int unitLength = 6;
        string unitLabel;
        string weightLabel;
        bool visibleItem1;
        bool visibleItem2;
        bool visibleDelete;
        bool visibleCancel;
        bool visibleTenKey;
        bool visibleWorker;
        bool isEnable;
        bool isFocusLotNumber;
        bool isFocusWorker;
        bool isFocusWeight;
        bool isFocusUnit;
        bool isFocusCompleted;
        bool isFocusAmount;

        //プロパティ
        public static ViewModelInProcessInfo Instance   //インスタンス
        { get; set; } = new ViewModelInProcessInfo();
        public override string ProcessName              //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);
                if (value == null) { return; }
                iProcess = ProcessCategory.SetProcess(value);

                inProcess.ProcessName = ProcessName;
                inProcess.Place = iProcess.Name;                //保管場所
                inProcess.ProcessNext = iProcess.Next;          //次の工程設定

                switch (value)
                {
                    case "合板":
                        NextFocus = "Amount";
                        VisibleItem1 = true;
                        VisibleItem2 = true;
                        WeightLabel = "焼結重量";
                        UnitLabel = "重 量";
                        AmountRow = 5;
                        Notice = "※スリッター時のみ記入";
                        break;

                    case "プレス":
                        if (NextFocus != null) { NextFocus = "LotNumber"; }
                        VisibleItem1 = true;
                        VisibleItem2 = false;
                        WeightLabel = "単 重";
                        UnitLabel = "数 量";
                        AmountRow = 5;
                        Notice = string.Empty;
                        break;

                    default:
                        if (NextFocus != null) { NextFocus = "LotNumber"; }
                        VisibleItem1 = false;
                        VisibleItem2 = false;
                        UnitLabel = "数 量";
                        AmountRow = 4;
                        Notice = string.Empty;
                        break;
                }
            }
        }
        public override string InProcessCODE            //仕掛在庫CODE
        {
            get { return inProcessCODE; }
            set
            {
                SetProperty(ref inProcessCODE, value);
                ButtonName = string.IsNullOrEmpty(value) ? "登　録" : "修　正";
            }
        }
        public override string LotNumber                //ロット番号（テキストボックス）
        {
            get { return lotNumber; }
            set { SetProperty(ref lotNumber, value); }
        }
        public bool RegFlg                              //新規・既存フラグ
        {
            get { return regFlg; }
            set 
            { 
                SetProperty(ref regFlg, value);
                VisibleCancel = value;
                VisibleDelete = !value;

                //データ取得
                if (!value) 
                { 
                    DisplayData();
                }
                else
                {
                    //予定表からロット番号取得
                    LotNumber = management.Display(ViewModelPlanList.Instance.LotNumber);
                    DisplayLot();
                    SetGotFocus("Worker");
                }
            }
        }
        public int AmountWidth                          //コイル数テキストボックスのWidth
        {
            get { return amountWidth; }
            set { SetProperty(ref amountWidth, value); }
        }
        public int AmountRow                            //数量・重量の位置
        {
            get { return amountRow; }
            set { SetProperty(ref amountRow, value); }
        }
        public string Notice                            //注意文
        {
            get { return notice; }
            set { SetProperty(ref notice, value); }
        }
        public string ButtonName                        //登録ボタン名
        {
            get { return buttonName; }
            set { SetProperty(ref buttonName, value); }
        }
        public int LotNumberLength                      //文字数（ロット番号）
        {
            get { return lotNumberLength; }
            set { SetProperty(ref lotNumberLength, value); }
        }
        public int AmountLength                         //文字数（数量）
        {
            get { return amountLength; }
            set { SetProperty(ref amountLength, value); }
        }
        public int WeightLength                         //文字数（重量・焼結重量）
        {
            get { return weightLength; }
            set { SetProperty(ref weightLength, value); }
        }
        public int UnitLength                           //文字数（枚数・コイル数）
        {
            get { return unitLength; }
            set { SetProperty(ref unitLength, value); }
        }
        public string UnitLabel                         //ラベル（重量・数量）
        {
            get { return unitLabel; }
            set { SetProperty(ref unitLabel, value); }
        }
        public string WeightLabel                       //ラベル（重量・焼結重量）
        {
            get { return weightLabel; }
            set { SetProperty(ref weightLabel, value); }
        }
        public bool VisibleItem1                        //表示・非表示（入力項目）
        {
            get { return visibleItem1; }
            set { SetProperty(ref visibleItem1, value); }
        }
        public bool VisibleItem2                        //表示・非表示（入力項目）
        {
            get { return visibleItem2; }
            set { SetProperty(ref visibleItem2, value); }
        }
        public bool VisibleDelete                       //表示・非表示（削除ボタン）
        {
            get { return visibleDelete; }
            set { SetProperty(ref visibleDelete, value); }
        }
        public bool VisibleCancel                       //表示・非表示（取消ボタン）
        {
            get { return visibleCancel; }
            set { SetProperty(ref visibleCancel, value); }
        }
        public bool VisibleTenKey                       //表示・非表示（テンキー）
        {
            get { return visibleTenKey; }
            set { SetProperty(ref visibleTenKey, value); }
        }
        public bool VisibleWorker                       //表示・非表示（作業者）
        {
            get { return visibleWorker; }
            set { SetProperty(ref visibleWorker, value); }
        }
        public bool IsEnable                            //表示・非表示（下部ボタン）
        {
            get { return isEnable; }
            set 
            { 
                SetProperty(ref isEnable, value);
                if (value) { return; }
                Notice = string.Empty;
                AmountWidth = inProcess.Amount.Length * 50;
                inProcess.Amount = !management.VisibleCoil ? inProcess.Amount : CONVERT.ConvertCircleEnclosing(inProcess.Amount);
            }
        }
        public bool IsFocusLotNumber                    //フォーカス（ロット番号）
        {
            get { return isFocusLotNumber; }
            set { SetProperty(ref isFocusLotNumber, value); }
        }
        public bool IsFocusWorker                       //フォーカス（作業者）
        {
            get { return isFocusWorker; }
            set { SetProperty(ref isFocusWorker, value); }
        }
        public bool IsFocusWeight                       //フォーカス（重量）
        {
            get { return isFocusWeight; }
            set { SetProperty(ref isFocusWeight, value); }
        }
        public bool IsFocusUnit                         //フォーカス（単位）
        {
            get { return isFocusUnit; }
            set { SetProperty(ref isFocusUnit, value); }
        }
        public bool IsFocusCompleted                    //フォーカス（完了）
        {
            get { return isFocusCompleted; }
            set { SetProperty(ref isFocusCompleted, value); }
        }
        public bool IsFocusAmount                       //フォーカス（数量）
        {
            get { return isFocusAmount; }
            set { SetProperty(ref isFocusAmount, value); }
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
        internal ViewModelInProcessInfo()
        {
            inProcess = new InProcess();
            management = new Management();
            product = new Product();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            ViewModelControlTenKey.Instance.Itenkey = this;
            ViewModelControlWorker.Instance.Iworker = this;
            ViewModelWindowMain.Instance.ProcessName = INI.GetString("Page", "Process");
            DisplayCapution();

            //履歴から仕掛データ読み込み
            InProcessCODE = ViewModelInProcessList.Instance.InProcessCODE;
            RegFlg = string.IsNullOrEmpty(InProcessCODE);
            if (string.IsNullOrEmpty(inProcess.ProductName)) { SetGotFocus("LotNumber"); }
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            ViewModelWindowMain.Instance.ProcessWork = "仕掛搬入";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            Initialize(false);
        }

        //初期化
        public void Initialize(bool flg = true)
        {
            //入力データ初期化
            inProcess.InProcessDate = SetToDay(DateTime.Now);
            InProcessCODE = string.Empty;
            inProcess.ProductName = string.Empty;
            inProcess.LotNumber = string.Empty;
            inProcess.Amount = string.Empty;
            inProcess.Weight = string.Empty;
            inProcess.Unit = string.Empty;
            inProcess.Status = "搬入";
            inProcess.UnitLabel = "重 量";
            inProcess.Worker = INI.GetString("Page", "Worker");
            inProcess.Completed = string.Empty;
            inProcess.Coil = string.Empty;
            inProcess.ShirringUnit = string.Empty;
            inProcess.Comment = string.Empty;
            management.VisibleCoil = false;
            management.AmountLabel = "枚 数";
            LotNumber = string.Empty;
            AmountWidth = 150;
            RegFlg = true;
            IsEnable = true;
            IsFocusLotNumber = false;
            IsFocusWorker = false;
            IsFocusWeight = false;
            IsFocusUnit = false;
            IsFocusCompleted = false;
            IsFocusAmount = false;

            if (flg) 
            {
                ViewModelInProcessList.Instance.InProcessCODE = null;
                ViewModelPlanList.Instance.LotNumber = null;
            }
        }

        //ロット番号処理
        private void DisplayLot()
        {
            //データ表示
            iShape = Shape.SetShape(management.ShapeName);
            inProcess.LotNumber = LotNumber;
            inProcess.ProductName = management.ProductName;
            inProcess.UnitLabel = (ProcessName == "合板") ? "重 量" : "数 量";
            inProcess.DisplayInProcessData(LotNumber);              //仕掛情報取得
            inProcess.SetNextProcess(management.ProductCODE);       //製品によって次工程を設定

            //コイル数取得
            inProcess.Coil = inProcess.InProcessCoil(LotNumber, InProcessCODE);
            if (string.IsNullOrEmpty(management.ShirringUnit)) { management.VisibleCoil = false; }

            //サウンド再生
            if (!string.IsNullOrEmpty(management.ProductName) && management.ProductName != inProcess.ProductName) { SOUND.PlayAsync(SoundFolder + CONST.SOUND_LOT); }
        }

        //データ表示
        private void DisplayData()
        {
            inProcess.Select(InProcessCODE);
            LotNumber = management.Display(inProcess.LotNumber);
            DisplayLot();
            IsEnable = DATETIME.ToStringDate(inProcess.InProcessDate) < SetVerificationDay(DateTime.Now) ? false : true;
            SetGotFocus("LotNumber");
        }

        //キーイベント
        public async void KeyDown(object value)
        {
            var result = false;
            switch (value)
            {
                case "Regist":
                    //登録
                    if (await IsRequiredRegist())
                    {
                        result = (bool)await DialogHost.Show(new ControlMessage("搬入データを登録します", "", "警告"));
                        await System.Threading.Tasks.Task.Delay(100);
                        SetGotFocus(Focus);
                        if (result) { RegistData(); SetGotFocus("LotNumber"); }
                    }
                    break;

                case "Delete":
                    //削除
                    result = (bool)await DialogHost.Show(new ControlMessage("搬入データを削除します", "※削除されたデータは復元できません", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result) { DeleteDate(); SetGotFocus("LotNumber"); }
                    break;

                case "Cancel":
                    //取消
                    result = (bool)await DialogHost.Show(new ControlMessage("搬入データをクリアします", "※入力されたものが消去されます", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)  { Initialize(); SetGotFocus("LotNumber"); }
                    break;

                case "Enter":
                    //フォーカス移動
                    SetNextFocus();
                    break;

                case "BS":
                    //バックスペース処理
                    BackSpaceText();
                    break;

                case "CLEAR":
                    //文字列消去
                    ClearText();
                    break;

                case "1": case "2": case "3": case "4": case "5": case "6": case "7": case "8": case "9": case "0": case "-":
                    DisplayText(value);
                    break;

                case "Completed":
                    //完了チェック
                    inProcess.Completed = inProcess.Completed == "E" ? "" : "E";
                    break;

                case "DisplayInfo":
                    //搬入登録画面
                    Initialize();
                    SetGotFocus("LotNumber");
                    break;

                case "DisplayList":
                    //仕掛在庫一覧画面
                    Initialize(); SetGotFocus("LotNumber");
                    ViewModelWindowMain.Instance.FramePage = new InProcessList();
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    Initialize(); SetGotFocus("LotNumber");
                    ViewModelWindowMain.Instance.FramePage = new PlanList();
                    break;
            }
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus)
            {
                case "Worker":
                    inProcess.Worker = value.ToString();
                    break;
            }
            SetNextFocus();
        }

        //登録処理
        private void RegistData()
        {
            //コード確定
            if (RegFlg)
            {
                var inprocessdate = STRING.ToDateDB(inProcess.InProcessDate);
                var inprocesscode = inProcess.GenerateCode(iProcess.Mark + inprocessdate);
                InProcessCODE = inprocesscode;
                inProcess.InProcessDate = SetToDay(DateTime.Now);       //強制的に本日にする
            }

            //登録処理
            inProcess.InsertLog(RegFlg);
            inProcess.Resist(InProcessCODE);

            //処理完了
            Initialize();
        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(inProcess.Unit))
            {
                focus = "Unit";
                messege1 = "数量を入力してください";
                messege2 = "※数量は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (string.IsNullOrEmpty(inProcess.Worker))
            {
                focus = "Worker";
                messege1 = "担当者を選択してください";
                messege2 = "※担当者は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (string.IsNullOrEmpty(LotNumber))
            {
                focus = "LotNumber";
                messege1 = "ロット番号を入力してください";
                messege2 = "※ロット番号は必須項目です。";
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

        //削除処理
        private void DeleteDate()
        {
            //削除処理
            inProcess.DeleteLog();
            inProcess.DeleteHistory(InProcessCODE);

            //処理完了
            Initialize();
        }

        //入力制御
        private void DisplayText(object value)
        {

            switch (Focus)
            {
                case "LotNumber":
                    
                    if (LotNumber == null) { LotNumber = string.Empty; }
                    if (LotNumber.Length < LotNumberLength) { LotNumber += value.ToString(); }
                    break;

                case "Unit":
                    if (inProcess.Unit.Length < UnitLength) { inProcess.Unit += value.ToString(); }
                    break;

                case "Weight":
                    if (inProcess.Weight.Length < WeightLength) { inProcess.Weight += value.ToString(); }
                    break;

                case "Amount":
                    if (inProcess.Amount.Length < AmountLength) { inProcess.Amount += value.ToString(); }
                    break;

                default:
                    break;
            }
        }

        //文字列消去
        private void ClearText()
        {
            switch (Focus)
            {
                case "LotNumber":
                    LotNumber = string.Empty;
                    break;

                case "Unit":
                    inProcess.Unit = string.Empty;
                    break;

                case "Weight":
                    inProcess.Weight = string.Empty;
                    break;

                case "Amount":
                    inProcess.Amount = string.Empty;
                    break;

                default:
                    break;
            }
        }

        //バックスペース処理
        private void BackSpaceText()
        {
            switch (Focus)
            {
                case "LotNumber":
                    if (LotNumber.Length > 0) { LotNumber = LotNumber[..^1]; }
                    break;

                case "Unit":
                    if (inProcess.Unit.Length > 0) { inProcess.Unit = inProcess.Unit[..^1]; }
                    break;

                case "Weight":
                    if (inProcess.Weight.Length > 0) { inProcess.Weight = inProcess.Weight[..^1]; }
                    break;

                case "Amount":
                    if (inProcess.Amount.Length > 0) { inProcess.Amount = inProcess.Amount[..^1]; }
                    break;

                default:
                    break;
            }
        }

        //次のフォーカスへ
        private void SetNextFocus()
        {
            switch (Focus)
            {
                case "LotNumber":
                    SetGotFocus("Worker");
                    break;

                case "Worker":
                    SetGotFocus("Weight");
                    break;

                case "Weight":
                    SetGotFocus("Unit");
                    break;

                case "Unit":
                    SetGotFocus("Completed");
                    break;

                case "Completed":
                    SetGotFocus(NextFocus);
                    break;

                case "Amount":
                    SetGotFocus("LotNumber");
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
                case "LotNumber":
                    IsFocusLotNumber = true;
                    IsFocusWorker = false;
                    IsFocusWeight = false;
                    IsFocusUnit = false;
                    IsFocusCompleted = false;
                    IsFocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    ViewModelControlTenKey.Instance.InputString = "-";
                    break;

                case "Worker":
                    IsFocusLotNumber = false;
                    IsFocusWorker = true;
                    IsFocusWeight = false;
                    IsFocusUnit = false;
                    IsFocusCompleted = false;
                    IsFocusAmount = false;
                    VisibleTenKey = false;
                    VisibleWorker = true;
                    break;

                case "Weight":
                    IsFocusLotNumber = false;
                    IsFocusWorker = false;
                    IsFocusWeight = true;
                    IsFocusUnit = false;
                    IsFocusCompleted = false;
                    IsFocusAmount = false;

                    VisibleTenKey = true;
                    VisibleWorker = false;
                    ViewModelControlTenKey.Instance.InputString = ".";
                    break;

                case "Unit":
                    IsFocusLotNumber = false;
                    IsFocusWorker = false;
                    IsFocusWeight = false;
                    IsFocusUnit = true;
                    IsFocusCompleted = false;
                    IsFocusAmount = false;

                    VisibleTenKey = true;
                    VisibleWorker = false;
                    ViewModelControlTenKey.Instance.InputString = ".";
                    break;

                case "Completed":
                    IsFocusLotNumber = false;
                    IsFocusWorker = false;
                    IsFocusWeight = false;
                    IsFocusUnit = false;
                    IsFocusCompleted = true;
                    IsFocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    break;

                case "Amount":
                    IsFocusLotNumber = false;
                    IsFocusWorker = false;
                    IsFocusWeight = false;
                    IsFocusUnit = false;
                    IsFocusCompleted = false;
                    IsFocusAmount = true;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    ViewModelControlTenKey.Instance.InputString = ".";
                    break;

                default:
                    break;
            }
        }

        //ロット番号フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            LotNumber = management.Display(LotNumber);
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
