using ClassBase;
using DocumentFormat.OpenXml.EMMA;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    public class ViewModelTransportInfo : Common, IKeyDown, ITenKey, IWorker, IDisposable
    {
        //変数
        CompositeDisposable Disposable                      //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelTransportInfo Instance       //インスタンス
        { get; set; } = new ViewModelTransportInfo();
        public ReactivePropertySlim<string> LotNumber       //ロット番号
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ProcessName     //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> InProcessCODE   //仕掛在庫CODE
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> InProcessDate   //引取日
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<int> AmountLength       //文字数（数量）
        { get; set; } = new ReactivePropertySlim<int>(6);
        public ReactivePropertySlim<bool> VisibleTenKey     //表示・非表示（テンキー）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleWorker     //表示・非表示（作業者）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<string> Amount          //数量
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Worker          //担当者
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Status          //作業区分
        { get; set; } = new ReactivePropertySlim<string>("移動");

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand gotFocus;
        public ICommand GotFocus => gotFocus ??= new ActionCommand(SetGotFocus);
        ActionCommand lostFocus;
        public ICommand LostFocus => lostFocus ??= new ActionCommand(SetLostFocus);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ設定
            LotNumber = inProcess.ToReactivePropertySlimAsSynchronized(x => x.LotNumber).AddTo(Disposable);
            ProcessName = inProcess.ToReactivePropertySlimAsSynchronized(x => x.ProcessName).AddTo(Disposable);
            InProcessCODE = inProcess.ToReactivePropertySlimAsSynchronized(x => x.InProcessCODE).AddTo(Disposable);
            InProcessDate = inProcess.ToReactivePropertySlimAsSynchronized(x => x.InProcessDate).AddTo(Disposable);
            Amount = inProcess.ToReactivePropertySlimAsSynchronized(x => x.Amount).AddTo(Disposable);
            Worker = inProcess.ToReactivePropertySlimAsSynchronized(x => x.Worker).AddTo(Disposable);
            Status = inProcess.ToReactivePropertySlimAsSynchronized(x => x.Status).AddTo(Disposable);

            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                if (x == null) { return; }
                iProcess = ProcessCategory.SetProcess(x);
            }).AddTo(Disposable);
        }

        //コンストラクター
        internal ViewModelTransportInfo()
        {
            inProcess = new InProcess();
            management = new Management();
            SetProperty();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            ViewModelControlTenKey.Instance.Itenkey = this;
            ViewModelControlWorker.Instance.Iworker = this;
            ViewModelWindowMain.Instance.ProcessName.Value = INI.GetString("Page", "Process");
            DisplayCapution();
            DisplayData();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            ViewModelWindowMain.Instance.ProcessWork.Value = "仕掛搬出";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisibleList.Value = true;
            ViewModelWindowMain.Instance.VisibleInfo.Value = false;
            ViewModelWindowMain.Instance.VisibleDefect.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = false;
            ViewModelWindowMain.Instance.VisiblePlan.Value = false;
            ViewModelWindowMain.Instance.InitializeIcon();

            //編集モード
            InProcessCODE.Value = ViewModelTransportList.Instance.InProcessCODE.Value;
        }

        //初期化
        public void Initialize()
        {
            //入力データ初期化
            InProcessDate.Value = SetToDay(DateTime.Now);
            ProductName.Value = string.Empty;
            LotNumber.Value = string.Empty;
            Amount.Value = string.Empty;
            Worker.Value = INI.GetString("Page", "Worker");
        }

        //データ表示
        private void DisplayData()
        {
            //前工程の仕掛取得
            Initialize();
            inProcess.TransportSelect();
            LotNumber.Value = DisplayLotNumber(LotNumber.Value);
            SetGotFocus("Amount");
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
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new TransportList());
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
                    //搬出登録画面
                    Initialize();
                    SetGotFocus("LotNumber");
                    break;

                case "DisplayList":
                    //搬出一覧画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new TransportList());
                    break;
            }
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus.Value)
            {
                case "Worker":
                    Worker.Value = value.ToString();
                    break;
            }
            NextFocus();
        }

        //登録処理
        private void RegistData()
        {
            //登録
            //inProcess.Resist();
            ViewModelWindowMain.Instance.FramePage.Value.Navigate(new TransportList());

        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(Worker.Value))
            {
                focus = "Worker";
                messege1 = "担当者を選択してください";
                messege2 = "※担当者は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (string.IsNullOrEmpty(Amount.Value))
            {
                focus = "Amount";
                messege1 = "重量を入力してください";
                messege2 = "※重量は必須項目です。";
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
            switch (Focus.Value)
            {
                case "Amount":
                    if (Amount.Value.Length < AmountLength.Value)
                    {
                        Amount.Value += value.ToString();
                        TransportInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    }
                    break;
                default:
                    break;
            }
        }

        //文字列消去
        private void ClearText()
        {
            switch (Focus.Value)
            {
                case "Amount":
                    Amount.Value = string.Empty;
                    TransportInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    break;

                default:
                    break;
            }
        }

        //バックスペース処理
        private void BackSpaceText()
        {
            switch (Focus.Value)
            {
                case "Amount":
                    if (Amount.Value.Length > 0)
                    {
                        Amount.Value = Amount.Value[..^1];
                        TransportInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    }
                    break;

                default:
                    break;
            }
        }

        //次のフォーカスへ
        private void NextFocus()
        {
            switch (Focus.Value)
            {
                case "Amount":
                    SetGotFocus("Worker");
                    break;

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
            Focus.Value = value;
            switch (Focus.Value)
            {
                case "Amount":
                    TransportInfo.Instance.Amount.Focus();
                    VisibleTenKey.Value = true;
                    VisibleWorker.Value = false;
                    ViewModelControlTenKey.Instance.InputString.Value = ".";
                    if (Amount != null) { TransportInfo.Instance.Amount.Select(Amount.Value.Length, 0); }
                    break;

                case "Worker":
                    TransportInfo.Instance.Worker.Focus();
                    VisibleTenKey.Value = false;
                    VisibleWorker.Value = true;
                    if (Worker != null) { TransportInfo.Instance.Worker.Select(Worker.Value.Length, 0); }
                    break;

                default:
                    break;
            }
        }

        //フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            LotNumber.Value = DisplayLotNumber(LotNumber.Value);
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
