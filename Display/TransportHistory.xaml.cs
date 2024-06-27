using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class TransportHistory : Page
    {
        public static TransportHistory Instance
        { get; set; }
        public TransportHistory()
        {
            Instance = this;
            DataContext = ViewModelTransportHistory.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportHistory : Common, IKeyDown, ISelect
    {
        //プロパティ変数
        string _ProcessName;
        string _InProcessCODE;
        string _InProcessDate;
        bool _VisibleShape;
        bool _VisibleUnit;
        bool _VisibleWeight;
        string _HeaderUnit;
        string _HeaderWeight;
        string _HeaderAmount;

        //プロパティ
        public static ViewModelTransportHistory Instance    //インスタンス
        { get; set; } = new ViewModelTransportHistory();
        public string ProcessName                           //工程区分
        {
            get { return _ProcessName; }
            set 
            { 
                SetProperty(ref _ProcessName, value);
                ViewModelWindowMain.Instance.ProcessName = value;
                inProcess.ProcessName = value;
                iProcess = ProcessCategory.SetProcess(value);

            }
        }
        public string InProcessCODE                         //仕掛在庫CODE
        {
            get { return _InProcessCODE; }
            set { SetProperty(ref _InProcessCODE, value); }
        }
        public string InProcessDate                         //作業日
        {
            get { return _InProcessDate; }
            set 
            { 
                SetProperty(ref _InProcessDate, value);
                inProcess.InProcessDate = value;
                DiaplayList();
            }
        }
        public bool VisibleShape                            //表示・非表示（形状）
        {
            get { return _VisibleShape; }
            set { SetProperty(ref _VisibleShape, value); }
        }
        public bool VisibleUnit                             //表示・非表示（コイル・枚数）
        {
            get { return _VisibleUnit; }
            set { SetProperty(ref _VisibleUnit, value); }
        }
        public bool VisibleWeight                           //表示・非表示（重量）
        {
            get { return _VisibleWeight; }
            set { SetProperty(ref _VisibleWeight, value); }
        }
        public string HeaderUnit                            //コイル・枚数
        {
            get { return _HeaderUnit; }
            set { SetProperty(ref _HeaderUnit, value); }
        }
        public string HeaderWeight                          //焼結重量・単重
        {
            get { return _HeaderWeight; }
            set { SetProperty(ref _HeaderWeight, value); }
        }
        public string HeaderAmount                          //ヘッダー（重量・数量）
        {
            get { return _HeaderAmount; }
            set { SetProperty(ref _HeaderAmount, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        internal ViewModelTransportHistory()
        {
            inProcess = new InProcess();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;
            DisplayCapution();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName = INI.GetString("Page", "Process");
            ViewModelWindowMain.Instance.ProcessWork = "引取履歴";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = false;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = true;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList = "ViewList";
            ViewModelWindowMain.Instance.IconPlan = "FileDocumentArrowRightOutline";
            Initialize();
        }

        //初期化
        public void Initialize()
        {
            InProcessDate = STRING.ToDateDB(SetToDay(DateTime.Now));
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //引取登録
                    ViewModelWindowMain.Instance.FramePage.Navigate(new TransportInfo());
                    break;

                case "DisplayList":
                    //引取履歴
                    ViewModelWindowMain.Instance.FramePage.Navigate(new TransportHistory());
                    break;

                case "DisplayPlan":
                    //仕掛置場
                    ViewModelWindowMain.Instance.FramePage.Navigate(new TransportList());
                    break;

                case "PreviousDate":
                    //前日へ移動
                    InProcessDate = DATETIME.AddDate(InProcessDate, -1).ToString("yyyyMMdd");
                    break;
                
                case "NextDate":
                    //次の日へ移動
                    InProcessDate = DATETIME.AddDate(InProcessDate, 1).ToString("yyyyMMdd");
                    break;

                case "Today":
                    //当日へ移動
                    InProcessDate = DateTime.Now.ToString("yyyyMMdd");
                    break;
            }
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectedIndex = -1;
            SelectTable = inProcess.SelectListTransportHistory(InProcessDate);           
        }

        //選択処理
        public async void SelectList()
        {
            if(SelectedItem == null) { return; }
            InProcessCODE = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            ViewModelPlanList.Instance.LotNumber = null;
            ViewModelWindowMain.Instance.FramePage.Navigate(new InProcessInfo());
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    KeyDown("DisplayInfo");
                    break;
            }
        }
    }
}
