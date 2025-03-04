using ClassBase;
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
    public partial class ManufactureInfo : UserControl
    {
        public static string ManufactureCODE    //製造CODE
        { get; set; }
        public static string LotNumber          //ロット番号
        { get; set; }

        //コンストラクター
        public ManufactureInfo()
        {
            DataContext = new ViewModelManufactureInfo(ManufactureCODE, LotNumber);
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelManufactureInfo : Common, IWindowBase, IBarcode, ITenKey, IWorker, IWorkProcess, ITimer
    {
        //変数
        string processName;
        string manufactureCODE;
        string manufactureDate;
        string lotNumber = string.Empty;
        string lotNumberSEQ = string.Empty;
        string productName = string.Empty;
        string workProcess = string.Empty;
        string startTime = string.Empty;
        string endTime = string.Empty;
        string workTime = string.Empty;
        string amount = string.Empty;
        string equipmentCODE = string.Empty;
        string equipment1 = string.Empty;
        string equipment2 = string.Empty;
        string team = string.Empty;
        string completed = string.Empty;
        string sales = string.Empty;
        string breakName;
        string buttonName = "数 量";
        string labelAmount;
        int lengthLotNumber = 10;
        int lengthStartTime = 4;
        int lengthEndTime = 4;
        int lengthAmount = 5;
        bool enabledControl1;
        bool enabledControl2;
        bool visiblePackaging;
        bool visibleButtonStart;
        bool visibleButtonEnd;
        bool visibleButtonCancel;
        bool visibleButtonBreak;
        bool visibleEdit;
        bool visibleTenKey;
        bool visibleWorker;
        bool visibleWorkProcess;
        bool visibleSeal;
        bool isRegist;
        bool isEnable;
        bool isConvertTime;
        bool focusLotNumber;
        bool focusWorker;
        bool focusWorkProcess;
        bool focusStartTime;
        bool focusEndTime;
        bool focusAmount;
        bool focusCompleted;
        bool focusSales;

        //プロパティ
        public string ManufactureCODE           //製造CODE
        {
            get => manufactureCODE;
            set 
            { 
                IsRegist = string.IsNullOrEmpty(value);
                
                var manufacture = new Manufacture(value);
                CopyProperty(manufacture, this, "ManufactureCODE");
                DisplayLot(LotNumber);
                SetProperty(ref manufactureCODE, value);
            }
        }
        public string ProcessName               //工程区分
        {
            get => processName;
            set
            {
                switch (value)
                {
                    case "検査":
                        VisibleSeal = false;
                        VisiblePackaging = true;
                        if (IsRegist) { WorkProcess = "検査"; }
                        break;

                    case "梱包":
                        VisibleSeal = false;
                        VisiblePackaging = false;
                        if (IsRegist) { WorkProcess = "梱包"; }
                        break;

                    default:
                        VisibleSeal = true;
                        VisiblePackaging = true;
                        if (IsRegist) { WorkProcess = ""; }
                        break;
                }
                SetProperty(ref processName, value);
            }
        }
        public string ManufactureDate           //製造日
        {
            get => manufactureDate;
            set 
            { 
                IsEnable = value.ToDate() < SetVerificationDay(DateTime.Now) ? false : true;
                SetProperty(ref manufactureDate, value);
            }
        }
        public string LotNumber                 //ロット番号（テキストボックス）
        {
            get => lotNumber;
            set => SetProperty(ref lotNumber, value);
        }
        public string LotNumberSEQ              //ロット番号SEQ
        {
            get => lotNumberSEQ;
            set => SetProperty(ref lotNumberSEQ, value);
        }
        public string ProductName               //品番
        {
            get => productName;
            set => SetProperty(ref productName, value);
        }
        public string WorkProcess               //工程
        {
            get => workProcess;
            set => SetProperty(ref workProcess, value);
        }
        public string StartTime                 //開始時刻
        {
            get => startTime;
            set 
            {
                var manufacture = new Manufacture();
                manufacture.IsConvertTime = IsConvertTime;
                manufacture.ManufactureDate = ManufactureDate;
                manufacture.StartTime = value;
                manufacture.EndTime = EndTime;

                SetProperty(ref startTime, manufacture.StartTime);
                WorkTime = manufacture.WorkTime;

            }
        }
        public string EndTime                   //終了時刻
        {
            get => endTime;
            set
            {
                var manufacture = new Manufacture();
                manufacture.IsConvertTime = IsConvertTime;
                manufacture.ManufactureDate = ManufactureDate;
                manufacture.StartTime = StartTime;
                manufacture.EndTime = value;

                SetProperty(ref endTime, manufacture.EndTime);
                WorkTime = manufacture.WorkTime;
            }
        }
        public string WorkTime                  //作業時間
        {
            get => workTime;
            set => SetProperty(ref workTime, value);
        }
        public string Amount                    //枚数・コイル数・個数
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }
        public string EquipmentCODE             //設備CODE
        {
            get => equipmentCODE;
            set 
            {
                var equipment = new Equipment();
                equipment.EquipmentCODE = value;

                var name = equipment.EquipmentName;
                Equipment1 = value;
            }
        }
        public string Equipment1                //設備
        {
            get => equipment1;
            set => SetProperty(ref equipment1, value);
        }
        public string Equipment2                //設備
        {
            get => equipment2;
            set => SetProperty(ref equipment2, value);
        }
        public string Team                      //班名
        {
            get => team;
            set => SetProperty(ref team, value);
        }
        public string Completed                 //完了
        {
            get => completed;
            set => SetProperty(ref completed, value);
        }
        public string Sales                     //売上
        {
            get => sales;
            set => SetProperty(ref sales, value);
        }
        public string ButtonName                //登録ボタン名
        {
            get => buttonName;
            set => SetProperty(ref buttonName, value);
        }
        public string BreakName                 //中断ボタン名
        {
            get => breakName;
            set => SetProperty(ref breakName, value);
        }
        public string LabelAmount               //ラベル（数量）
        {
            get => labelAmount;
            set => SetProperty(ref labelAmount, value);
        }
        public int LengthLotNumber              //文字数（ロット番号）
        {
            get => lengthLotNumber;
            set => SetProperty(ref lengthLotNumber, value);
        }
        public int LengthStartTime              //文字数（開始時間）
        {
            get => lengthStartTime;
            set => SetProperty(ref lengthStartTime, value);
        }
        public int LengthEndTime                //文字数（終了時間）
        {
            get => lengthEndTime;
            set => SetProperty(ref lengthEndTime, value);
        }
        public int LengthAmount                 //文字数（数量）
        {
            get => lengthAmount;
            set => SetProperty(ref lengthAmount, value);
        }
        public bool EnabledControl1             //コントロール使用可能
        {
            get => enabledControl1;
            set
            {
                SetProperty(ref enabledControl1, value);
                VisibleButtonStart = value;
            }
        }
        public bool EnabledControl2             //コントロール使用可能
        {
            get => enabledControl2;
            set => SetProperty(ref enabledControl2, value);
        }
        public bool VisiblePackaging            //表示・非表示（数量)
        {
            get => visiblePackaging;
            set => SetProperty(ref visiblePackaging, value);
        }
        public bool VisibleButtonStart          //開始ボタン表示・非表示
        {
            get => visibleButtonStart;
            set => SetProperty(ref visibleButtonStart, value);
        }
        public bool VisibleButtonEnd            //終了ボタン表示・非表示
        {
            get => visibleButtonEnd;
            set => SetProperty(ref visibleButtonEnd, value);
        }
        public bool VisibleButtonCancel         //取消ボタン表示・非表示
        {
            get => visibleButtonCancel;
            set => SetProperty(ref visibleButtonCancel, value);
        }
        public bool VisibleButtonBreak          //中断ボタン表示・非表示
        {
            get => visibleButtonBreak;
            set => SetProperty(ref visibleButtonBreak, value);
        }
        public bool VisibleEdit                 //表示・非表示（削除ボタン）
        {
            get => visibleEdit;
            set => SetProperty(ref visibleEdit, value);
        }
        public bool VisibleTenKey               //表示・非表示（テンキー）
        {
            get => visibleTenKey;
            set => SetProperty(ref visibleTenKey, value);
        }
        public bool VisibleWorker               //表示・非表示（作業者）
        {
            get => visibleWorker;
            set => SetProperty(ref visibleWorker, value);
        }
        public bool VisibleWorkProcess          //表示・非表示（工程）
        {
            get => visibleWorkProcess;
            set => SetProperty(ref visibleWorkProcess, value);
        }
        public bool VisibleSeal                 //表示・非表示（売上）
        {
            get => visibleSeal;
            set => SetProperty(ref visibleSeal, value);
        }
        public bool IsRegist                    //新規・既存フラグ（true:新規、false:既存）
        {
            get => isRegist;
            set
            {
                SetProperty(ref isRegist, value);
                //EnabledControl1 = !value;
                //EnabledControl2 = !value;
                //VisibleButtonStart = value;
                ButtonName = value ? "登　録" : "修　正";
            }
        }
        public bool IsEnable                    //表示・非表示（下部ボタン）
        {
            get => isEnable;
            set => SetProperty(ref isEnable, value);
        }
        public bool IsConvertTime               //時間変換をするか
        {
            get => isConvertTime;
            set => isConvertTime = value;
        }
        public bool FocusLotNumber              //フォーカス（ロット番号）
        {
            get => focusLotNumber;
            set => SetProperty(ref focusLotNumber, value);
        }
        public bool FocusWorker                 //フォーカス（作業者）
        {
            get => focusWorker;
            set => SetProperty(ref focusWorker, value);
        }
        public bool FocusWorkProcess            //フォーカス（工程）
        {
            get => focusWorkProcess;
            set => SetProperty(ref focusWorkProcess, value);
        }
        public bool FocusStartTime              //フォーカス（開始時間）
        {
            get => focusStartTime;
            set => SetProperty(ref focusStartTime, value);
        }
        public bool FocusEndTime                //フォーカス（終了時間）
        {
            get => focusEndTime;
            set => SetProperty(ref focusEndTime, value);
        }
        public bool FocusAmount                 //フォーカス（数量）
        {
            get => focusAmount;
            set => SetProperty(ref focusAmount, value);
        }
        public bool FocusCompleted              //フォーカス（完了）
        {
            get => focusCompleted;
            set => SetProperty(ref focusCompleted, value);
        }
        public bool FocusSales                  //フォーカス（売上）
        {
            get => focusSales;
            set => SetProperty(ref focusSales, value);
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
        internal ViewModelManufactureInfo(string code, string number)
        {
            Ibarcode = this;
            Initialize();
            ManufactureCODE = code;
            if (string.IsNullOrEmpty(code)) { LotNumber = number; DisplayLot(LotNumber); }
        }

        //ロード時
        private void OnLoad()
        {
            CtrlWindow.Interface = this;
            CtrlWindow.Itimer = this;
            DisplayCapution();
            SetFocus();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            CtrlWindow.VisiblePower = true;
            CtrlWindow.VisibleList = true;
            CtrlWindow.VisibleInfo = true;
            CtrlWindow.VisibleDefect = false;
            CtrlWindow.VisibleArrow = false;
            CtrlWindow.VisiblePlan = true;
            CtrlWindow.InitializeIcon();
            CtrlWindow.ProcessName = ProcessName;
            ViewModelControlTenKey.Instance.Itenkey = this;
            ViewModelControlWorker.Instance.Iworker = this;
            ViewModelControlWorkProcess.Instance.IworkProcess = this;
            CtrlWindow.ProcessWork = string.IsNullOrEmpty(Equipment1) ? ProcessName + "実績" : Equipment1 + " - " + EquipmentCODE;

            switch (Mode)
            {
                case "登録":
                    EnabledControl1 = true;
                    EnabledControl2 = true;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;
                    VisibleEdit = true;
                    CtrlWindow.VisibleList = true;
                    CtrlWindow.VisibleDefect = false;
                    CtrlWindow.VisibleArrow = false;
                    SetGotFocus("LotNumber");
                    break;

                case "準備":
                    EnabledControl1 = true;
                    EnabledControl2 = false;
                    VisibleButtonStart = true;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;
                    CtrlWindow.VisibleList = true;
                    CtrlWindow.VisibleDefect = false;
                    CtrlWindow.VisibleArrow = false;
                    break;

                case "作業中":
                    EnabledControl1 = false;
                    EnabledControl2 = true;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = true;
                    VisibleButtonBreak = true;
                    VisibleButtonCancel = false;
                    BreakName = "中　断";
                    CtrlWindow.VisibleList = true;
                    CtrlWindow.VisibleDefect = false;
                    SetGotFocus("Amount");

                    break;

                case "中断":
                    EnabledControl1 = false;
                    EnabledControl2 = false;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = true;
                    VisibleButtonCancel = true;
                    BreakName = "再　開";
                    CtrlWindow.VisibleList = true;
                    CtrlWindow.VisibleDefect = false;
                    break;

                case "編集":
                    EnabledControl1 = true;
                    EnabledControl2 = true;
                    VisibleEdit = true;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;
                    CtrlWindow.VisibleList = true;
                    CtrlWindow.VisibleDefect = false;
                    break;

                default:
                    Mode = "準備";
                    EnabledControl1 = true;
                    EnabledControl2 = false;
                    VisibleButtonStart = true;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;
                    CtrlWindow.VisibleList = true;
                    CtrlWindow.VisibleDefect = false;
                    CtrlWindow.VisibleArrow = false;
                    break;
            }

            //ボタン設定
            CtrlWindow.VisiblePower = true;
            CtrlWindow.VisibleInfo = true;
            CtrlWindow.VisibleArrow = false;
        }

        //初期化
        public void Initialize()
        {
            ReadINI();
            ManufactureCODE = string.Empty;
            ManufactureDate = SetToDay(DateTime.Now);
            LotNumber = string.Empty;
            ProductName = string.Empty;
            WorkProcess = string.Empty;
            StartTime = string.Empty;
            EndTime = string.Empty;
            WorkTime = string.Empty;
            Amount = string.Empty;
            Completed = string.Empty;
            IsRegist = true;
            IsEnable = true;
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus)
            {
                case "Worker":
                    Worker = value.ToString();
                    break;

                case "WorkProcess":
                    WorkProcess = value.ToString();
                    break;
            }
            SetNextFocus();
        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(Worker))
            {
                focus = "Worker";
                messege1 = "担当者を選択してください";
                messege2 = "※担当者は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            switch(ProcessName)
            {
                case "合板":
                case "プレス":
                case "仕上":
                    if (string.IsNullOrEmpty(WorkProcess))
                    {
                        focus = "WorkProcess";
                        messege1 = "工程を選択してください";
                        messege2 = "※工程は必須項目です。";
                        messege3 = "確認";
                        result = false;
                    }
                    break;
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
                CtrlMessage = new ControlMessage(messege1, messege2, messege3);
                var messege = (bool)await DialogHost.Show(CtrlMessage);
                await System.Threading.Tasks.Task.Delay(100);
                if (messege) { SetGotFocus(focus); }
            }
            return result;
        }

        //登録処理
        private void RegistData()
        {
            var manufacture = new Manufacture();
            CopyProperty(this, manufacture);

            //コード確定
            if (IsRegist)
            {
                var date = ManufactureDate.ToStringDateDB();
                var code = manufacture.GenerateCode(Mark + date);
                ManufactureCODE = code;
            }

            //登録処理
            manufacture.InsertLog(IsRegist);
            manufacture.Resist(ManufactureCODE);
            Initialize();
        }

        //削除処理
        private void DeleteDate()
        {
            var manufacture = new Manufacture();
            manufacture.DeleteLog();
            manufacture.Delete(ManufactureCODE);        //製造実績削除
            Initialize();
        }

        //入力制御
        private void DisplayText(object value)
        {
            switch (Focus)
            {
                case "LotNumber":
                    if (LotNumber.Length < LengthLotNumber) { LotNumber += value.ToString(); }
                    break;

                case "StartTime":
                    if (StartTime.Length > LengthStartTime) { return; }
                    IsConvertTime = !(StartTime.Length < 3);
                    StartTime += value.ToString();
                    IsConvertTime = true;
                    if (StartTime.Length > LengthStartTime - 1) { SetNextFocus(); }
                    break;

                case "EndTime":
                    if (EndTime.Length > LengthEndTime) { return; }
                    IsConvertTime = !(EndTime.Length < 3);
                    EndTime += value.ToString();
                    IsConvertTime = true;
                    if (EndTime.Length > LengthEndTime - 1) { SetNextFocus(); }
                    break;

                case "Amount":
                    if (Amount.Length < LengthAmount) { Amount += value.ToString(); }
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

                case "StartTime":
                    StartTime = string.Empty;
                    break;

                case "EndTime":
                    EndTime = string.Empty;
                    break;

                case "Amount":
                    Amount = string.Empty;
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

                case "StartTime":
                    if (StartTime.Length == 0) { return; }
                    IsConvertTime = false;
                    StartTime = StartTime[..^1];
                    break;

                case "EndTime":
                    if (EndTime.Length == 0) { return; }
                    IsConvertTime = false;
                    EndTime = EndTime[..^1];
                    break;

                case "Amount":
                    if (Amount.Length > 0) { Amount = Amount[..^1]; }
                    break;

                default:
                    break;
            }
        }

        //確定処理
        private void SetNextFocus()
        {
            switch(Mode)
            {
                case "登録": case "編集":
                    switch (Focus)
                    {
                        case "LotNumber":
                            SetGotFocus("WorkProcess");
                            break;

                        case "WorkProcess":
                            SetGotFocus("Worker");
                            break;

                        case "Worker":
                            SetGotFocus("StartTime");
                            break;

                        case "StartTime":
                            SetGotFocus("EndTime");
                            break;

                        case "EndTime":
                            SetGotFocus("Amount");
                            break;

                        case "Amount":
                            SetGotFocus("Completed");
                            break;

                        case "Completed":
                            SetGotFocus("Sales");
                            break;

                        case "Sales":
                            SetGotFocus("LotNumber");
                            break;

                        default:
                            break;
                    }
                    break;

                case "準備":
                    switch (Focus)
                    {
                        case "LotNumber":
                            SetGotFocus("WorkProcess");
                            break;

                        case "WorkProcess":
                            SetGotFocus("Worker");
                            break;

                        case "Worker":
                            SetGotFocus("LotNumber");
                            break;

                        default:
                            break;
                    }
                    break;

                default:

                    //作業中
                    switch (Focus)
                    {
                        case "Amount":
                            SetGotFocus("Completed");
                            break;

                        case "Completed":
                            SetGotFocus("Sales");
                            break;

                        case "Sales":
                            SetGotFocus("Amount");
                            break;

                        default:
                            break;
                    }
                    break;
            }
        }

        //フォーカス処理（GotForcus）
        private void SetGotFocus(object value)
        {
            var controlTenKey = ViewModelControlTenKey.Instance;

            Focus = value;
            switch (Focus)
            {
                case "LotNumber":
                    FocusLotNumber = true;
                    FocusWorkProcess = false;
                    FocusWorker = false;
                    FocusStartTime = false;
                    FocusEndTime = false;
                    FocusAmount = false;
                    FocusCompleted = false;
                    FocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = "-";
                    break;

                case "WorkProcess":
                    FocusLotNumber = false;
                    FocusWorkProcess = true;
                    FocusWorker = false;
                    FocusStartTime = false;
                    FocusEndTime = false;
                    FocusAmount = false;
                    FocusCompleted = false;
                    FocusSales = false;
                    VisibleTenKey = false;
                    VisibleWorker = false;
                    VisibleWorkProcess = true;
                    break;

                case "Worker":
                    FocusLotNumber = false;
                    FocusWorkProcess = false;
                    FocusWorker = true;
                    FocusStartTime = false;
                    FocusEndTime = false;
                    FocusAmount = false;
                    FocusCompleted = false;
                    FocusSales = false;
                    VisibleTenKey = false;
                    VisibleWorker = true;
                    VisibleWorkProcess = false;
                    break;

                case "StartTime":
                    FocusLotNumber = false;
                    FocusWorkProcess = false;
                    FocusWorker = false;
                    FocusStartTime = true;
                    FocusEndTime = false;
                    FocusAmount = false;
                    FocusCompleted = false;
                    FocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
                    break;

                case "EndTime":
                    FocusLotNumber = false;
                    FocusWorkProcess = false;
                    FocusWorker = false;
                    FocusStartTime = false;
                    FocusEndTime = true;
                    FocusAmount = false;
                    FocusCompleted = false;
                    FocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
                    break;

                case "Amount":
                    FocusLotNumber = false;
                    FocusWorkProcess = false;
                    FocusWorker = false;
                    FocusStartTime = false;
                    FocusEndTime = false;
                    FocusAmount = true;
                    FocusCompleted = false;
                    FocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
                    break;

                case "Completed":
                    FocusLotNumber = false;
                    FocusWorkProcess = false;
                    FocusWorker = false;
                    FocusStartTime = false;
                    FocusEndTime = false;
                    FocusAmount = false;
                    FocusCompleted = true;
                    FocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
                    break;

                case "Sales":
                    FocusLotNumber = false;
                    FocusWorkProcess = false;
                    FocusWorker = false;
                    FocusStartTime = false;
                    FocusEndTime = false;
                    FocusAmount = false;
                    FocusCompleted = false;
                    FocusSales = true;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
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
                    DisplayLot(LotNumber);
                    break;

                case "StartTime":
                    IsConvertTime = true;
                    StartTime = StartTime.Replace(":", "");
                    break;

                case "EndTime":
                    IsConvertTime = true;
                    EndTime = EndTime.Replace(":", "");
                    break;
            }
        }

        //フォーカス設定
        private void SetFocus()
        {
            if (string.IsNullOrEmpty(LotNumber)) { SetGotFocus("LotNumber"); return; }
            if (string.IsNullOrEmpty(WorkProcess)) { SetGotFocus("WorkProcess"); return; }
            SetGotFocus("LotNumber");
        }

        //現在の日付設定
        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsRegist) { ManufactureDate = SetToDay(DateTime.Now); }
        }

        //QRコード処理
        public void SetBarcode()
        {
            if (!CONVERT.IsLotNumber(ReceivedData)) { return; }
            LotNumber = ReceivedData.StringLeft(10);
            LotNumberSEQ = ReceivedData.StringRight(ReceivedData.Length - 11);
            DisplayLot(LotNumber);
            SetGotFocus("WorkProcess");
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

        //キーイベント
        public async void KeyDown(object value)
        {
            var result = false;
            switch (value)
            {
                case "WorkStart":
                    //作業開始
                    if (await IsRequiredRegist())
                    {
                        CtrlMessage = new ControlMessage("作業を開始します。", "※「はい」ボタンを押して作業を開始します。", "警告");
                        result = (bool)await DialogHost.Show(CtrlMessage);
                        await System.Threading.Tasks.Task.Delay(100);
                        if (result)
                        {
                            //ボタン処理
                            Mode = "作業中";
                            SetGotFocus("Amount");
                            StartTime = DateTime.Now.ToString("HH:mm");
                        }
                    }
                    break;

                case "WorkEnd":
                    //作業終了処理
                    EndTime = DateTime.Now.ToString("HH:mm");
                    CtrlMessage = new ControlMessage("作業を完了します。", "※登録後、次の作業の準備をしてください。", "警告");
                    result = (bool)await DialogHost.Show(CtrlMessage);
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        RegistData();
                        Mode = "準備";
                    }
                    else
                    {
                        EndTime = string.Empty;
                        WorkTime = string.Empty;
                    }
                    break;

                case "WorkBreak":
                    //中断処理・再開処理
                    Mode = (Mode == "中断") ? "作業中" : "中断";
                    break;

                case "Cancel":
                    //取消
                    CtrlMessage = new ControlMessage("この作業を取消します。", "※入力されたものが消去されます", "警告");
                    result = (bool)await DialogHost.Show(CtrlMessage);
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        Initialize();
                        Mode = "準備";
                    }
                    break;

                case "Regist":
                    CtrlMessage = new ControlMessage("作業データを" + ButtonName.Replace("　", "") + "します。", "※「はい」ボタンを押して作業データを" + ButtonName.Replace("　", "") + "します。", "警告");
                    result = (bool)await DialogHost.Show(CtrlMessage);
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        RegistData();
                        SetGotFocus("LotNumber");
                    }
                    break;

                case "Delete":
                    CtrlMessage = new ControlMessage("作業データを削除します", "※削除されたデータは復元できません", "警告");
                    result = (bool)await DialogHost.Show(CtrlMessage);
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        DeleteDate();
                        DisplayFramePage(new ManufactureList());
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
                    //テンキー処理
                    DisplayText(value);
                    break;

                case "Completed":
                    //完了
                    Completed = Completed == "E" ? "" : "E";
                    break;

                case "Sales":
                    //売上
                    Sales = Sales == "*" ? "" : "*";
                    break;

                case "DisplayInfo":
                    //加工登録画面
                    Initialize();
                    SetFocus();
                    break;

                case "DisplayList":
                    //加工一覧画面
                    DisplayFramePage(new ManufactureList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    DisplayFramePage(new PlanList());
                    break;
            }
        }
    }
}
