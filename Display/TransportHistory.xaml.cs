using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class TransportHistory : UserControl
    {
        //コンストラクター
        public TransportHistory()
        {
            DataContext = ViewModelTransportHistory.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportHistory : Common, IKeyDown, ISelect
    {
        //変数
        ViewModelWindowMain windowMain;
        DataGridBehavior dataGridBehavior;
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
                windowMain.ProcessName = value;
                inProcess.ProcessName = value;
                process = new ProcessCategory(value);
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
                inProcess.TransportDate = value;
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

            //デフォルト値設定
            TransportDate = STRING.ToDateDB(SetToDay(DateTime.Now));
            SelectedIndex = -1;
        }

        //ロード時
        private void OnLoad()
        {
            SetInterface();
            DisplayCapution();
            DiaplayList();
        }

        //インターフェース設定
        private void SetInterface()
        {
            windowMain = ViewModelWindowMain.Instance;
            dataGridBehavior = DataGridBehavior.Instance;

            windowMain.Ikeydown = this;
            dataGridBehavior.Iselect = this;
            Instance = this;
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            Initialize();
            windowMain.ProcessWork = "引取履歴";
            windowMain.VisiblePower = true;
            windowMain.VisibleList = true;
            windowMain.VisibleInfo = false;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = true;
            windowMain.VisiblePlan = true;
            windowMain.InitializeIcon();
            windowMain.IconList = "ViewList";
            windowMain.IconPlan = "FileDocumentArrowRightOutline";
        }

        //初期化
        public void Initialize()
        {
            ProcessName = IniFile.GetString("Page", "Process");
            InProcessCODE = string.Empty;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //引取登録
                    windowMain.FramePage = new TransportInfo();
                    break;

                case "DisplayList":
                    //引取履歴
                    TransportDate = DateTime.Now.ToString("yyyyMMdd");
                    windowMain.FramePage = new TransportHistory();
                    break;

                case "DisplayPlan":
                    //仕掛置場
                    windowMain.FramePage = new TransportList();
                    break;

                case "PreviousDate":
                    //前日へ移動
                    TransportDate = DATETIME.AddDate(TransportDate, -1).ToString("yyyyMMdd");
                    break;
                
                case "NextDate":
                    //次の日へ移動
                    TransportDate = DATETIME.AddDate(TransportDate, 1).ToString("yyyyMMdd");
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
            TransportInfo.InProcessCODE = InProcessCODE;
            windowMain.FramePage = new TransportInfo();
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
