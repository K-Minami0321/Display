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
    public partial class InProcessList : Page
    {
        public static InProcessList Instance
        { get; set; }
        public InProcessList()
        {
            Instance = this;
            DataContext = ViewModelInProcessList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelInProcessList : Common, IKeyDown, ISelect
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
        public static ViewModelInProcessList Instance   //インスタンス
        { get; set; } = new ViewModelInProcessList();
        public string ProcessName                       //工程区分
        {
            get { return inProcess.ProcessName; }
            set 
            { 
                SetProperty(ref _ProcessName, value);
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
            get { return _InProcessCODE; }
            set { SetProperty(ref _InProcessCODE, value); }
        }
        public string InProcessDate                     //作業日
        {
            get { return inProcess.InProcessDate; }
            set 
            { 
                SetProperty(ref _InProcessDate, value);
                inProcess.InProcessDate = value;
                DiaplayList();
            }
        }
        public bool VisibleShape                        //表示・非表示（形状）
        {
            get { return _VisibleShape; }
            set { SetProperty(ref _VisibleShape, value); }
        }
        public bool VisibleUnit                         //表示・非表示（コイル・枚数）
        {
            get { return _VisibleUnit; }
            set { SetProperty(ref _VisibleUnit, value); }
        }
        public bool VisibleWeight                       //表示・非表示（重量）
        {
            get { return _VisibleWeight; }
            set { SetProperty(ref _VisibleWeight, value); }
        }
        public string HeaderUnit                        //コイル・枚数
        {
            get { return _HeaderUnit; }
            set { SetProperty(ref _HeaderUnit, value); }
        }
        public string HeaderWeight                      //焼結重量・単重
        {
            get { return _HeaderWeight; }
            set { SetProperty(ref _HeaderWeight, value); }
        }
        public string HeaderAmount                      //ヘッダー（重量・数量）
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
        internal ViewModelInProcessList()
        {
            inProcess = new InProcess();
            InProcessDate = STRING.ToDateDB(SetToDay(DateTime.Now));
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;

            //初期設定
            DisplayCapution();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            ViewModelWindowMain.Instance.ProcessWork = "搬入履歴";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = true;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList = "refresh";
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //仕掛在庫登録画面
                    InProcessCODE = null;
                    ViewModelWindowMain.Instance.FramePage.Navigate(new InProcessInfo());
                    break;

                case "DisplayList":
                    //仕掛在庫一覧画面
                    InProcessDate = DateTime.Now.ToString("yyyyMMdd");
                    ViewModelWindowMain.Instance.FramePage.Navigate(new InProcessList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.FramePage.Navigate(new PlanList());
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
            SelectTable = inProcess.SelectList(null, null, InProcessDate);           
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
