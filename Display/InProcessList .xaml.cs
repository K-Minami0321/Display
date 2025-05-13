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
        public InProcessList(string date)
        {
            DataContext = new ViewModelInProcessList(date);
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelInProcessList : Common, IWindowBase, ISelect
    {
        //変数
        InProcess inProcess;
        string inProcessCODE = string.Empty;
        string inProcessDate = string.Empty;
        string headerUnit = string.Empty;
        string headerWeight = string.Empty;
        string headerAmount = string.Empty;
        bool visibleShape;
        bool visibleUnit;
        bool visibleWeight;

        //プロパティ
        public string InProcessCODE             //売上CODE
        {
            get => inProcessCODE;
            set => SetProperty(ref inProcessCODE, value);
        }
        public string InProcessDate             //売上日
        {
            get { return inProcessDate; }
            set 
            { 
                SetProperty(ref inProcessDate, value);
                DiaplayList();
            }
        }
        public string HeaderUnit                //コイル・枚数
        {
            get => headerUnit;
            set => SetProperty(ref headerUnit, value);
        }
        public string HeaderWeight              //焼結重量・単重
        {
            get => headerWeight;
            set => SetProperty(ref headerWeight, value);
        }
        public string HeaderAmount              //ヘッダー（重量・数量）
        {
            get => headerAmount;
            set => SetProperty(ref headerAmount, value);
        }
        public bool VisibleShape                //表示・非表示（形状）
        {
            get => visibleShape;
            set => SetProperty(ref visibleShape, value);
        }
        public bool VisibleUnit                 //表示・非表示（コイル・枚数）
        {
            get => visibleUnit;
            set => SetProperty(ref visibleUnit, value);
        }
        public bool VisibleWeight               //表示・非表示（重量）
        {
            get => visibleWeight;
            set => SetProperty(ref visibleWeight, value);
        }
        public static string CacheDate          //キャッシュ（対象日）
        { get; set; }
        public static int CacheSelectedIndex    //キャッシュ（選択行）
        { get; set; }
        public static double CacheScrollIndex   //キャッシュ（スクロール位置）
        { get; set; }

        //イベント
        ActionCommand? commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand? commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        public ViewModelInProcessList(string date)
        {
            inProcess = new();

            ReadINI();
            SelectedIndex = -1;
            InProcessDate = CacheDate ?? date.ToStringDateDB();
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            DiaplayList();
            StateLoad();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            WindowProperty = new()
            {
                IwindowBase = this,
                VisiblePower = true,
                VisibleList = true,
                VisibleInfo = true,
                VisibleDefect = false,
                VisibleArrow = true,
                VisiblePlan = true,
                VisiblePrinter = false,
                VisibleQRcode = false,
                ProcessWork = ProcessName + "売上"
            };

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

            Iselect = this;
        }

        //状態読込
        private void StateLoad()
        {
            SelectedIndex = CacheSelectedIndex;
            ScrollIndex = CacheScrollIndex;
        }

        //状態保存
        private void StateSave()
        {
            CacheDate = InProcessDate;
            CacheSelectedIndex = SelectedIndex;
            CacheScrollIndex = ScrollIndex;
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectTable = inProcess.SelectList(ProcessName, null, null, InProcessDate);           
        }

        //選択処理
        public async void SelectList()
        {
            if(SelectedItem == null) { return; }
            var code = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            StateSave();
            DisplayFramePage(new InProcessInfo(code, string.Empty));
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

                    //搬入登録
                    DisplayFramePage(new InProcessInfo(string.Empty, string.Empty));
                    break;

                case "DisplayList":

                    //仕掛在庫一覧
                    SelectedIndex = -1;
                    InProcessDate = DateTime.Now.ToString("yyyyMMdd");
                    DisplayFramePage(new InProcessList(InProcessDate));
                    break;

                case "DisplayPlan":

                    //計画一覧
                    DisplayFramePage(new PlanList());
                    break;

                case "PreviousDate":

                    //前日へ移動
                    InProcessDate = DATETIME.AddDate(InProcessDate, -1).ToString("yyyyMMdd");
                    SelectedIndex = 0;
                    ScrollIndex = 0;
                    break;

                case "NextDate":

                    //次の日へ移動
                    InProcessDate = DATETIME.AddDate(InProcessDate, 1).ToString("yyyyMMdd");
                    SelectedIndex = 0;
                    ScrollIndex = 0;
                    break;

                case "Today":

                    //当日へ移動
                    InProcessDate = DateTime.Now.ToString("yyyyMMdd");
                    SelectedIndex = 0;
                    ScrollIndex = 0;
                    break;
            }
        }
    }
}
