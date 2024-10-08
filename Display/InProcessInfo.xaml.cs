﻿using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class InProcessInfo : UserControl
    {
        public static string InProcessCODE     //仕掛CODE
        { get; set; }
        public static string LotNumber         //ロット番号
        { get; set; }

        //コンストラクター
        public InProcessInfo()
        {
            DataContext = new ViewModelInProcessInfo(InProcessCODE, LotNumber);
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelInProcessInfo : Common, IKeyDown, ITenKey, IWorker, ITimer
    {
        //変数
        ViewModelWindowMain windowMain;
        ViewModelControlTenKey controlTenKey;
        ViewModelControlWorker controlWorker;
       string processName;
        string inProcessCODE;
        string inProcessDate;
        string lotNumber;
        string amountLabel;
        string notice;
        int amountWidth = 150;
        int amountRow = 5;
        int lotNumberLength = 10;
        int amountLength = 6;
        int weightLength = 6;
        int unitLength = 6;
        string buttonName;
        string weightLabel;
        bool visibleCoil;
        bool visibleItem1;
        bool visibleItem2;
        bool visibleDelete;
        bool visibleCancel;
        bool visibleTenKey;
        bool visibleWorker;
        bool isRegist = true;
        bool isEnable;
        bool isFocusLotNumber;
        bool isFocusWorker;
        bool isFocusWeight;
        bool isFocusUnit;
        bool isFocusCompleted;
        bool isFocusAmount;

        //プロパティ
        public string ProcessName           //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);              
                process = new ProcessCategory(value);
                switch (value)
                {
                    case "合板":
                        NextFocus = "Amount";
                        VisibleItem1 = true;
                        VisibleItem2 = true;
                        WeightLabel = "焼結重量";
                        inProcess.UnitLabel = "重 量";
                        AmountRow = 5;
                        Notice = "※スリッター時のみ記入";
                        break;

                    case "プレス":
                        if (NextFocus != null) { NextFocus = "LotNumber"; }
                        VisibleItem1 = true;
                        VisibleItem2 = false;
                        WeightLabel = "単 重";
                        inProcess.UnitLabel = "数 量";
                        AmountRow = 5;
                        Notice = string.Empty;
                        VisibleCoil = false;
                        break;

                    default:
                        if (NextFocus != null) { NextFocus = "LotNumber"; }
                        VisibleItem1 = false;
                        VisibleItem2 = false;
                        inProcess.UnitLabel = "数 量";
                        AmountRow = 4;
                        Notice = string.Empty;
                        VisibleCoil = false;
                        break;
                }

                inProcess.ProcessName = ProcessName;
                inProcess.Place = process.Name;                //保管場所
                inProcess.ProcessNext = process.Next;          //次の工程設定
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
        public string LotNumber             //ロット番号
        {
            get { return lotNumber; }
            set { SetProperty(ref lotNumber, value); }
        }
        public string Notice                //注意文
        {
            get { return notice; }
            set { SetProperty(ref notice, value); }
        }
        public int AmountWidth              //コイル数テキストボックスのWidth
        {
            get { return amountWidth; }
            set { SetProperty(ref amountWidth, value); }
        }
        public int AmountRow                //数量・重量の位置
        {
            get { return amountRow; }
            set { SetProperty(ref amountRow, value); }
        }
        public int LotNumberLength          //文字数（ロット番号）
        {
            get { return lotNumberLength; }
            set { SetProperty(ref lotNumberLength, value); }
        }
        public int AmountLength             //文字数（数量）
        {
            get { return amountLength; }
            set { SetProperty(ref amountLength, value); }
        }
        public int WeightLength             //文字数（重量・焼結重量）
        {
            get { return weightLength; }
            set { SetProperty(ref weightLength, value); }
        }
        public int UnitLength               //文字数（枚数・コイル数）
        {
            get { return unitLength; }
            set { SetProperty(ref unitLength, value); }
        }
        public string ButtonName            //登録ボタン名
        {
            get { return buttonName; }
            set { SetProperty(ref buttonName, value); }
        }
        public string AmountLabel           //ラベル（数量）
        {
            get { return amountLabel; }
            set { SetProperty(ref amountLabel, value); }
        }
        public string WeightLabel           //ラベル（重量・焼結重量）
        {
            get { return weightLabel; }
            set { SetProperty(ref weightLabel, value); }
        }
        public bool VisibleCoil             //表示・非表示（コイル数）
        {
            get { return visibleCoil; }
            set { SetProperty(ref visibleCoil, value); }
        }
        public bool VisibleItem1            //表示・非表示（入力項目）
        {
            get { return visibleItem1; }
            set { SetProperty(ref visibleItem1, value); }
        }
        public bool VisibleItem2            //表示・非表示（入力項目）
        {
            get { return visibleItem2; }
            set { SetProperty(ref visibleItem2, value); }
        }
        public bool VisibleDelete           //表示・非表示（削除ボタン）
        {
            get { return visibleDelete; }
            set { SetProperty(ref visibleDelete, value); }
        }
        public bool VisibleCancel           //表示・非表示（取消ボタン）
        {
            get { return visibleCancel; }
            set { SetProperty(ref visibleCancel, value); }
        }
        public bool VisibleTenKey           //表示・非表示（テンキー）
        {
            get { return visibleTenKey; }
            set { SetProperty(ref visibleTenKey, value); }
        }
        public bool VisibleWorker           //表示・非表示（作業者）
        {
            get { return visibleWorker; }
            set { SetProperty(ref visibleWorker, value); }
        }
        public bool IsRegist                //新規・既存フラグ（true:新規、false:既存）
        {
            get { return isRegist; }
            set
            {
                SetProperty(ref isRegist, value);
                VisibleCancel = value;
                VisibleDelete = !value;
                ButtonName = value ? "登　録" : "修　正";
            }
        }
        public bool IsEnable                //表示・非表示（下部ボタン）
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
        public bool IsFocusLotNumber        //フォーカス（ロット番号）
        {
            get { return isFocusLotNumber; }
            set { SetProperty(ref isFocusLotNumber, value); }
        }
        public bool IsFocusWorker           //フォーカス（作業者）
        {
            get { return isFocusWorker; }
            set { SetProperty(ref isFocusWorker, value); }
        }
        public bool IsFocusWeight           //フォーカス（重量）
        {
            get { return isFocusWeight; }
            set { SetProperty(ref isFocusWeight, value); }
        }
        public bool IsFocusUnit             //フォーカス（単位）
        {
            get { return isFocusUnit; }
            set { SetProperty(ref isFocusUnit, value); }
        }
        public bool IsFocusCompleted        //フォーカス（完了）
        {
            get { return isFocusCompleted; }
            set { SetProperty(ref isFocusCompleted, value); }
        }
        public bool IsFocusAmount           //フォーカス（数量）
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
        internal ViewModelInProcessInfo(string code,string number)
        {
            inProcess = new InProcess();
            management = new Management();

            //データ取得
            Initialize();
            InProcessCODE = code;
            if (string.IsNullOrEmpty(code)) { LotNumber = number; DisplayLot(LotNumber); }     //予定表からロット番号取得

            //デフォルト値設定
            IsRegist = string.IsNullOrEmpty(inProcess.InProcessCODE);
            IsEnable = DATETIME.ToStringDate(inProcess.InProcessDate) < SetVerificationDay(DateTime.Now) ? false : true;
        }

        //ロード時
        private void OnLoad()
        {
            SetInterface();
            DisplayCapution();
            SetFocus();
        }

        //インターフェース設定
        private void SetInterface()
        {
            windowMain = ViewModelWindowMain.Instance;
            controlTenKey = ViewModelControlTenKey.Instance;
            controlWorker = ViewModelControlWorker.Instance;

            windowMain.Ikeydown = this;
            windowMain.Itimer = this;
            controlTenKey.Itenkey = this;
            controlWorker.Iworker = this;
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            windowMain.VisiblePower = true;
            windowMain.VisibleList = true;
            windowMain.VisibleInfo = true;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = false;
            windowMain.VisiblePlan = true;
            windowMain.InitializeIcon();
            windowMain.ProcessWork = "仕掛搬入";
            windowMain.ProcessName = ProcessName;
        }

        //初期化
        public void Initialize()
        {
            ProcessName = IniFile.GetString("Page", "Process");
            InProcessCODE = string.Empty;
            LotNumber = string.Empty;

            inProcess.Worker = IniFile.GetString("Page", "Worker");
            inProcess.InProcessDate = SetToDay(DateTime.Now);
            AmountWidth = 150;
            IsRegist = true;
            IsEnable = true;
        }

        //ロット番号処理
        private void DisplayLot(string lotnumber)
        {
            //データ取得
            CopyProperty(new Management(management.GetLotNumber(lotnumber), ProcessName), management);

            //データ表示
            if (!string.IsNullOrEmpty(management.ProductName) && management.ProductName != inProcess.ProductName) { Sound.PlayAsync(SoundFolder + CONST.SOUND_LOT); }
            shape = new ProductShape(management.ShapeName);
            LotNumber = management.LotNumber;
            inProcess.LotNumber = LotNumber;
            inProcess.Coil = inProcess.InProcessCoil(LotNumber, inProcess.InProcessCODE);   //コイル数取得
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
            if (IsRegist)
            {
                var inprocessdate = STRING.ToDateDB(inProcess.InProcessDate);
                var inprocesscode = inProcess.GenerateCode(process.Mark + inprocessdate);
                inProcess.InProcessCODE = inprocesscode;
            }

            //登録処理
            inProcess.ProductName = management.ProductName;
            inProcess.Status = "搬入";
            inProcess.TransportDate = string.Empty;
            inProcess.InsertLog(IsRegist);
            inProcess.Resist(inProcess.InProcessCODE);
            IsRegist = true;
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
            inProcess.DeleteLog();
            inProcess.DeleteHistory(inProcess.InProcessCODE);
            Initialize();
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
                        if (result)
                        {
                            RegistData();
                            SetGotFocus("LotNumber");
                        }
                    }
                    break;

                case "Delete":
                    //削除
                    result = (bool)await DialogHost.Show(new ControlMessage("搬入データを削除します", "※削除されたデータは復元できません", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        DeleteDate();
                        windowMain.FramePage = new InProcessList();
                    }
                    break;

                case "Cancel":
                    //取消
                    result = (bool)await DialogHost.Show(new ControlMessage("搬入データをクリアします", "※入力されたものが消去されます", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        Initialize();
                        SetGotFocus("LotNumber");
                    }
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

                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "0":
                case "-":
                    DisplayText(value);
                    break;

                case "Completed":
                    //完了チェック
                    inProcess.Completed = inProcess.Completed == "E" ? "" : "E";
                    break;

                case "DisplayInfo":
                    //搬入登録画面
                    Initialize();
                    SetFocus();
                    break;

                case "DisplayList":
                    //仕掛在庫一覧画面
                    windowMain.FramePage = new InProcessList();
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    windowMain.FramePage = new PlanList();
                    break;
            }
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
                    controlTenKey.InputString = "-";
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
                    controlTenKey.InputString = ".";
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
                    controlTenKey.InputString = ".";
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
                    controlTenKey.InputString = ".";
                    break;

                default:
                    break;
            }
        }

        //ロット番号フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            DisplayLot(LotNumber);
        }

        //フォーカス設定
        private void SetFocus()
        {
            if (string.IsNullOrEmpty(LotNumber)) { SetGotFocus("LotNumber"); return; }
            if (string.IsNullOrEmpty(inProcess.Worker)) { SetGotFocus("Worker"); return; }
            SetGotFocus("LotNumber");
        }

        //現在の日付設定
        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsRegist) { inProcess.InProcessDate = SetToDay(DateTime.Now); }
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
