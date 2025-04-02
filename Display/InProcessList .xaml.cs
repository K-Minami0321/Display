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
    public class ViewModelInProcessList : Common, IWindowBase, ISelect
    {
        //変数
        string inProcessCODE;
        string inProcessDate;
        string headerUnit;
        string headerWeight;
        string headerAmount;
        bool visibleShape;
        bool visibleUnit;
        bool visibleWeight;

        //プロパティ
        public static ViewModelInProcessList Instance   //インスタンス
        { get; set; } = new ViewModelInProcessList();
        public string InProcessDate                     //作業日
        {
            get { return inProcessDate; }
            set 
            { 
                SetProperty(ref inProcessDate, value);
                DiaplayList();
            }
        }
        public string HeaderUnit                        //コイル・枚数
        {
            get => headerUnit;
            set => SetProperty(ref headerUnit, value);
        }
        public string HeaderWeight                      //焼結重量・単重
        {
            get => headerWeight;
            set => SetProperty(ref headerWeight, value);
        }
        public string HeaderAmount                      //ヘッダー（重量・数量）
        {
            get => headerAmount;
            set => SetProperty(ref headerAmount, value);
        }
        public bool VisibleShape                        //表示・非表示（形状）
        {
            get => visibleShape;
            set => SetProperty(ref visibleShape, value);
        }
        public bool VisibleUnit                         //表示・非表示（コイル・枚数）
        {
            get => visibleUnit;
            set => SetProperty(ref visibleUnit, value);
        }
        public bool VisibleWeight                       //表示・非表示（重量）
        {
            get => visibleWeight;
            set => SetProperty(ref visibleWeight, value);
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

            ReadINI();
            Initialize();
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //WindowMain
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                VisiblePower = true,
                VisibleList = true,
                VisibleInfo = true,
                VisibleDefect = false,
                VisibleArrow = true,
                VisiblePlan = true,
                ProcessWork = ProcessName + "売上"
            };

            DataGridBehavior.Instance.Iselect = this;

            //工程区分
            switch (ProcessName)
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
        
        //初期化
        public void Initialize()
        {
            SelectedIndex = -1;
            InProcessDate = SetToDay(DateTime.Now).ToStringDateDB();
        }

        //一覧表示
        private void DiaplayList()
        {
            var inProcess = new InProcess();
            SelectTable = inProcess.SelectList(ProcessName, null, null, InProcessDate);           
        }

        //選択処理
        public async void SelectList()
        {
            if(SelectedItem == null) { return; }
            InProcessInfo.InProcessCODE = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            InProcessInfo.LotNumber = string.Empty;
            DisplayFramePage(new InProcessInfo());
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    KeyDown("DisplayPlan");
                    break;
            }
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":

                    //搬入登録画面
                    DataInitialize();
                    DisplayFramePage(new InProcessInfo());
                    break;

                case "DisplayList":

                    //仕掛在庫一覧画面
                    Initialize();
                    DisplayFramePage(new InProcessList());
                    break;

                case "DisplayPlan":

                    //計画一覧画面
                    DisplayFramePage(new PlanList());
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
    }
}
