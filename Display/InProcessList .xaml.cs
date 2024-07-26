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
    public partial class InProcessList : UserControl
    {
        public InProcessList()
        {
            DataContext = ViewModelInProcessList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelInProcessList : Common, IKeyDown, ISelect
    {
        //変数
        string processName;
        string inProcessCODE;
        string inProcessDate;
        bool visibleShape;
        bool visibleUnit;
        bool visibleWeight;
        string headerUnit;
        string headerWeight;
        string headerAmount;

        //プロパティ
        public static ViewModelInProcessList Instance   //インスタンス
        { get; set; } = new ViewModelInProcessList();
        public string ProcessName                       //工程区分
        {
            get { return inProcess.ProcessName; }
            set 
            { 
                SetProperty(ref processName, value);
                inProcess.ProcessName = value;

                if (value == null) { return; }
                iProcess = ProcessCategory.SetProcess(value);
                switch (value)
                {
                    case "合板":
                        VisibleShape = true;
                        VisibleUnit = true;
                        VisibleWeight = false;
                        HeaderUnit = "数量";
                        HeaderAmount = "重量";
                        break;

                    case "プレス":
                        VisibleShape = false;
                        VisibleUnit = false;
                        VisibleWeight = true;
                        HeaderAmount = "数量";
                        HeaderWeight = "単重";
                        break;

                    default:
                        VisibleShape = false;
                        VisibleUnit = false;
                        VisibleWeight = false;
                        HeaderAmount = "数量";
                        break;
                }
            }
        }
        public string InProcessCODE                     //仕掛在庫CODE
        {
            get { return inProcessCODE; }
            set { SetProperty(ref inProcessCODE, value); }
        }
        public string InProcessDate                     //作業日
        {
            get { return inProcessDate; }
            set 
            { 
                SetProperty(ref inProcessDate, value);
                inProcess.InProcessDate = value;
                DiaplayList();
            }
        }
        public bool VisibleShape                        //表示・非表示（形状）
        {
            get { return visibleShape; }
            set { SetProperty(ref visibleShape, value); }
        }
        public bool VisibleUnit                         //表示・非表示（コイル・枚数）
        {
            get { return visibleUnit; }
            set { SetProperty(ref visibleUnit, value); }
        }
        public bool VisibleWeight                       //表示・非表示（重量）
        {
            get { return visibleWeight; }
            set { SetProperty(ref visibleWeight, value); }
        }
        public string HeaderUnit                        //コイル・枚数
        {
            get { return headerUnit; }
            set { SetProperty(ref headerUnit, value); }
        }
        public string HeaderWeight                      //焼結重量・単重
        {
            get { return headerWeight; }
            set { SetProperty(ref headerWeight, value); }
        }
        public string HeaderAmount                      //ヘッダー（重量・数量）
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
        internal ViewModelInProcessList()
        {
            Instance = this;
            inProcess = new InProcess();

            //デフォルト値設定
            InProcessDate = STRING.ToDateDB(SetToDay(DateTime.Now));
        }

        //ロード時
        private void OnLoad()
        {           
            ViewModelWindowMain.Instance.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;
            DisplayCapution();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            Initialize();
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = true;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.ProcessWork = "搬入履歴";
        }
        
        //初期化
        public void Initialize()
        {
            ProcessName = INI.GetString("Page", "Process");
            InProcessCODE = string.Empty;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //仕掛在庫登録画面
                    InProcessCODE = null;
                    ViewModelWindowMain.Instance.FramePage = new InProcessInfo();
                    break;

                case "DisplayList":
                    //仕掛在庫一覧画面
                    InProcessDate = DateTime.Now.ToString("yyyyMMdd");
                    ViewModelWindowMain.Instance.FramePage = new InProcessList();
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.FramePage = new PlanList();
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
            SelectTable = inProcess.SelectList(null, null, InProcessDate);           
        }

        //選択処理
        public async void SelectList()
        {
            if(SelectedItem == null) { return; }
            InProcessCODE = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            ViewModelPlanList.Instance.LotNumber = null;
            ViewModelWindowMain.Instance.FramePage = new InProcessInfo();
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
