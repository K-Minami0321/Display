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
    public partial class TransportInfo : UserControl
    {
        //コンストラクター
        public TransportInfo(string code)
        {
            DataContext = new ViewModelTransportInfo(code);
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportInfo : Common, IKeyDown, IWorker, ITimer
    {
        //変数
        string processName;
        string inProcessCODE;
        string lotNumber;
        string productName;
        string shapeName;
        string transportDate;
        string transportWorker;
        string unit;
        string place;
        string status;
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
                InProcess inProcess = new InProcess(value, ProcessName);
                CopyProperty(inProcess, this, "InProcessCODE");
                SetProperty(ref inProcessCODE, value);
            }
        }
        public string LotNumber             //ロット番号
        {
            get { return lotNumber; }
            set 
            {
                Management management = new Management(GetLotNumber(value), ProcessName);
                CopyProperty(management, this, "LotNumber");
                if (!string.IsNullOrEmpty(ProductName) && management.ProductName != ProductName) { Sound.PlayAsync(SoundFolder + CONST.SOUND_LOT); }
                SetProperty(ref lotNumber, value);
            }
        }
        public string ProductName           //品番
        {
            get { return productName; }
            set { SetProperty(ref productName, value); }
        }
        public string ShapeName             //形状
        {
            get { return shapeName; }
            set { SetProperty(ref shapeName, value); }
        }
        public string TransportDate         //搬出日
        {
            get { return transportDate; }
            set { SetProperty(ref transportDate, value); }
        }
        public string TransportWorker       //作業者
        {
            get { return transportWorker; }
            set { SetProperty(ref transportWorker, value); }
        }
        public string Unit                  //重量
        {
            get { return unit; }
            set { SetProperty(ref unit, value); }
        }
        public string Place                 //場所
        {
            get { return place; }
            set { SetProperty(ref place, value); }
        }
        public string Status                //ステータス
        {
            get { return status; }
            set { SetProperty(ref status, value); }
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

        //コンストラクター
        internal ViewModelTransportInfo(string code)
        {
            //仕掛移動データ取得
            Initialize();
            InProcessCODE = code;
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            SetGotFocus("Worker");
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            ViewModelWindowMain windowMain = ViewModelWindowMain.Instance;
            windowMain.VisiblePower = true;
            windowMain.VisibleList = true;
            windowMain.VisibleInfo = false;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = false;
            windowMain.VisiblePlan = true;
            windowMain.Ikeydown = this;
            windowMain.Itimer = this;
            windowMain.InitializeIcon();
            windowMain.ProcessWork = "仕掛引取";
            windowMain.IconPlan = "FileDocumentArrowRightOutline";
            windowMain.ProcessName = ProcessName;

            ViewModelControlWorker controlWorker = ViewModelControlWorker.Instance;
            controlWorker.Iworker = this;
        }

        //初期化
        public void Initialize()
        {
            ProcessName = IniFile.GetString("Page", "Process");
            TransportWorker = IniFile.GetString("Page", "Worker");
            TransportDate = SetToDay(DateTime.Now);

            IsRegist = (Status == "搬入");
            IsEnable = DATETIME.ToStringDate(TransportDate) < SetVerificationDay(DateTime.Now) ? false : true;
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus)
            {
                case "Worker":
                    TransportWorker = value.ToString();
                    IsFocusWorker = false;
                    break;
            }
            NextFocus();
        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(TransportWorker))
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

        //データ処理
        private void ProcessData(string function)
        {
            InProcess inProcess = new InProcess();

            var flg = false;
            switch (function)
            {
                case "Regist":
                    //登録
                    Place = "プレス";
                    Status = "引取";
                    CopyProperty(this, inProcess);
                    inProcess.TransportRegist(InProcessCODE);
                    DisplayFramePage(new TransportList());

                    break;

                case "Cancel":
                    //キャンセル
                    inProcess.DeleteLog();
                    Place = "合板";
                    Status = "搬入";
                    CopyProperty(this, inProcess);
                    TransportDate = string.Empty;
                    TransportWorker = string.Empty;
                    inProcess.TransportRegist(InProcessCODE);
                    break;
            }
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

        //現在の日付設定
        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsRegist) { TransportDate = SetToDay(DateTime.Now); }
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
                case "Regist":
                    //登録
                    if (await IsRequiredRegist()) { ProcessData("Regist"); }
                    break;

                case "Cancel":
                    //取消（合板に戻す）
                    result = (bool)await DialogHost.Show(new ControlMessage("仕掛引取一覧に戻ります。", "※入力されたものが消去されます", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        ProcessData("Cancel");
                        DisplayFramePage(new TransportList());
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
                    DisplayFramePage(new TransportHistory());
                    break;

                case "DisplayPlan":
                    //仕掛置場
                    DisplayFramePage(new TransportList());
                    break;
            }
        }
    }
}
