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
    public class ViewModelManufactureInfo : Common, IKeyDown, ITenKey, IWorker, IWorkProcess, ITimer
    {
        //変数
        ViewModelWindowMain windowMain;
        ViewModelControlTenKey controlTenKey;
        ViewModelControlWorker controlWorker;
        ViewModelControlWorkProcess controlWorkProcess;
        string status;
        string processName;
        string manufactureCODE;
        string lotNumber;
        string equipmentCODE;
        string manufactureDate;
        string equipment1;
        string equipment2;
        string team;
        string buttonName;
        string productName;
        int lotNumberLength = 10;
        int startTimeLength = 4;
        int endTimeLength = 4;
        int amountLength = 5;
        string breakName;
        bool isRegist;
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
        bool isEnable;
        bool isFocusLotNumber;
        bool isFocusWorker;
        bool isFocusWorkProcess;
        bool isFocusStartTime;
        bool isFocusEndTime;
        bool isFocusAmount;
        bool isFocusCompleted;
        bool isFocusSales;

        //プロパティ
        public string Status                    //入力状況
        {
            get { return status; }
            set
            {
                SetProperty(ref status, value);
                SetStatus();
            }
        }
        public string ProcessName               //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);
                process = new ProcessCategory(value);
                
                switch (value)
                {
                    case "検査":
                        VisibleSeal = false;
                        VisiblePackaging = true;
                        if (IsRegist) { manufacture.WorkProcess = "検査"; }
                        break;

                    case "梱包":
                        VisibleSeal = false;
                        VisiblePackaging = false;
                        if (IsRegist) { manufacture.WorkProcess = "梱包"; }
                        break;

                    default:
                        VisibleSeal = true;
                        VisiblePackaging = true;
                        if (IsRegist) { manufacture.WorkProcess = ""; }
                        break;
                }
            }
        }
        public string ManufactureCODE           //製造CODE
        {
            get { return manufactureCODE; }
            set 
            { 
                SetProperty(ref manufactureCODE, value);
                CopyProperty(new Manufacture(ManufactureCODE, ProcessName), manufacture);
                DisplayLot(manufacture.LotNumber);
            }
        }
        public string LotNumber                 //ロット番号（テキストボックス）
        {
            get { return lotNumber; }
            set { SetProperty(ref lotNumber, value); }
        }
        public string EquipmentCODE             //設備CODE
        {
            get { return equipmentCODE; }
            set 
            {
                equipment = new Equipment(value);
                var name = equipment.EquipmentName;
                windowMain.ProcessWork = string.IsNullOrEmpty(name) ? process.Name + "実績" : name + " - " + value;
                Equipment1 = value;
            }
        }
        public string Equipment1                //設備
        {
            get { return equipment1; }
            set
            {
                SetProperty(ref equipment1, value);
                manufacture.Equipment1 = value;
            }
        }
        public string Equipment2                //設備
        {
            get { return equipment2; }
            set 
            { 
                SetProperty(ref equipment2, value);
                manufacture.Equipment2 = value;
            }
        }
        public string Team                      //班名
        {
            get { return team; }
            set 
            { 
                SetProperty(ref team, value);
                manufacture.Team = value;
            }
        }
        public string ButtonName                //登録ボタン名
        {
            get { return buttonName; }
            set { SetProperty(ref buttonName, value); }
        }
        public int LotNumberLength              //文字数（ロット番号）
        {
            get { return lotNumberLength; }
            set { SetProperty(ref lotNumberLength, value); }
        }
        public int StartTimeLength              //文字数（開始時間）
        {
            get { return startTimeLength; }
            set { SetProperty(ref startTimeLength, value); }
        }
        public int EndTimeLength                //文字数（終了時間）
        {
            get { return endTimeLength; }
            set { SetProperty(ref endTimeLength, value); }
        }
        public int AmountLength                 //文字数（数量）
        {
            get { return amountLength; }
            set { SetProperty(ref amountLength, value); }
        }
        public string BreakName                 //中断ボタン名
        {
            get { return breakName; }
            set { SetProperty(ref breakName, value); }
        }
        public bool EnabledControl1             //コントロール使用可能
        {
            get { return enabledControl1; }
            set
            {
                SetProperty(ref enabledControl1, value);
                VisibleButtonStart = value;
            }
        }
        public bool EnabledControl2             //コントロール使用可能
        {
            get { return enabledControl2; }
            set { SetProperty(ref enabledControl2, value); }
        }
        public bool VisiblePackaging            //表示・非表示（数量)
        {
            get { return visiblePackaging; }
            set { SetProperty(ref visiblePackaging, value); }
        }
        public bool VisibleButtonStart          //開始ボタン表示・非表示
        {
            get { return visibleButtonStart; }
            set { SetProperty(ref visibleButtonStart, value); }
        }
        public bool VisibleButtonEnd            //終了ボタン表示・非表示
        {
            get { return visibleButtonEnd; }
            set { SetProperty(ref visibleButtonEnd, value); }
        }
        public bool VisibleButtonCancel         //取消ボタン表示・非表示
        {
            get { return visibleButtonCancel; }
            set { SetProperty(ref visibleButtonCancel, value); }
        }
        public bool VisibleButtonBreak          //中断ボタン表示・非表示
        {
            get { return visibleButtonBreak; }
            set { SetProperty(ref visibleButtonBreak, value); }
        }
        public bool VisibleEdit                 //表示・非表示（削除ボタン）
        {
            get { return visibleEdit; }
            set { SetProperty(ref visibleEdit, value); }
        }
        public bool VisibleTenKey               //表示・非表示（テンキー）
        {
            get { return visibleTenKey; }
            set { SetProperty(ref visibleTenKey, value); }
        }
        public bool VisibleWorker               //表示・非表示（作業者）
        {
            get { return visibleWorker; }
            set { SetProperty(ref visibleWorker, value); }
        }
        public bool VisibleWorkProcess          //表示・非表示（工程）
        {
            get { return visibleWorkProcess; }
            set { SetProperty(ref visibleWorkProcess, value); }
        }
        public bool VisibleSeal                 //表示・非表示（売上）
        {
            get { return visibleSeal; }
            set { SetProperty(ref visibleSeal, value); }
        }
        public bool IsRegist                    //新規・既存フラグ（true:新規、false:既存）
        {
            get { return isRegist; }
            set
            {
                SetProperty(ref isRegist, value);
                EnabledControl1 = !value;
                EnabledControl2 = !value;
                VisibleButtonStart = value;
                ButtonName = value ? "登　録" : "修　正";
            }
        }
        public bool IsEnable                    //表示・非表示（下部ボタン）
        {
            get { return isEnable; }
            set { SetProperty(ref isEnable, value); }
        }
        public bool IsFocusLotNumber            //フォーカス（ロット番号）
        {
            get { return isFocusLotNumber; }
            set { SetProperty(ref isFocusLotNumber, value); }
        }
        public bool IsFocusWorker               //フォーカス（作業者）
        {
            get { return isFocusWorker; }
            set { SetProperty(ref isFocusWorker, value); }
        }
        public bool IsFocusWorkProcess          //フォーカス（工程）
        {
            get { return isFocusWorkProcess; }
            set { SetProperty(ref isFocusWorkProcess, value); }
        }
        public bool IsFocusStartTime            //フォーカス（開始時間）
        {
            get { return isFocusStartTime; }
            set { SetProperty(ref isFocusStartTime, value); }
        }
        public bool IsFocusEndTime              //フォーカス（終了時間）
        {
            get { return isFocusEndTime; }
            set { SetProperty(ref isFocusEndTime, value); }
        }
        public bool IsFocusAmount               //フォーカス（数量）
        {
            get { return isFocusAmount; }
            set { SetProperty(ref isFocusAmount, value); }
        }
        public bool IsFocusCompleted            //フォーカス（完了）
        {
            get { return isFocusCompleted; }
            set { SetProperty(ref isFocusCompleted, value); }
        }
        public bool IsFocusSales                //フォーカス（売上）
        {
            get { return isFocusSales; }
            set { SetProperty(ref isFocusSales, value); }
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
            manufacture = new Manufacture();
            management = new Management();

            //データ取得
            Initialize();
            ManufactureCODE = code;
            if (string.IsNullOrEmpty(code)) { LotNumber = number; DisplayLot(LotNumber); }

            //デフォルト値設定
            IsRegist = string.IsNullOrEmpty(manufacture.ManufactureCODE);
            IsEnable = DATETIME.ToStringDate(manufacture.ManufactureDate) < SetVerificationDay(DateTime.Now) ? false : true;
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
            controlWorkProcess = ViewModelControlWorkProcess.Instance;

            windowMain.Ikeydown = this;
            windowMain.Itimer = this;
            controlTenKey.Itenkey = this;
            controlWorker.Iworker = this;
            controlWorkProcess.IworkProcess = this;
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
            windowMain.ProcessName = ProcessName;
            EquipmentCODE = IniFile.GetString("Page", "Equipment");
            Status = IniFile.GetString("Manufacture", "Mode");
        }

        //初期化
        public void Initialize()
        {
            ProcessName = IniFile.GetString("Page", "Process");
            ManufactureCODE = string.Empty;
            LotNumber = string.Empty;

            manufacture.ManufactureDate = SetToDay(DateTime.Now);
            manufacture.Worker = IniFile.GetString("Page", "Worker");
            manufacture.LotNumber = string.Empty;
            manufacture.ProductName = string.Empty;
            manufacture.WorkProcess = string.Empty;
            manufacture.StartTime = string.Empty;
            manufacture.EndTime = string.Empty;
            manufacture.WorkTime = string.Empty;
            manufacture.Amount = string.Empty;
            manufacture.Weight = string.Empty;
            manufacture.Completed = string.Empty;
            manufacture.Sales = string.Empty;
            IsRegist = true;
            IsEnable = true;
        }

        //ロット番号処理
        private void DisplayLot(string lotnumber)
        {
            //データ取得
            CopyProperty(new Management(management.GetLotNumber(lotnumber), ProcessName), management);

            //データ表示
            if (!string.IsNullOrEmpty(management.ProductName) && management.ProductName != manufacture.ProductName) { Sound.PlayAsync(SoundFolder + CONST.SOUND_LOT); }
            shape = new ProductShape(management.ShapeName);
            LotNumber = management.LotNumber;
            manufacture.LotNumber = LotNumber;
            manufacture.ProductName = management.ProductName;
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus)
            {
                case "Worker":
                    manufacture.Worker = value.ToString();
                    break;

                case "WorkProcess":
                    manufacture.WorkProcess = value.ToString();
                    break;
            }
            SetNextFocus();
        }

        //状態により設定する
        private void SetStatus()
        {
            switch (Status)
            {
                case "登録":
                    EnabledControl1 = true;
                    EnabledControl2 = true;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;
                    VisibleEdit = true;
                    windowMain.VisibleList = true;
                    windowMain.VisibleDefect = false;
                    windowMain.VisibleArrow = false;
                    SetGotFocus("LotNumber");
                    break;

                case "準備":
                    EnabledControl1 = true;
                    EnabledControl2 = false;
                    VisibleButtonStart = true;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;
                    windowMain.VisibleList = true;
                    windowMain.VisibleDefect = false;
                    windowMain.VisibleArrow = false;
                    break;

                case "作業中":
                    EnabledControl1 = false;
                    EnabledControl2 = true;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = true;
                    VisibleButtonBreak = true;
                    VisibleButtonCancel = false;
                    BreakName = "中　断";
                    windowMain.VisibleList = true;
                    windowMain.VisibleDefect = false;
                    SetGotFocus("Amount");

                    //一覧からデータ読み込み
                    manufacture.ManufactureCODE = ManufactureCODE;
                    break;

                case "中断":
                    EnabledControl1 = false;
                    EnabledControl2 = false;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = true;
                    VisibleButtonCancel = true;
                    BreakName = "再　開";
                    windowMain.VisibleList = true;
                    windowMain.VisibleDefect = false;
                    break;

                case "編集":
                    EnabledControl1 = true;
                    EnabledControl2 = true;
                    VisibleEdit = true;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;
                    windowMain.VisibleList = true;
                    windowMain.VisibleDefect = false;
                    break;

                default:
                    Status = "準備";
                    EnabledControl1 = true;
                    EnabledControl2 = false;
                    VisibleButtonStart = true;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;
                    windowMain.VisibleList = true;
                    windowMain.VisibleDefect = false;
                    windowMain.VisibleArrow = false;
                    break;
            }

            //ボタン設定
            windowMain.VisiblePower = true;
            windowMain.VisibleInfo = true;
            windowMain.VisibleArrow = false;
        }

        //登録処理
        private void RegistData()
        {
            //コード確定
            var mark = process.Mark;
            if (IsRegist)
            {
                var date = STRING.ToDateDB(manufacture.ManufactureDate);
                var code = manufacture.GenerateCode(mark + date);
                manufacture.ManufactureCODE = code;
            }

            //登録処理
            manufacture.LotNumber = LotNumber;
            manufacture.InsertLog(IsRegist);
            manufacture.Resist(manufacture.ManufactureCODE);

            Initialize();
            Status = IniFile.GetString("Manufacture", "Mode");
        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(manufacture.Worker))
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
                    if (string.IsNullOrEmpty(manufacture.WorkProcess))
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
                var messege = (bool)await DialogHost.Show(new ControlMessage(messege1, messege2, messege3));
                await System.Threading.Tasks.Task.Delay(100);
                if (messege) { SetGotFocus(focus); }
            }
            return result;
        }

        //削除処理
        private void DeleteDate()
        {
            manufacture.DeleteLog();
            manufacture.Delete(manufacture.ManufactureCODE);        //製造実績削除

            Initialize();
            Status = IniFile.GetString("Manufacture", "Mode");
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
                        result = (bool)await DialogHost.Show(new ControlMessage("作業を開始します。", "※「はい」ボタンを押して作業を開始します。", "警告"));
                        await System.Threading.Tasks.Task.Delay(100);
                        if (result)
                        {
                            //ボタン処理
                            Status = "作業中";
                            SetGotFocus("Amount");
                            manufacture.StartTime = DateTime.Now.ToString("HH:mm");
                        }
                    }
                    break;

                case "WorkEnd":
                    //作業終了処理
                    manufacture.EndTime = DateTime.Now.ToString("HH:mm");
                    result = (bool)await DialogHost.Show(new ControlMessage("作業を完了します。", "※登録後、次の作業の準備をしてください。", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        RegistData();
                        Status = "準備";
                    }
                    else
                    {
                        manufacture.EndTime = string.Empty;
                        manufacture.WorkTime = string.Empty;
                    }
                    break;

                case "WorkBreak":
                    //中断処理・再開処理
                    Status = (Status == "中断") ? "作業中" : "中断";
                    break;

                case "Cancel":
                    //取消
                    result = (bool)await DialogHost.Show(new ControlMessage("この作業を取消します。", "※入力されたものが消去されます", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        Initialize();
                        Status = "準備";
                    }
                    break;

                case "Regist":
                    result = (bool)await DialogHost.Show(new ControlMessage("作業データを" + ButtonName.Replace("　", "") +"します。", "※「はい」ボタンを押して作業データを" + ButtonName.Replace("　", "") + "します。", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        RegistData();
                        SetGotFocus("LotNumber");
                    }
                    break;

                case "Delete":
                    result = (bool)await DialogHost.Show(new ControlMessage("作業データを削除します", "※削除されたデータは復元できません", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        DeleteDate();
                        windowMain.FramePage = new ManufactureList();
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
                    manufacture.Completed = manufacture.Completed == "E" ? "" : "E";
                    break;

                case "Sales":
                    //売上
                    manufacture.Sales = manufacture.Sales == "*" ? "" : "*";
                    break;

                case "DisplayInfo":
                    //加工登録画面
                    Initialize();
                    Status = IniFile.GetString("Manufacture", "Mode");
                    SetFocus();
                    break;

                case "DisplayList":
                    //加工一覧画面
                    windowMain.FramePage = new ManufactureList();
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
                    if (LotNumber.Length < LotNumberLength) { LotNumber += value.ToString(); }
                    break;

                case "StartTime":
                    if (manufacture.StartTime.Length > StartTimeLength) { return; }
                    manufacture.IsConvertTime = !(manufacture.StartTime.Length < 3);
                    manufacture.StartTime += value.ToString();
                    manufacture.IsConvertTime = true;
                    if (manufacture.StartTime.Length > StartTimeLength - 1) { SetNextFocus(); }
                    break;

                case "EndTime":
                    if (manufacture.EndTime.Length > EndTimeLength) { return; }
                    manufacture.IsConvertTime = !(manufacture.EndTime.Length < 3);
                    manufacture.EndTime += value.ToString();
                    manufacture.IsConvertTime = true;
                    if (manufacture.EndTime.Length > EndTimeLength - 1) { SetNextFocus(); }
                    break;

                case "Amount":
                    if (manufacture.Amount.Length < AmountLength) { manufacture.Amount += value.ToString(); }
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
                    manufacture.StartTime = string.Empty;
                    break;

                case "EndTime":
                    manufacture.EndTime = string.Empty;
                    break;

                case "Amount":
                    manufacture.Amount = string.Empty;
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
                    if (manufacture.StartTime.Length == 0) { return; }
                    manufacture.IsConvertTime = false;
                    manufacture.StartTime = manufacture.StartTime[..^1];
                    break;

                case "EndTime":
                    if (manufacture.EndTime.Length == 0) { return; }
                    manufacture.IsConvertTime = false;
                    manufacture.EndTime = manufacture.EndTime[..^1];
                    break;

                case "Amount":
                    if (manufacture.Amount.Length > 0) { manufacture.Amount = manufacture.Amount[..^1]; }
                    break;

                default:
                    break;
            }
        }

        //確定処理
        private void SetNextFocus()
        {
            switch(Status)
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
            Focus = value;
            switch (Focus)
            {
                case "LotNumber":
                    IsFocusLotNumber = true;
                    IsFocusWorkProcess = false;
                    IsFocusWorker = false;
                    IsFocusStartTime = false;
                    IsFocusEndTime = false;
                    IsFocusAmount = false;
                    IsFocusCompleted = false;
                    IsFocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = "-";
                    break;

                case "WorkProcess":
                    IsFocusLotNumber = false;
                    IsFocusWorkProcess = true;
                    IsFocusWorker = false;
                    IsFocusStartTime = false;
                    IsFocusEndTime = false;
                    IsFocusAmount = false;
                    IsFocusCompleted = false;
                    IsFocusSales = false;
                    VisibleTenKey = false;
                    VisibleWorker = false;
                    VisibleWorkProcess = true;
                    break;

                case "Worker":
                    IsFocusLotNumber = false;
                    IsFocusWorkProcess = false;
                    IsFocusWorker = true;
                    IsFocusStartTime = false;
                    IsFocusEndTime = false;
                    IsFocusAmount = false;
                    IsFocusCompleted = false;
                    IsFocusSales = false;
                    VisibleTenKey = false;
                    VisibleWorker = true;
                    VisibleWorkProcess = false;
                    break;

                case "StartTime":
                    IsFocusLotNumber = false;
                    IsFocusWorkProcess = false;
                    IsFocusWorker = false;
                    IsFocusStartTime = true;
                    IsFocusEndTime = false;
                    IsFocusAmount = false;
                    IsFocusCompleted = false;
                    IsFocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
                    break;

                case "EndTime":
                    IsFocusLotNumber = false;
                    IsFocusWorkProcess = false;
                    IsFocusWorker = false;
                    IsFocusStartTime = false;
                    IsFocusEndTime = true;
                    IsFocusAmount = false;
                    IsFocusCompleted = false;
                    IsFocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
                    break;

                case "Amount":
                    IsFocusLotNumber = false;
                    IsFocusWorkProcess = false;
                    IsFocusWorker = false;
                    IsFocusStartTime = false;
                    IsFocusEndTime = false;
                    IsFocusAmount = true;
                    IsFocusCompleted = false;
                    IsFocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
                    break;

                case "Completed":
                    IsFocusLotNumber = false;
                    IsFocusWorkProcess = false;
                    IsFocusWorker = false;
                    IsFocusStartTime = false;
                    IsFocusEndTime = false;
                    IsFocusAmount = false;
                    IsFocusCompleted = true;
                    IsFocusSales = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    controlTenKey.InputString = ".";
                    break;

                case "Sales":
                    IsFocusLotNumber = false;
                    IsFocusWorkProcess = false;
                    IsFocusWorker = false;
                    IsFocusStartTime = false;
                    IsFocusEndTime = false;
                    IsFocusAmount = false;
                    IsFocusCompleted = false;
                    IsFocusSales = true;
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
                    manufacture.IsConvertTime = true;
                    manufacture.StartTime = manufacture.StartTime.Replace(":","");
                    break;

                case "EndTime":
                    manufacture.IsConvertTime = true;
                    manufacture.EndTime = manufacture.EndTime.Replace(":", "");
                    break;
            }
        }

        //フォーカス設定
        private void SetFocus()
        {
            if (string.IsNullOrEmpty(LotNumber)) { SetGotFocus("LotNumber"); return; }
            if (string.IsNullOrEmpty(manufacture.WorkProcess)) { SetGotFocus("WorkProcess"); return; }
            SetGotFocus("LotNumber");
        }

        //現在の日付設定
        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsRegist) { manufacture.ManufactureDate = SetToDay(DateTime.Now); }
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
