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
    public partial class TransportHistory : UserControl
    {
        public TransportHistory()
        {
            DataContext = ViewModelTransportHistory.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportHistory : Common, IKeyDown, ISelect
    {
        //プロパティ変数
        string processName;
        string inProcessCODE;
        string transportDate;
        bool visibleShape;
        bool visibleUnit;
        bool visibleWeight;
        string headerUnit;
        string headerWeight;
        string headerAmount;

        //プロパティ
        public static ViewModelTransportHistory Instance    //インスタンス
        { get; set; } = new ViewModelTransportHistory();
        public string ProcessName                           //工程区分
        {
            get { return processName; }
            set 
            { 
                SetProperty(ref processName, value);
                ViewModelWindowMain.Instance.ProcessName = value;
                inProcess.ProcessName = value;
                iProcess = ProcessCategory.SetProcess(value);

            }
        }
        public string InProcessCODE                         //仕掛在庫CODE
        {
            get { return inProcessCODE; }
            set { SetProperty(ref inProcessCODE, value); }
        }
        public string TransportDate                         //作業日
        {
            get { return transportDate; }
            set 
            { 
                SetProperty(ref transportDate, value);
                inProcess.InProcessDate = value;
                DiaplayList();
            }
        }
        public bool VisibleShape                            //表示・非表示（形状）
        {
            get { return visibleShape; }
            set { SetProperty(ref visibleShape, value); }
        }
        public bool VisibleUnit                             //表示・非表示（コイル・枚数）
        {
            get { return visibleUnit; }
            set { SetProperty(ref visibleUnit, value); }
        }
        public bool VisibleWeight                           //表示・非表示（重量）
        {
            get { return visibleWeight; }
            set { SetProperty(ref visibleWeight, value); }
        }
        public string HeaderUnit                            //コイル・枚数
        {
            get { return headerUnit; }
            set { SetProperty(ref headerUnit, value); }
        }
        public string HeaderWeight                          //焼結重量・単重
        {
            get { return headerWeight; }
            set { SetProperty(ref headerWeight, value); }
        }
        public string HeaderAmount                          //ヘッダー（重量・数量）
        {
            get { return headerAmount; }
            set { SetProperty(ref headerAmount, value); }
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

            Initialize();
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = false;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = true;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList = "ViewList";
            ViewModelWindowMain.Instance.IconPlan = "FileDocumentArrowRightOutline";
            
        }

        //初期化
        public void Initialize()
        {
            TransportDate = STRING.ToDateDB(SetToDay(DateTime.Now));
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //引取登録
                    ViewModelWindowMain.Instance.FramePage = new TransportInfo();
                    break;

                case "DisplayList":
                    //引取履歴
                    ViewModelWindowMain.Instance.FramePage = new TransportHistory();
                    break;

                case "DisplayPlan":
                    //仕掛置場
                    ViewModelWindowMain.Instance.FramePage = new TransportList();
                    break;

                case "PreviousDate":
                    //前日へ移動
                    TransportDate = DATETIME.AddDate(TransportDate, -1).ToString("yyyyMMdd");
                    break;
                
                case "NextDate":
                    //次の日へ移動
                    TransportDate = DATETIME.AddDate(TransportDate, 1).ToString("yyyyMMdd");
                    break;

                case "Today":
                    //当日へ移動
                    TransportDate = DateTime.Now.ToString("yyyyMMdd");
                    break;
            }
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectTable = inProcess.SelectListTransportHistory(TransportDate);           
        }

        //選択処理
        public async void SelectList()
        {
            if(SelectedItem == null) { return; }
            InProcessCODE = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            ViewModelPlanList.Instance.LotNumber = null;
            ViewModelTransportList.Instance.InProcessCODE = InProcessCODE;
            ViewModelWindowMain.Instance.FramePage = new TransportInfo();
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
