using ClassBase;
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
        //コンストラクター
        public InProcessInfo(string code, string date, string lotnumber = "")
        {
            DataContext = new ViewModelInProcessInfo(code, date, lotnumber);
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelInProcessInfo : Common, IWindowBase, IBarcode, ITenKey, IWorker, ITimer
    {
        //変数
        InProcess inProcess = new InProcess();
        string inProcessCODE;
        string inProcessDate;
        string lotNumber;
        string lotNumberSEQ;
        string coil;
        string unit = string.Empty;
        string weight = string.Empty;
        string amount = string.Empty;
        string shirringUnit;
        string transportDate = string.Empty;
        string completed;
        string status = "搬入";
        string notice;
        string buttonName;
        int amountWidth = 150;
        int amountRow = 5;
        int lengthLotNumberSEQ = 2;
        int lengthLotNumber = 10;
        int lengthAmount = 6;
        int lengthWeight = 6;
        int lengthUnit = 6;
        string labelWeight;
        string labelAmount = "数 量";
        string labelUnit;
        bool visibleCoil;
        bool visibleItem1;
        bool visibleItem2;
        bool visibleDelete;
        bool visibleCancel;
        bool visibleTenKey;
        bool visibleWorker;
        bool isRegist = true;
        bool isEnable = true;
        bool focusLotNumberSEQ;
        bool focusLotNumber;
        bool focusWorker;
        bool focusWeight;
        bool focusUnit;
        bool focusCompleted;
        bool focusAmount;

        //プロパティ
        public string InProcessCODE     //仕掛CODE
        {
            get => inProcessCODE;
            set 
            {
                SetProperty(ref inProcessCODE, value);

                inProcess = new InProcess(ProcessName);
                inProcess.InProcessCODE = value;
                if (inProcess.DataCount == 0) { return; }

                CopyProperty(inProcess, this, "InProcessCODE");
                DisplayLot(LotNumber, value);
                IsRegist = false;
            }
        }
        public string InProcessDate     //搬入日
        {
            get => inProcessDate;
            set
            {
                if (string.IsNullOrEmpty(value)) { value = SetToDay(DateTime.Now); }
                SetProperty(ref inProcessDate, value);

                IsDate = value == SetToDay(DateTime.Now) ? true : false;
                IsEnable = value.ToDate() < SetVerificationDay(DateTime.Now) && !string.IsNullOrEmpty(InProcessCODE) ? false : true;
            }
        }
        public string LotNumber         //ロット番号
        {
            get => lotNumber;
            set => SetProperty(ref lotNumber, value);
        }
        public string LotNumberSEQ      //ロット番号SEQ
        {
            get => lotNumberSEQ;
            set => SetProperty(ref lotNumberSEQ, value);
        }
        public string Unit              //数量
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }
        public string Weight            //重量
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }
        public string Amount            //枚数
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }
        public string ShirringUnit      //コイル数
        {
            get => shirringUnit;
            set => SetProperty(ref shirringUnit, value);
        }
        public string TransportDate     //搬出日
        {
            get => transportDate;
            set => SetProperty(ref transportDate, value);
        }
        public string Completed         //完了
        {
            get => completed;
            set => SetProperty(ref completed, value);
        }
        public string Status            //状態
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        public string Notice            //注意文
        {
            get => notice;
            set => SetProperty(ref notice, value);
        }
        public string ButtonName        //登録ボタン名
        {
            get => buttonName;
            set => SetProperty(ref buttonName, value);
        }
        public int AmountWidth          //コイル数テキストボックスのWidth
        {
            get => amountWidth;
            set => SetProperty(ref amountWidth, value);
        }
        public int AmountRow            //数量・重量の位置
        {
            get => amountRow;
            set => SetProperty(ref amountRow, value);
        }
        public int LengthLotNumber      //文字数（ロット番号）
        {
            get => lengthLotNumber;
            set => SetProperty(ref lengthLotNumber, value);
        }
        public int LengthNumberSEQ      //文字数（ロット番号SEQ）
        {
            get => lengthLotNumberSEQ;
            set => SetProperty(ref lengthLotNumberSEQ, value);
        }
        public int LengthAmount         //文字数（数量）
        {
            get => lengthAmount;
            set => SetProperty(ref lengthAmount, value);
        }
        public int LengthWeight         //文字数（重量・焼結重量）
        {
            get => lengthWeight;
            set => SetProperty(ref lengthWeight, value);
        }
        public int LengthUnit           //文字数（枚数・コイル数）
        {
            get => lengthUnit;
            set => SetProperty(ref lengthUnit, value);
        }
        public string LabelWeight       //ラベル（重量・焼結重量）
        {
            get => labelWeight;
            set => SetProperty(ref labelWeight, value);
        }
        public string LabelAmount       //ラベル（数量）
        {
            get => labelAmount;
            set => SetProperty(ref labelAmount, value);
        }
        public string LabelUnit         //ラベル（数量・重量）
        {
            get => labelUnit;
            set => SetProperty(ref labelUnit, value);
        }
        public bool VisibleCoil         //表示・非表示（コイル数）
        {
            get => visibleCoil;
            set => SetProperty(ref visibleCoil, value);
        }
        public bool VisibleItem1        //表示・非表示（入力項目）
        {
            get => visibleItem1;
            set => SetProperty(ref visibleItem1, value);
        }
        public bool VisibleItem2        //表示・非表示（入力項目）
        {
            get => visibleItem2;
            set => SetProperty(ref visibleItem2, value);
        }
        public bool VisibleDelete       //表示・非表示（削除ボタン）
        {
            get => visibleDelete;
            set => SetProperty(ref visibleDelete, value);
        }
        public bool VisibleCancel       //表示・非表示（取消ボタン）
        {
            get => visibleCancel;
            set => SetProperty(ref visibleCancel, value);
        }
        public bool VisibleTenKey       //表示・非表示（テンキー）
        {
            get => visibleTenKey;
            set => SetProperty(ref visibleTenKey, value);
        }
        public bool VisibleWorker       //表示・非表示（作業者）
        {
            get => visibleWorker;
            set => SetProperty(ref visibleWorker, value);
        }
        public bool IsRegist            //新規・既存フラグ（true:新規、false:既存）
        {
            get => isRegist;
            set
            {
                SetProperty(ref isRegist, value);
                VisibleCancel = value;
                VisibleDelete = !value;
                ButtonName = value ? "登　録" : "修　正";
            }
        }
        public bool IsEnable            //表示・非表示（下部ボタン）
        {
            get => isEnable;
            set 
            { 
                SetProperty(ref isEnable, value);
                if (value) { return; }
                Notice = string.Empty;
                AmountWidth = Amount.Length * 50;
                Amount = !VisibleCoil ? Amount : Amount.ToCircleEnclosing();
            }
        }
        public bool IsDate                      //日付調整
        { get; set; }
        public bool FocusLotNumber      //フォーカス（ロット番号）
        {
            get => focusLotNumber;
            set => SetProperty(ref focusLotNumber, value);
        }
        public bool FocusLotNumberSEQ   //フォーカス（ロット番号SEQ）
        {
            get => focusLotNumberSEQ;
            set => SetProperty(ref focusLotNumberSEQ, value);
        }
        public bool FocusWorker         //フォーカス（作業者）
        {
            get => focusWorker;
            set => SetProperty(ref focusWorker, value);
        }
        public bool FocusWeight         //フォーカス（重量）
        {
            get => focusWeight;
            set => SetProperty(ref focusWeight, value);
        }
        public bool FocusUnit           //フォーカス（単位）
        {
            get => focusUnit;
            set => SetProperty(ref focusUnit, value);
        }
        public bool FocusCompleted      //フォーカス（完了）
        {
            get => focusCompleted;
            set => SetProperty(ref focusCompleted, value);
        }
        public bool FocusAmount         //フォーカス（数量）
        {
            get => focusAmount;
            set => SetProperty(ref focusAmount, value);
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
        internal ViewModelInProcessInfo(string code, string date, string lotnumber)
        {
            Ibarcode = this;

            ReadINI();
            Initialize(lotnumber);

            InProcessCODE = code;
            InProcessDate = string.IsNullOrEmpty(date) ? SetToDay(DateTime.Now) : date;
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            SetFocus();
            VisibleCoil = string.IsNullOrEmpty(ShirringUnit) ? false : true;
        }

        //初期化
        public void Initialize(string lotnumber = "")
        {
            LotNumber = lotnumber;
            LotNumberSEQ = string.Empty;
            ProductName = string.Empty;
            Worker = string.Empty;
            Weight = string.Empty;
            Unit = string.Empty;
            Amount = string.Empty;
            Completed = string.Empty;
            AmountWidth = 150;
            IsRegist = true;
            IsEnable = true;
            DisplayLot(LotNumber);
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //WindowMain
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                Itimer = this,
                VisibleList = true,
                VisibleInfo = true,
                VisibleDefect = false,
                VisibleArrow = false,
                VisiblePlan = true,
                VisiblePrinter = false,
                VisibleQRcode = false,
                Process = ProcessName,
                ProcessWork = ProcessName + "売上",
            };

            //テンキーコントロール
            TenKeyProperty = new PropertyTenKey();
            TenKeyProperty.Itenkey = this;

            //作業者コントロール
            WorkerProperty = new PropertyWorker();
            WorkerProperty.Iworker = this;

            //工程区分
            switch (ProcessName)
            {
                case "合板":

                    NextFocus = "Amount";
                    VisibleItem1 = true;
                    VisibleItem2 = true;
                    LabelWeight = "焼結重量";
                    LabelUnit = "重 量";
                    AmountRow = 5;
                    Notice = "※スリッター時のみ記入";
                    break;

                case "プレス":

                    if (NextFocus != null) { NextFocus = "LotNumber"; }
                    VisibleItem1 = true;
                    VisibleItem2 = false;
                    LabelWeight = "単 重";
                    LabelUnit = "数 量";
                    AmountRow = 5;
                    Notice = string.Empty;
                    VisibleCoil = false;
                    break;

                default:

                    if (NextFocus != null) { NextFocus = "LotNumber"; }
                    VisibleItem1 = false;
                    VisibleItem2 = false;
                    LabelUnit = "数 量";
                    AmountRow = 4;
                    Notice = string.Empty;
                    VisibleCoil = false;
                    break;
            }
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus)
            {
                case "Worker":
                    Worker = value.ToString();
                    break;
            }
            SetNextFocus();
        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            if (IsMessage) { return false; }

            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(Unit))
            {
                focus = "Unit";
                messege1 = "数量を入力してください";
                messege2 = "※数量は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (string.IsNullOrEmpty(Worker))
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
                MessageControl = new ControlMessage();
                MessageProperty = new PropertyMessage()
                {
                    Message = messege1,
                    Contents = messege2,
                    Type = messege3
                };
                var messege = (bool)await DialogHost.Show(MessageControl);
                await System.Threading.Tasks.Task.Delay(100);

                SetGotFocus(focus);
                MessageControl = null;
            }
            return result;
        }

        //登録処理
        private void RegistData()
        {
            var code = InProcessCODE;
            CopyProperty(this, inProcess);

            //コード確定
            if (IsRegist)
            {
                var inprocessdate = InProcessDate.ToStringDateDB();
                var inprocesscode = inProcess.GenerateCode(Mark + inprocessdate);
                code = inprocesscode;
            }

            //登録処理
            inProcess.InsertLog(IsRegist);
            inProcess.Resist(code);
            IsRegist = true;
            Initialize();
        }

        //削除処理
        private void DeleteDate()
        {
            inProcess.DeleteLog();
            inProcess.DeleteHistory(InProcessCODE);
            Initialize();
        }

        //入力制御
        private void DisplayText(object value)
        {
            switch (Focus)
            {
                case "LotNumber":

                    if (LotNumber == null) { LotNumber = string.Empty; }
                    if (LotNumber.Length < LengthLotNumber) { LotNumber += value.ToString(); }
                    break;

                case "LotNumberSEQ":

                    if (LotNumberSEQ == null) { LotNumberSEQ = string.Empty; }
                    if (LotNumberSEQ.Length < LengthNumberSEQ) { LotNumberSEQ += value.ToString(); }
                    break;

                case "Unit":

                    if (Unit.Length < LengthUnit) { Unit += value.ToString(); }
                    break;

                case "Weight":

                    if (Weight.Length < LengthWeight) { Weight += value.ToString(); }
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

                case "LotNumberSEQ":

                    LotNumberSEQ = string.Empty;
                    break;

                case "Unit":

                    Unit = string.Empty;
                    break;

                case "Weight":

                    Weight = string.Empty;
                    break;

                case "Amount":

                    Amount = string.Empty;
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

                case "LotNumberSEQ":

                    if (LotNumberSEQ.Length > 0) { LotNumberSEQ = LotNumberSEQ[..^1]; }
                    break;

                case "Unit":

                    if (Unit.Length > 0) { Unit = Unit[..^1]; }
                    break;

                case "Weight":

                    if (Weight.Length > 0) { Weight = Weight[..^1]; }
                    break;

                case "Amount":

                    if (Amount.Length > 0) { Amount = Amount[..^1]; }
                    break;
            }
        }

        //次のフォーカスへ
        private void SetNextFocus()
        {
            switch (Focus)
            {
                case "LotNumber":

                    SetGotFocus("LotNumberSEQ");
                    break;

                case "LotNumberSEQ":

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
            }
        }

        //フォーカス処理（GotFocus）
        private void SetGotFocus(object value)
        {
            Focus = value;
            switch (Focus)
            {
                case "LotNumber":

                    FocusLotNumber = true;
                    FocusLotNumberSEQ = false;
                    FocusWorker = false;
                    FocusWeight = false;
                    FocusUnit = false;
                    FocusCompleted = false;
                    FocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    TenKeyProperty.InputString = "-";
                    break;

                case "LotNumberSEQ":

                    FocusLotNumber = false;
                    FocusLotNumberSEQ = true;
                    FocusWorker = false;
                    FocusWeight = false;
                    FocusUnit = false;
                    FocusCompleted = false;
                    FocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    TenKeyProperty.InputString = "-";
                    break;

                case "Worker":

                    FocusLotNumber = false;
                    FocusLotNumberSEQ = false;
                    FocusWorker = true;
                    FocusWeight = false;
                    FocusUnit = false;
                    FocusCompleted = false;
                    FocusAmount = false;
                    VisibleTenKey = false;
                    VisibleWorker = true;
                    break;

                case "Weight":

                    FocusLotNumber = false;
                    FocusLotNumberSEQ = false;
                    FocusWorker = false;
                    FocusWeight = true;
                    FocusUnit = false;
                    FocusCompleted = false;
                    FocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    TenKeyProperty.InputString = ".";
                    break;

                case "Unit":

                    FocusLotNumber = false;
                    FocusLotNumberSEQ = false;
                    FocusWorker = false;
                    FocusWeight = false;
                    FocusUnit = true;
                    FocusCompleted = false;
                    FocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    TenKeyProperty.InputString = ".";
                    break;

                case "Completed":

                    FocusLotNumber = false;
                    FocusLotNumberSEQ = false;
                    FocusWorker = false;
                    FocusWeight = false;
                    FocusUnit = false;
                    FocusCompleted = true;
                    FocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    break;

                case "Amount":

                    FocusLotNumber = false;
                    FocusLotNumberSEQ = false;
                    FocusWorker = false;
                    FocusWeight = false;
                    FocusUnit = false;
                    FocusCompleted = false;
                    FocusAmount = true;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    TenKeyProperty.InputString = ".";
                    break;
            }
        }

        //ロット番号フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            DisplayLot(LotNumber, InProcessCODE);
        }

        //フォーカス設定
        private void SetFocus()
        {
            if (string.IsNullOrEmpty(LotNumber)) { SetGotFocus("LotNumber"); return; }
            if (string.IsNullOrEmpty(Worker)) { SetGotFocus("Worker"); return; }
            SetGotFocus("LotNumber");
        }

        //現在の日付設定
        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsRegist && IsDate) { InProcessDate = SetToDay(DateTime.Now); }
        }

        //QRコード処理
        public void GetQRCode()
        {
            //ロット番号
            if (CONVERT.IsLotNumber(ReceivedData)) 
            {
                LotNumber = ReceivedData.StringLeft(10);
                LotNumberSEQ = ReceivedData.StringRight(ReceivedData.Length - 11);
                DisplayLot(LotNumber, InProcessCODE);
                SetGotFocus("Worker");
            }

            //作業者
            if (CONVERT.IsWoker(ReceivedData))
            {
                Worker = ReceivedData;
                SetGotFocus("Weight");
            }
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
            var messege = false;

            switch (value)
            {
                case "Regist":

                    //登録
                    if (!(bool)await IsRequiredRegist()) { return;  }

                    if (IsMessage) { return; }

                    MessageControl = new ControlMessage();
                    MessageProperty = new PropertyMessage()
                    {
                        Message = "搬入データを登録します",
                        Contents = "",
                        Type = "警告"
                    };
                    messege = (bool)await DialogHost.Show(MessageControl);
                    await System.Threading.Tasks.Task.Delay(100);

                    SetGotFocus(Focus);
                    if (messege)
                    {
                        RegistData();
                        SetGotFocus("LotNumber");
                    }
                    MessageControl = null;
                    break;

                case "Delete":

                    //削除
                    if (IsMessage) { return; }

                    MessageControl = new ControlMessage();
                    MessageProperty = new PropertyMessage()
                    {
                        Message = "搬入データを削除します",
                        Contents = "※削除されたデータは復元できません。",
                        Type = "警告"
                    };
                    messege = (bool)await DialogHost.Show(MessageControl);
                    await System.Threading.Tasks.Task.Delay(100);

                    SetGotFocus(Focus);
                    if (messege)
                    {
                        DeleteDate();
                        DisplayFramePage(new InProcessList(InProcessDate));
                    }
                    MessageControl = null;
                    break;

                case "Cancel":

                    //取消
                    if (IsMessage) { return; }

                    MessageControl = new ControlMessage();
                    MessageProperty = new PropertyMessage()
                    {
                        Message = "搬入データをクリアします",
                        Contents = "※入力されたものが消去されます。",
                        Type = "警告"
                    };
                    messege = (bool)await DialogHost.Show(MessageControl);
                    await System.Threading.Tasks.Task.Delay(100);

                    SetGotFocus(Focus);
                    if (messege)
                    {
                        Initialize();
                        SetGotFocus("LotNumber");
                    }
                    MessageControl = null;
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
                    
                    //数字入力
                    DisplayText(value);
                    break;

                case "Completed":

                    //完了チェック
                    Completed = Completed == "E" ? "" : "E";
                    break;

                case "DisplayInfo":

                    //搬入登録画面
                    Initialize();
                    SetFocus();
                    break;

                case "DisplayList":

                    //仕掛在庫一覧画面
                    DisplayFramePage(new InProcessList(InProcessDate));
                    break;

                case "DisplayPlan":

                    //計画一覧画面
                    DisplayFramePage(new PlanList());
                    break;
            }
        }
    }
}
