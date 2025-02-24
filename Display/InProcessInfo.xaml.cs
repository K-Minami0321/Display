using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

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
        string inProcessCODE;
        string lotNumber;
        string coil;
        string unit = string.Empty;
        string weight = string.Empty;
        string amount = string.Empty;
        string shirringUnit;
        string inProcessDate;
        string transportDate = string.Empty;
        string completed;
        string status = "搬入";
        string notice;
        string buttonName;
        int amountWidth = 150;
        int amountRow = 5;
        int lengthLotNumber = 10;
        int lengthAmount = 6;
        int lengthWeight = 6;
        int lengthUnit = 6;
        string labelWeight;
        string labelAmount;
        string labelUnit;
        bool visibleCoil;
        bool visibleItem1;
        bool visibleItem2;
        bool visibleDelete;
        bool visibleCancel;
        bool visibleTenKey;
        bool visibleWorker;
        bool isRegist = true;
        bool isEnable;
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

                InProcess inProcess = new InProcess(ProcessName);
                inProcess.InProcessCODE = value;
                CopyProperty(inProcess, this, "InProcessCODE");

                DisplayLot(LotNumber, value);
                IsRegist = string.IsNullOrEmpty(value);
            }
        }
        public string LotNumber         //ロット番号
        {
            get => lotNumber;
            set => SetProperty(ref lotNumber, value);
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
        public string InProcessDate     //搬入日
        {
            get => inProcessDate;
            set 
            { 
                IsEnable = value.ToDate() < SetVerificationDay(DateTime.Now) ? false : true;
                SetProperty(ref inProcessDate, value);
            }
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
        public bool FocusLotNumber      //フォーカス（ロット番号）
        {
            get => focusLotNumber;
            set => SetProperty(ref focusLotNumber, value);
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
        internal ViewModelInProcessInfo(string code, string number)
        {
            Initialize();
            InProcessCODE = code;
            if (string.IsNullOrEmpty(code)) { LotNumber = number; DisplayLot(LotNumber, InProcessCODE); }     //予定表からロット番号取得
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();           
            SetFocus();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            var windowMain = ViewModelWindowMain.Instance;
            windowMain.VisiblePower = true;
            windowMain.VisibleList = true;
            windowMain.VisibleInfo = true;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = false;
            windowMain.VisiblePlan = true;
            windowMain.InitializeIcon();
            windowMain.ProcessWork = "完了実績";
            windowMain.ProcessName = ProcessName;
            windowMain.Ikeydown = this;
            windowMain.Itimer = this;

            ViewModelControlTenKey.Instance.Itenkey = this;
            ViewModelControlWorker.Instance.Iworker = this;

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

        //初期化
        public void Initialize()
        {
            ReadINI();
            InProcessDate = SetToDay(DateTime.Now);
            InProcessCODE = string.Empty;
            LotNumber = string.Empty;
            AmountWidth = 150;
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
                var messege = (bool)await DialogHost.Show(new ControlMessage(messege1, messege2, messege3));
                await System.Threading.Tasks.Task.Delay(100);
                if (messege) { SetGotFocus(focus); }
            }
            return result;
        }

        //登録処理
        private void RegistData()
        {
            var inProcess = new InProcess();
            CopyProperty(this, inProcess);

            //コード確定
            if (IsRegist)
            {
                var inprocessdate = InProcessDate.ToStringDateDB();
                var inprocesscode = inProcess.GenerateCode(Mark + inprocessdate);
                InProcessCODE = inprocesscode;
            }

            //登録処理
            inProcess.InsertLog(IsRegist);
            inProcess.Resist(InProcessCODE);
            IsRegist = true;
            Initialize();
        }

        //削除処理
        private void DeleteDate()
        {
            var inProcess = new InProcess();
            inProcess.DeleteLog();
            inProcess.DeleteHistory(InProcessCODE);
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
                        DisplayFramePage(new InProcessList());
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
                    Completed = Completed == "E" ? "" : "E";
                    break;

                case "DisplayInfo":
                    //搬入登録画面
                    Initialize();
                    SetFocus();
                    break;

                case "DisplayList":
                    //仕掛在庫一覧画面
                    DisplayFramePage(new InProcessList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    DisplayFramePage(new PlanList());
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
                    if (LotNumber.Length < LengthLotNumber) { LotNumber += value.ToString(); }
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
            var controlTenKey = ViewModelControlTenKey.Instance;

            Focus = value;
            switch (Focus)
            {
                case "LotNumber":
                    FocusLotNumber = true;
                    FocusWorker = false;
                    FocusWeight = false;
                    FocusUnit = false;
                    FocusCompleted = false;
                    FocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    controlTenKey.InputString = "-";
                    break;

                case "Worker":
                    FocusLotNumber = false;
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
                    FocusWorker = false;
                    FocusWeight = true;
                    FocusUnit = false;
                    FocusCompleted = false;
                    FocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    controlTenKey.InputString = ".";
                    break;

                case "Unit":
                    FocusLotNumber = false;
                    FocusWorker = false;
                    FocusWeight = false;
                    FocusUnit = true;
                    FocusCompleted = false;
                    FocusAmount = false;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    controlTenKey.InputString = ".";
                    break;

                case "Completed":
                    FocusLotNumber = false;
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
                    FocusWorker = false;
                    FocusWeight = false;
                    FocusUnit = false;
                    FocusCompleted = false;
                    FocusAmount = true;
                    VisibleTenKey = true;
                    VisibleWorker = false;
                    controlTenKey.InputString = ".";
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
            if (IsRegist) { InProcessDate = SetToDay(DateTime.Now); }
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
