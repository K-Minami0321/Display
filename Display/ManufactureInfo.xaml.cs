using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class ManufactureInfo : Page
    {
        public static ManufactureInfo Instance
        { get; set; }
        public ManufactureInfo()
        {
            Instance = this;
            DataContext = ViewModelManufactureInfo.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelManufactureInfo : Common, IKeyDown, ITenKey, IWorker, IWorkProcess
    {
        //プロパティ変数
        string _EquipmentCODE;
        DataTable _DefectList;
        bool _EditFlg;
        bool _RegFlg;
        string _Status;
        string _ProcessName;
        string _ManufactureCODE;
        string _ManufactureDate;
        string _Equipment1;
        string _Equipment2;
        string _Team;
        string _LotNumber;
        string _ProductName;
        string _StartTime;
        string _EndTime;
        bool _EnabledControl1;
        bool _EnabledControl2;
        int _LotNumberLength = 10;
        int _AmountLength = 5;
        string _BreakName;
        bool _VisibleTime;
        bool _VisiblePackaging;
        bool _VisibleButton;
        bool _VisibleButtonStart;
        bool _VisibleButtonEnd;
        bool _VisibleButtonCancel;
        bool _VisibleButtonBreak;
        bool _VisibleEdit;
        bool _VisibleTenKey;
        bool _VisibleWorker;
        bool _VisibleWorkProcess;

        //プロパティ
        public static ViewModelManufactureInfo Instance     //インスタンス
        { get; set; } = new ViewModelManufactureInfo();
        public string EquipmentCODE                         //設備CODE
        {
            get { return _EquipmentCODE; }
            set 
            { 
                 _EquipmentCODE = value;

                equipment.EquipmentCODE = value;
                equipment.Select();

                var name = equipment.EquipmentName;
                if (!string.IsNullOrEmpty(equipment.EquipmentName))
                {
                    name = name + " - " + EquipmentCODE;
                    Team = equipment.Team;
                }
                else
                {
                    if (iProcess == null) { return; }
                    name = iProcess.Name;
                }
                ViewModelWindowMain.Instance.ProcessWork = name;
                Equipment1 = value;
            }
        }
        public DataTable DefectList                         //不良データ
        {
            get { return _DefectList; }
            set { SetProperty(ref _DefectList, value); }
        }
        public bool EditFlg                                 //編集中フラグ
        {
            get { return _EditFlg; }
            set { SetProperty(ref _EditFlg, value); }
        }
        public bool RegFlg                                  //新規・既存フラグ(true:新規、false:既存)
        {
            get { return _RegFlg; }
            set 
            { 
                SetProperty(ref _RegFlg, value);
                if (!value) { DisplayData(); }
                VisibleEdit = !value;
            }
        }
        public string Status                                //入力状況
        {
            get { return _Status; }
            set 
            { 
                SetProperty(ref _Status, value);
                SetStatus();
                EditFlg = Status == "作業中" ? true : false;
            }
        }
        public string ProcessName                           //工程区分
        {
            get { return _ProcessName; }
            set 
            { 
                SetProperty(ref _ProcessName, value);
                iProcess = ProcessCategory.SetProcess(value);
                switch (value)
                {
                    case "合板":
                    case "プレス":
                    case "仕上":
                        VisiblePackaging = true;
                        break;

                    default:
                        VisiblePackaging = false;
                        break;
                }
            }
        }
        public string ManufactureCODE                       //製造CODE
        {
            get { return manufacture.ManufactureCODE; }
            set 
            { 
                SetProperty(ref _ManufactureCODE, value);
                manufacture.ManufactureCODE = value;
            }
        }
        public string ManufactureDate                       //作業日
        {
            get { return manufacture.ManufactureDate; }
            set 
            { 
                SetProperty(ref _ManufactureDate, value);
                manufacture.ManufactureDate = value;
                VisibleButton = value != STRING.ToDate(SetToDay(DateTime.Now)) ? false : true;
            }
        }
        public string Equipment1                            //設備
        {
            get { return manufacture.Equipment1; }
            set
            {
                SetProperty(ref _Equipment1, value);
                manufacture.Equipment1 = value;
            }
        }
        public string Equipment2                            //設備
        {
            get { return manufacture.Equipment2; }
            set 
            { 
                SetProperty(ref _Equipment2, value);
                manufacture.Equipment2 = value;
            }
        }
        public string Team                                  //班名
        {
            get { return manufacture.Team; }
            set 
            { 
                SetProperty(ref _Team, value);
                manufacture.Team = value;
            }
        }
        public string LotNumber                             //ロット番号
        {
            get { return manufacture.LotNumber; }
            set 
            { 
                SetProperty(ref _LotNumber, value);
                manufacture.LotNumber = value;
            }
        }
        public string ProductName                           //品番
        {
            get { return manufacture.ProductName; }
            set
            {
                SetProperty(ref _ProductName, value);
                manufacture.ProductName = value;
            }
        }
        public string StartTime                             //開始時間
        {
            get { return manufacture.StartTime; }
            set 
            { 
                SetProperty(ref _StartTime, value);
                manufacture.StartTime = value;
                VisibleTime = !string.IsNullOrEmpty(value);
            }
        }
        public string EndTime                               //終了時間
        {
            get { return manufacture.EndTime; }
            set 
            { 
                SetProperty(ref _EndTime, value);
                manufacture.EndTime = value;
                manufacture.WorkTime = manufacture.CalculateWorkingTime(ManufactureDate, StartTime, value);
            }
        }
        public bool EnabledControl1                         //コントロール使用可能
        {
            get { return _EnabledControl1; }
            set { SetProperty(ref _EnabledControl1, value); }
        }
        public bool EnabledControl2                         //コントロール使用可能
        {
            get { return _EnabledControl2; }
            set { SetProperty(ref _EnabledControl2, value); }
        }
        public int LotNumberLength                          //文字数（ロット番号）
        {
            get { return _LotNumberLength; }
            set { SetProperty(ref _LotNumberLength, value); }
        }
        public int AmountLength                             //文字数（数量）
        {
            get { return _AmountLength; }
            set { SetProperty(ref _AmountLength, value); }
        }
        public string BreakName                             //中断ボタン名
        {
            get { return _BreakName; }
            set { SetProperty(ref _BreakName, value); }
        }
        public bool VisibleTime                             //表示・非表示（時間の区切り）
        {
            get { return _VisibleTime; }
            set { SetProperty(ref _VisibleTime, value); }
        }
        public bool VisiblePackaging                        //表示・非表示（数量)
        {
            get { return _VisiblePackaging; }
            set { SetProperty(ref _VisiblePackaging, value); }
        }
        public bool VisibleButton                           //表示・非表示（下部ボタン）
        {
            get { return _VisibleButton; }
            set { SetProperty(ref _VisibleButton, value); }
        }
        public bool VisibleButtonStart                      //開始ボタン表示・非表示
        {
            get { return _VisibleButtonStart; }
            set { SetProperty(ref _VisibleButtonStart, value); }
        }
        public bool VisibleButtonEnd                        //終了ボタン表示・非表示
        {
            get { return _VisibleButtonEnd; }
            set { SetProperty(ref _VisibleButtonEnd, value); }
        }
        public bool VisibleButtonCancel                     //取消ボタン表示・非表示
        {
            get { return _VisibleButtonCancel; }
            set { SetProperty(ref _VisibleButtonCancel, value); }
        }
        public bool VisibleButtonBreak                      //中断ボタン表示・非表示
        {
            get { return _VisibleButtonBreak; }
            set { SetProperty(ref _VisibleButtonBreak, value); }
        }
        public bool VisibleEdit                             //表示・非表示（削除ボタン）
        {
            get { return _VisibleEdit; }
            set { SetProperty(ref _VisibleEdit, value); }
        }
        public bool VisibleTenKey                           //表示・非表示（テンキー）
        {
            get { return _VisibleTenKey; }
            set { SetProperty(ref _VisibleTenKey, value); }
        }
        public bool VisibleWorker                           //表示・非表示（作業者）
        {
            get { return _VisibleWorker; }
            set { SetProperty(ref _VisibleWorker, value); }
        }
        public bool VisibleWorkProcess                      //表示・非表示（工程）
        {
            get { return _VisibleWorkProcess; }
            set { SetProperty(ref _VisibleWorkProcess, value); }
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
        internal ViewModelManufactureInfo()
        {
            manufacture = new Manufacture();
            management = new Management();
            equipment = new Equipment();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            ViewModelControlTenKey.Instance.Itenkey = this;
            ViewModelControlWorker.Instance.Iworker = this;
            ViewModelControlWorkProcess.Instance.IworkProcess = this;
            ViewModelWindowMain.Instance.ProcessName = INI.GetString("Page", "Process");
            DisplayCapution();
            SetGotFocus("LotNumber");
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {          
            //キャプション
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            EquipmentCODE = INI.GetString("Page", "Equipment");
            

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();

            //一覧からデータ読み込み（修正）
            ManufactureCODE = ViewModelManufactureList.Instance.ManufactureCODE;
            DefectList = ViewModelDefectInfo.Instance.DefectList;
            RegFlg = string.IsNullOrEmpty(ManufactureCODE);

            if (RegFlg)
            {
                //予定表からロット番号取得
                LotNumber = ViewModelPlanList.Instance.LotNumber;     //データ取得
                if (LotNumber == null) { LotNumber = string.Empty; }
                DisplayLotNumber(LotNumber);
                SetGotFocus("Worker");
            }
            if (string.IsNullOrEmpty(ProductName)) { SetGotFocus("LotNumber"); }

        }

        //初期化
        public void Initialize()
        {
            ManufactureDate = SetToDay(DateTime.Now);
            ManufactureCODE = string.Empty;
            ProductName = string.Empty;
            LotNumber = string.Empty;
            Equipment1 = string.Empty;
            Equipment2 = string.Empty;
            manufacture.WorkProcess = string.Empty;
            manufacture.Worker = INI.GetString("Page", "Worker");
            StartTime = string.Empty;
            EndTime = string.Empty;
            manufacture.WorkTime = string.Empty;
            manufacture.Amount = string.Empty;
            manufacture.Weight = string.Empty;
            manufacture.IsCompleted = false;
            manufacture.IsSales = false;
            manufacture.IsOutsourced = false;
            AmountLabel = "数 量";
        }

        //データ表示
        private void DisplayData()
        {
            manufacture.Select(ManufactureCODE);
            DisplayLotNumber(LotNumber);        //ロット情報の取得
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
                            StartTime = DateTime.Now.ToString("HH:mm");
                        }
                    }
                    break;

                case "WorkEnd":
                    //作業終了処理
                    EndTime = DateTime.Now.ToString("HH:mm");
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
                        EndTime = string.Empty;
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
                    if (result) { 
                        Initialize(); 
                        Status = "準備"; 
                    }
                    break;

                case "Regist":
                    result = (bool)await DialogHost.Show(new ControlMessage("作業データを修正します。", "※「はい」ボタンを押して作業データを修正します。", "警告"));
                    if (result) 
                    { 
                        RegistData();
                        ViewModelWindowMain.Instance.FramePage.Navigate(new ManufactureList());
                    }
                    break;

                case "Delete":
                    result = (bool)await DialogHost.Show(new ControlMessage("作業データを削除します", "※削除されたデータは復元できません", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result) 
                    { 
                        DeleteDate();
                        ViewModelWindowMain.Instance.FramePage.Navigate(new ManufactureList());
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

                case "1": case "2": case "3": case "4": case "5": case "6": case "7": case "8": case "9": case "0": case "-":
                    //テンキー処理
                    DisplayText(value);
                    break;

                case "Completed":
                    //完了
                    manufacture.IsCompleted = !manufacture.IsCompleted;
                    break;

                case "Sales":
                    //売上
                    manufacture.IsSales = !manufacture.IsSales;
                    break;

                case "DefectList":
                    //不良一覧
                    VisibleTenKey = false;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    break;

                case "DisplayInfo":
                    //加工登録画面
                    ViewModelWindowMain.Instance.FramePage.Navigate(new ManufactureInfo());
                    break;

                case "DisplayList":
                    //加工一覧画面

                    //編集モードの場合はデータクリア
                    if (!string.IsNullOrEmpty(ManufactureCODE)) { Initialize(); }

                    ViewModelWindowMain.Instance.FramePage.Navigate(new ManufactureList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.FramePage.Navigate(new PlanList());
                    break;

                case "DefectInfo":
                    //不良登録画面
                    ViewModelWindowMain.Instance.FramePage.Navigate(new DefectInfo());
                    break;
            }
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
            //SetNextFocus();
        }

        //状態により設定する
        private void SetStatus()
        {
            switch (Status)
            {
                case "準備":
                    EnabledControl1 = true;
                    EnabledControl2 = false;
                    VisibleButtonStart = true;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;

                    ViewModelWindowMain.Instance.VisibleList = true;
                    ViewModelWindowMain.Instance.VisibleDefect = false;
                    ViewModelWindowMain.Instance.VisibleArrow = false;
                    SetGotFocus("LotNumber");
                    break;

                case "作業中":
                    EnabledControl1 = false;
                    EnabledControl2 = true;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = true;
                    VisibleButtonBreak = true;
                    VisibleButtonCancel = false;
                    BreakName = "中　断";

                    ViewModelWindowMain.Instance.VisibleList = true;
                    ViewModelWindowMain.Instance.VisibleDefect = false;
                    SetGotFocus("Amount");

                    //一覧からデータ読み込み
                    ManufactureCODE = ViewModelManufactureList.Instance.ManufactureCODE;
                    RegFlg = string.IsNullOrEmpty(ManufactureCODE);
                    break;

                case "中断":
                    EnabledControl1 = false;
                    EnabledControl2 = false;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = true;
                    VisibleButtonCancel = true;
                    BreakName = "再　開";

                    ViewModelWindowMain.Instance.VisibleList = true;
                    ViewModelWindowMain.Instance.VisibleDefect = false;

                    break;

                case "編集":
                    EnabledControl1 = true;
                    EnabledControl2 = true;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;

                    ViewModelWindowMain.Instance.VisibleList = true;
                    ViewModelWindowMain.Instance.VisibleDefect = false;

                    break;

                default:
                    Status = "準備";
                    EnabledControl1 = true;
                    EnabledControl2 = false;
                    VisibleButtonStart = true;
                    VisibleButtonEnd = false;
                    VisibleButtonBreak = false;
                    VisibleButtonCancel = false;

                    ViewModelWindowMain.Instance.VisibleList = true;
                    ViewModelWindowMain.Instance.VisibleDefect = false;
                    ViewModelWindowMain.Instance.VisibleArrow = false;
                    break;
            }

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleInfo = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
        }

        //登録処理
        private void RegistData()
        {
            //コード確定
            var mark = iProcess.Mark;
            if (RegFlg)
            {
                var date = STRING.ToDateDB(ManufactureDate);
                var code = manufacture.GenerateCode(mark + date);
                ManufactureCODE = code;
            }

            //登録処理
            manufacture.InsertLog(RegFlg);
            manufacture.Resist(ManufactureCODE);

            //不良データ登録
            if (DefectList != null)
            {






            }

            //初期設定
            ViewModelManufactureList.Instance.ManufactureCODE = string.Empty;
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
            //製造実績削除
            manufacture.DeleteLog();
            manufacture.Delete(ManufactureCODE);

            //製造不良削除
            //defect.Delete(ManufactureCODE);

            //処理完了
            ViewModelManufactureList.Instance.ManufactureCODE = string.Empty;
            Initialize();
        }

        //入力制御
        private void DisplayText(object value)
        {
            switch (Focus)
            {
                case "LotNumber":
                    //
                    if (LotNumber.Length < LotNumberLength)
                    {
                        LotNumber += value.ToString();
                        ManufactureInfo.Instance.LotNumber.Select(LotNumber.Length, 0);
                    }
                    break;

                case "Amount":
                    if (manufacture.Amount.Length < AmountLength)
                    {
                        manufacture.Amount += value.ToString();
                        ManufactureInfo.Instance.Amount.Select(manufacture.Amount.Length, 0);
                    }
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
                    ManufactureInfo.Instance.LotNumber.Select(LotNumber.Length, 0);
                    break;

                case "Amount":
                    manufacture.Amount = string.Empty;
                    ManufactureInfo.Instance.Amount.Select(manufacture.Amount.Length, 0);
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
                    if (LotNumber.Length > 0)
                    {
                        LotNumber = LotNumber[..^1];
                        ManufactureInfo.Instance.LotNumber.Select(LotNumber.Length, 0);
                    }
                    break;

                case "Amount":
                    if (manufacture.Amount.Length > 0)
                    {
                        manufacture.Amount = manufacture.Amount[..^1];
                        ManufactureInfo.Instance.Amount.Select(manufacture.Amount.Length, 0);
                    }
                    break;

                default:
                    break;
            }
        }

        //確定処理
        private void SetNextFocus()
        {
            if (EnabledControl1)
            {
                //作業開始前
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
            } 
            else 
            {
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
            }
        }

        //フォーカス処理（GotForcus）
        private void SetGotFocus(object value)
        {
            if (ManufactureInfo.Instance == null) { return; }

            Focus = value;
            switch (Focus)
            {
                case "LotNumber":
                    
                    ManufactureInfo.Instance.LotNumber.Focus();
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    ViewModelControlTenKey.Instance.InputString = "-";
                    if (LotNumber != null) { ManufactureInfo.Instance.LotNumber.Select(LotNumber.Length, 0); }
                    break;

                case "Amount":
                    ManufactureInfo.Instance.Amount.Focus();
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    VisibleWorkProcess = false;
                    ViewModelControlTenKey.Instance.InputString = ".";
                    if (manufacture.Amount != null) { ManufactureInfo.Instance.Amount.Select(manufacture.Amount.Length, 0); }
                    break;

                case "Worker":
                    ManufactureInfo.Instance.Worker.Focus();
                    VisibleTenKey = false;
                    VisibleWorker = true;
                    VisibleWorkProcess = false;
                    if (manufacture.Worker != null) { ManufactureInfo.Instance.Worker.Select(manufacture.Worker.Length, 0); }
                    break;

                case "WorkProcess":
                    ManufactureInfo.Instance.WorkProcess.Focus();
                    VisibleTenKey = false;
                    VisibleWorker = false;
                    VisibleWorkProcess = true;
                    if (manufacture.WorkProcess != null) { ManufactureInfo.Instance.WorkProcess.Select(manufacture.WorkProcess.Length, 0); }
                    break;

                default:
                    break;
            }
        }

        //フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            LotNumber = DisplayLotNumber(LotNumber);    //ロット情報の取得
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
