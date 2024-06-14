using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Data;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class DefectInfo : Page
    {
        public static DefectInfo Instance
        { get; set; }
        public DefectInfo()
        {
            Instance = this;
            DataContext = ViewModelDefectInfo.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelDefectInfo : Common, IKeyDown, ITenKey, IDefectCategory ,IDefect, IDisposable
    {
        //変数
        CompositeDisposable Disposable                  //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ変数
        string _EquipmentCODE;

        //プロパティ
        public static ViewModelDefectInfo Instance      //インスタンス
        { get; set; } = new ViewModelDefectInfo();
        public string EquipmentCODE                     //設備CODE
        {
            get { return _EquipmentCODE; }
            set
            {
                _EquipmentCODE = value;

                equipment.EquipmentCODE = value;
                equipment.Select();

                var name = equipment.EquipmentName;
                if (!string.IsNullOrEmpty(name))
                {
                    name = name + " - " + EquipmentCODE;
                }
                ViewModelWindowMain.Instance.ProcessWork.Value = !string.IsNullOrEmpty(name) ? name : ProcessName.Value;
            }
        }




        public ReactivePropertySlim<DataTable> DefectList       //不良データ
        { get; set; } = new ReactivePropertySlim<DataTable>();
        public ReactivePropertySlim<string> ProcessName         //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ManufactureCODE     //製造CODE
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ManufactureDate     //作業日
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> LotNumber           //ロット番号
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> WorkProcess         //工程
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Category            //不良区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Contents            //不良内容
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Amount              //数量
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Weight              //重量
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> VisibleCategory       //不良区分（表示・非表示）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleDefect         //不良内容（表示・非表示）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleTenKey         //テンキー（表示・非表示）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleWeight         //重量（表示・非表示）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleDelete         //削除ボタン（表示・非表示）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<int> AmountLength           //文字数（数量）
        { get; set; } = new ReactivePropertySlim<int>(5);
        public ReactivePropertySlim<int> WeightLength           //文字数（数量）
        { get; set; } = new ReactivePropertySlim<int>(5);
        public ReactivePropertySlim<string> ButtonName          //登録ボタン名
        { get; set; } = new ReactivePropertySlim<string>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand gotFocus;
        public ICommand GotFocus => gotFocus ??= new ActionCommand(SetGotFocus);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ設定
            LotNumber = management.ToReactivePropertySlimAsSynchronized(x => x.LotNumber).AddTo(Disposable);
            ProductName = management.ToReactivePropertySlimAsSynchronized(x => x.ProductName).AddTo(Disposable);
            ManufactureCODE = defect.ToReactivePropertySlimAsSynchronized(x => x.ManufactureCODE).AddTo(Disposable);
            Category = defect.ToReactivePropertySlimAsSynchronized(x => x.Category).AddTo(Disposable);
            Contents = defect.ToReactivePropertySlimAsSynchronized(x => x.Contents).AddTo(Disposable);
            Amount = defect.ToReactivePropertySlimAsSynchronized(x => x.Amount).AddTo(Disposable);
            Weight = defect.ToReactivePropertySlimAsSynchronized(x => x.Weight).AddTo(Disposable);

            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                if (x == null) { return; }
                iProcess = ProcessCategory.SetProcess(x);
                DisplayItem();
            }).AddTo(Disposable);
            LotNumber.Subscribe(x =>
            {
                DisplayLotData();
            }).AddTo(Disposable);
            ManufactureCODE.Subscribe(x =>
            {
                ButtonName.Value = string.IsNullOrEmpty(x) ? "登　録" : "修　正";
                VisibleDelete.Value = string.IsNullOrEmpty(x) ? false : true;
            }).AddTo(Disposable);

        }

        //コンストラクター
        internal ViewModelDefectInfo()
        {
            manufacture = new Manufacture();
            management = new Management();
            equipment = new Equipment();
            defect = new Defect();
            SetProperty();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            ViewModelControlTenKey.Instance.Itenkey = this;
            ViewModelControlDefectCategory.Instance.IdefectCategory = this;
            ViewModelControlDefect.Instance.Idefect = this;
            ViewModelWindowMain.Instance.ProcessName.Value = INI.GetString("Page", "Process");
            ViewModelWindowMain.Instance.InitializeIcon();

            DisplayCapution();
            DisplayData();
            SetGotFocus("Category");
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            EquipmentCODE = INI.GetString("Page", "Equipment");

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisibleList.Value = false;
            ViewModelWindowMain.Instance.VisibleInfo.Value = true;
            ViewModelWindowMain.Instance.VisibleDefect.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = true;

            //コード引継ぎ
            ManufactureCODE.Value = ViewModelManufactureInfo.Instance.ManufactureCODE.Value;
            ManufactureDate.Value = ViewModelManufactureInfo.Instance.ManufactureDate.Value;
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            LotNumber.Value = ViewModelManufactureInfo.Instance.LotNumber.Value;
            WorkProcess.Value = ViewModelManufactureInfo.Instance.WorkProcess.Value;
            DefectList.Value = ViewModelManufactureInfo.Instance.DefectList.Value;
        }

        //初期化
        public void Initialize()
        {
            //入力データ初期化
            Category.Value = string.Empty;
            Contents.Value = string.Empty;
            Amount.Value = string.Empty;
            Weight.Value = string.Empty;
        }

        //データ表示
        private void DisplayData()
        {
            //不良データ取得
            Initialize();
            DefectList.Value = defect.Select();
        }

        //ロット取得
        private void DisplayLotData()
        {
            management.Select(LotNumber.Value);
        }

        //入力項目の表示
        private void DisplayItem()
        {
            switch (iProcess.Name)
            {
                case "合板":
                    VisibleWeight.Value = true;
                    break;

                default:
                    VisibleWeight.Value = false;
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
                    if (await IsRequiredRegist()) { RegistData(); }
                    break;

                case "Delete":
                    //削除処理
                    result = (bool)await DialogHost.Show(new ControlMessage("搬入データを削除します", "※削除されたデータは復元できません", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus.Value);
                    if (result) { DeleteDate(); }
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

                case "1": case "2": case "3": case "4": case "5": case "6": case "7": case "8": case "9": case "0":
                    //テンキー処理
                    DisplayText(value);
                    break;

                case "DisplayInfo":
                    //加工登録画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new ManufactureInfo());
                    break;
            }
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus.Value)
            {
                case "Category":
                    Category.Value = value.ToString();
                    break;

                case "Contents":
                    Contents.Value = value.ToString();
                    break;
            }
            NextFocus();
        }

        //登録処理
        public void RegistData()
        {







        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(Amount.Value))
            {
                focus = "Amount";
                messege1 = "数量を入力してください";
                messege2 = "※数量は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (string.IsNullOrEmpty(Contents.Value))
            {
                focus = "Contents";
                messege1 = "不良内容を選択してください";
                messege2 = "※不良内容は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (string.IsNullOrEmpty(LotNumber.Value))
            {
                focus = "Category";
                messege1 = "不良区分を入力してください";
                messege2 = "※不良区分は必須項目です。";
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






            Initialize();
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
                        DefectInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    }
                    break;

                case "Weight":
                    if (Weight.Value.Length < WeightLength.Value)
                    {
                        Weight.Value += value.ToString();
                        DefectInfo.Instance.Weight.Select(Weight.Value.Length, 0);
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
                    DefectInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    break;

                case "Weight":
                    Weight.Value = string.Empty;
                    DefectInfo.Instance.Weight.Select(Weight.Value.Length, 0);
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
                        DefectInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    }
                    break;

                case "Weight":
                    if (Weight.Value.Length > 0)
                    {
                        Weight.Value = Weight.Value[..^1];
                        DefectInfo.Instance.Weight.Select(Weight.Value.Length, 0);
                    }
                    break;

                default:
                    break;
            }
        }

        //確定処理
        private void NextFocus()
        {
            switch (Focus.Value)
            {
                case "Category":
                    SetGotFocus("Worker");
                    break;

                case "Contents":
                    SetGotFocus("Amount");
                    break;

                case "Amount":
                    SetGotFocus("Weight");
                    break;

                case "Weight":
                    SetGotFocus("Category");
                    break;

                default:
                    break;
            }
        }

        //フォーカス処理（GotForcus）
        private void SetGotFocus(object value)
        {
            Focus.Value = value;
            switch (Focus.Value)
            {
                case "Category":
                    DefectInfo.Instance.Category.Focus();
                    VisibleCategory.Value = true;
                    VisibleDefect.Value = false;
                    VisibleTenKey.Value = false;
                    if (Category != null) { DefectInfo.Instance.Category.Select(Category.Value.Length, 0); }
                    break;

                case "Contents":
                    DefectInfo.Instance.Contents.Focus();
                    VisibleCategory.Value = false;
                    VisibleDefect.Value = true;
                    VisibleTenKey.Value = false;
                    if (Contents != null) { DefectInfo.Instance.Contents.Select(Contents.Value.Length, 0); }
                    break;

                case "Amount":
                    DefectInfo.Instance.Amount.Focus();
                    VisibleCategory.Value = false;
                    VisibleDefect.Value = false;
                    VisibleTenKey.Value = true;
                    ViewModelControlTenKey.Instance.InputString.Value = ".";
                    if (Amount != null) { DefectInfo.Instance.Amount.Select(Amount.Value.Length, 0); }
                    break;

                case "Weight":
                    DefectInfo.Instance.Weight.Focus();
                    VisibleCategory.Value = false;
                    VisibleDefect.Value = false;
                    VisibleTenKey.Value = true;
                    ViewModelControlTenKey.Instance.InputString.Value = ".";
                    if (Weight != null) { DefectInfo.Instance.Weight.Select(Weight.Value.Length, 0); }
                    break;
            }
        }

        //スワイプ処理
        public void Swipe(object value)
        {



        }

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
