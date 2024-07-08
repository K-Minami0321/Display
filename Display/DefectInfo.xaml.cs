using ClassBase;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class DefectInfo : UserControl
    {
        public DefectInfo()
        {
            DataContext = ViewModelDefectInfo.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelDefectInfo : Common, IKeyDown, ITenKey, IDefectCategory ,IDefect
    {
        //プロパティ変数
        string _EquipmentCODE;
        DataTable _DefectList;
        string _ProcessName;
        string _ManufactureCODE;
        string _ManufactureDate;
        string _LotNumber;
        string _WorkProcess;
        int _AmountLength;
        int _WeightLength;
        string _ButtonName;
        bool _VisibleCategory;
        bool _VisibleDefect;
        bool _VisibleTenKey;
        bool _VisibleWeight;
        bool _VisibleDelete;
        bool _IsFocusCategory;
        bool _IsFocusContents;
        bool _IsFocusAmount;
        bool _IsFocusWeight;

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
                ViewModelWindowMain.Instance.ProcessWork = !string.IsNullOrEmpty(name) ? name : ProcessName;
            }
        }
        public DataTable DefectList                     //不良データ
        {
            get { return _DefectList; }
            set { SetProperty(ref _DefectList, value); }
        }
        public string ProcessName                       //工程区分
        {
            get { return management.ProcessName; }
            set 
            { 
                SetProperty(ref _ProcessName, value);
                management.ProcessName = value;
                if (value == null) { return; }
                iProcess = ProcessCategory.SetProcess(value);
                DisplayItem();
            }
        }
        public string ManufactureCODE                   //製造CODE
        {
            get { return defect.ManufactureCODE; }
            set 
            { 
                SetProperty(ref _ManufactureCODE, value);
                defect.ManufactureCODE = value;

                ButtonName = string.IsNullOrEmpty(value) ? "登　録" : "修　正";
                VisibleDelete = string.IsNullOrEmpty(value) ? false : true;
            }
        }
        public string ManufactureDate                   //作業日
        {
            get { return _ManufactureDate; }
            set { SetProperty(ref _ManufactureDate, value); }
        }
        public string LotNumber                         //ロット番号
        {
            get { return management.LotNumber; }
            set 
            { 
                SetProperty(ref _LotNumber, value);
                management.LotNumber = value;
                DisplayLotData();
            }
        }
        public string WorkProcess                       //工程
        {
            get { return _WorkProcess; }
            set { SetProperty(ref _WorkProcess, value); }
        }
        public int AmountLength                         //文字数（数量）
        {
            get { return _AmountLength; }
            set { SetProperty(ref _AmountLength, value); }
        }
        public int WeightLength                         //文字数（数量）
        {
            get { return _WeightLength; }
            set { SetProperty(ref _WeightLength, value); }
        }
        public string ButtonName                        //登録ボタン名
        {
            get { return _ButtonName; }
            set { SetProperty(ref _ButtonName, value); }
        }
        public bool VisibleCategory                     //不良区分（表示・非表示）
        {
            get { return _VisibleCategory; }
            set { SetProperty(ref _VisibleCategory, value); }
        }
        public bool VisibleDefect                       //不良内容（表示・非表示）
        {
            get { return _VisibleDefect; }
            set { SetProperty(ref _VisibleDefect, value); }
        }
        public bool VisibleTenKey                       //テンキー（表示・非表示）
        {
            get { return _VisibleTenKey; }
            set { SetProperty(ref _VisibleTenKey, value); }
        }
        public bool VisibleWeight                       //重量（表示・非表示）
        {
            get { return _VisibleWeight; }
            set { SetProperty(ref _VisibleWeight, value); }
        }
        public bool VisibleDelete                       //削除ボタン（表示・非表示）
        {
            get { return _VisibleDelete; }
            set { SetProperty(ref _VisibleDelete, value); }
        }
        public bool IsFocusCategory                     //フォーカス（不良区分）
        {
            get { return _IsFocusCategory; }
            set { SetProperty(ref _IsFocusCategory, value); }
        }
        public bool IsFocusContents                     //フォーカス（不良詳細）
        {
            get { return _IsFocusContents; }
            set { SetProperty(ref _IsFocusContents, value); }
        }
        public bool IsFocusAmount                       //フォーカス（数量）
        {
            get { return _IsFocusAmount; }
            set { SetProperty(ref _IsFocusAmount, value); }
        }
        public bool IsFocusWeight                       //フォーカス（重量）
        {
            get { return _IsFocusWeight; }
            set { SetProperty(ref _IsFocusWeight, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand gotFocus;
        public ICommand GotFocus => gotFocus ??= new ActionCommand(SetGotFocus);

        //コンストラクター
        internal ViewModelDefectInfo()
        {
            manufacture = new Manufacture();
            management = new Management();
            equipment = new Equipment();
            defect = new Defect();
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
            ViewModelWindowMain.Instance.ProcessName = INI.GetString("Page", "Process");
            ViewModelWindowMain.Instance.InitializeIcon();

            DisplayCapution();
            DisplayData();
            SetGotFocus("Category");
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            EquipmentCODE = INI.GetString("Page", "Equipment");

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = false;
            ViewModelWindowMain.Instance.VisibleInfo = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = true;

            //コード引継ぎ
            ManufactureCODE = ViewModelManufactureInfo.Instance.ManufactureCODE;
            ManufactureDate = ViewModelManufactureInfo.Instance.manufacture.ManufactureDate;
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            management.LotNumber = ViewModelManufactureInfo.Instance.LotNumber;
            manufacture.WorkProcess = ViewModelManufactureInfo.Instance.manufacture.WorkProcess;
            DefectList = ViewModelManufactureInfo.Instance.DefectList;
        }

        //初期化
        public void Initialize()
        {
            //入力データ初期化
            defect.Category = string.Empty;
            defect.Contents = string.Empty;
            defect.Amount = string.Empty;
            defect.Weight = string.Empty;
        }

        //データ表示
        private void DisplayData()
        {
            //不良データ取得
            Initialize();
            DefectList = defect.Select(ManufactureCODE);
        }

        //ロット取得
        private void DisplayLotData()
        {
            management.Select(LotNumber);
        }

        //入力項目の表示
        private void DisplayItem()
        {
            switch (iProcess.Name)
            {
                case "合板":
                    VisibleWeight = true;
                    break;

                default:
                    VisibleWeight = false;
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
                    SetGotFocus(Focus);
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
                    ViewModelWindowMain.Instance.FramePage = new ManufactureInfo();
                    break;
            }
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus)
            {
                case "Category":
                    defect.Category = value.ToString();
                    break;

                case "Contents":
                    defect.Contents = value.ToString();
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

            if (string.IsNullOrEmpty(defect.Amount))
            {
                focus = "Amount";
                messege1 = "数量を入力してください";
                messege2 = "※数量は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (string.IsNullOrEmpty(defect.Contents))
            {
                focus = "Contents";
                messege1 = "不良内容を選択してください";
                messege2 = "※不良内容は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (string.IsNullOrEmpty(LotNumber))
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
            switch (Focus)
            {
                case "Amount":
                    if (defect.Amount.Length < AmountLength)
                    {
                        defect.Amount += value.ToString();
                    }
                    break;

                case "Weight":
                    if (defect.Weight.Length < WeightLength)
                    {
                        defect.Weight += value.ToString();
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
                case "Amount":
                    defect.Amount = string.Empty;
                    break;

                case "Weight":
                    defect.Weight = string.Empty;
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
                case "Amount":
                    if (defect.Amount.Length > 0)
                    {
                        defect.Amount = defect.Amount[..^1];
                    }
                    break;

                case "Weight":
                    if (defect.Weight.Length > 0)
                    {
                        defect.Weight = defect.Weight[..^1];
                    }
                    break;

                default:
                    break;
            }
        }

        //確定処理
        private void NextFocus()
        {
            switch (Focus)
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
            Focus = value;
            switch (Focus)
            {
                case "Category":
                    IsFocusCategory = true;
                    IsFocusContents = false;
                    IsFocusAmount = false;
                    IsFocusWeight = false;
                    VisibleCategory = true;
                    VisibleDefect = false;
                    VisibleTenKey = false;
                    break;

                case "Contents":
                    IsFocusCategory = false;
                    IsFocusContents = true;
                    IsFocusAmount = false;
                    IsFocusWeight = false;
                    VisibleCategory = false;
                    VisibleDefect = true;
                    VisibleTenKey = false;
                    break;

                case "Amount":
                    IsFocusCategory = false;
                    IsFocusContents = false;
                    IsFocusAmount = true;
                    IsFocusWeight = false;
                    VisibleCategory = false;
                    VisibleDefect = false;
                    VisibleTenKey = true;
                    ViewModelControlTenKey.Instance.InputString = ".";
                    break;

                case "Weight":
                    IsFocusCategory = false;
                    IsFocusContents = false;
                    IsFocusAmount = false;
                    IsFocusWeight = true;
                    VisibleCategory = false;
                    VisibleDefect = false;
                    VisibleTenKey = true;
                    ViewModelControlTenKey.Instance.InputString = ".";
                    break;
            }
        }

        //スワイプ処理
        public void Swipe(object value)
        {



        }
    }
}
