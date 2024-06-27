using ClassBase;
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
    public partial class TransportInfo : Page
    {
        public static TransportInfo Instance
        { get; set; }
        public TransportInfo()
        {
            Instance = this;
            DataContext = ViewModelTransportInfo.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportInfo : Common, IKeyDown, IWorker
    {
        //プロパティ変数
        string _InProcessCODE;
        string _LotNumber;
        string _ProcessName;
        int _AmountLength = 6;
        bool _VisibleTenKey;
        bool _VisibleWorker;

        //プロパティ
        public static ViewModelTransportInfo Instance       //インスタンス
        { get; set; } = new ViewModelTransportInfo();
        public override string ProcessName                  //工程区分
        {
            get { return _ProcessName; }
            set
            {
                SetProperty(ref _ProcessName, value);
                if (value == null) { return; }
                iProcess = ProcessCategory.SetProcess(value);
            }
        }
        public override string InProcessCODE                //仕掛コード
        {
            get { return _InProcessCODE; }
            set 
            {
                SetProperty(ref _InProcessCODE, value);
                if (value == null) { return; }
                DisplayData();
            }
        }
        public override string LotNumber                    //ロット番号
        {
            get { return _LotNumber; }
            set { SetProperty(ref _LotNumber, value); }
        }
        public int AmountLength                             //文字数（数量）
        {
            get { return _AmountLength; }
            set { SetProperty(ref _AmountLength, value); }
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
                    ViewModelWindowMain.Instance.FramePage.Navigate(new TransportList());
                    break;

                case "Enter":
                    //フォーカス移動
                    NextFocus();
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

                case "DisplayInfo":
                    //引取登録画面
                    Initialize();
                    SetGotFocus("LotNumber");
                    break;

                case "DisplayList":
                    //引取履歴画面
                    ViewModelWindowMain.Instance.FramePage.Navigate(new TransportHistory());
                    break;

                case "DisplayPlan":
                    //仕掛置場
                    ViewModelWindowMain.Instance.FramePage.Navigate(new TransportList());
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
            ViewModelWindowMain.Instance.FramePage.Navigate(new TransportList());
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

        //入力制御
        private void DisplayText(object value)
        {
            switch (Focus)
            {
                default:
                    break;
            }
        }

        //文字列消去
        private void ClearText()
        {
            switch (Focus)
            {
                default:
                    break;
            }
        }

        //バックスペース処理
        private void BackSpaceText()
        {
            switch (Focus)
            {
                default:
                    break;
            }
        }

        //次のフォーカスへ
        private void NextFocus()
        {
            switch (Focus)
            {
                case "Worker":
                    SetGotFocus("Amount");
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
                    TransportInfo.Instance.Worker.Focus();
                    VisibleTenKey = false;
                    VisibleWorker = true;
                    if (inProcess.TransportWorker != null) { TransportInfo.Instance.Worker.Select(inProcess.TransportWorker.Length, 0); }
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
