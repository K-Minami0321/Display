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
        //コンストラクター
        public TransportHistory()
        {
            DataContext = ViewModelTransportHistory.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportHistory : Common, IWindowBase, ISelect
    {
        //変数
        string processName;
        string inProcessCODE;
        string transportDate;
        string headerUnit;
        string headerWeight;
        string headerAmount;
        bool visibleShape;
        bool visibleUnit;
        bool visibleWeight;

        //プロパティ
        public static ViewModelTransportHistory Instance    //インスタンス
        { get; set; } = new ViewModelTransportHistory();
        public string InProcessCODE                         //仕掛在庫CODE
        {
            get => inProcessCODE;
            set => SetProperty(ref inProcessCODE, value);
        }
        public string TransportDate                         //作業日
        {
            get => transportDate;
            set 
            {
                SetProperty(ref transportDate, value);
                DiaplayList();
            }
        }
        public string HeaderUnit                            //コイル・枚数
        {
            get => headerUnit;
            set => SetProperty(ref headerUnit, value);
        }
        public string HeaderWeight                          //焼結重量・単重
        {
            get => headerWeight;
            set => SetProperty(ref headerWeight, value);
        }
        public string HeaderAmount                          //ヘッダー（重量・数量）
        {
            get => headerAmount;
            set => SetProperty(ref headerAmount, value);
        }
        public bool VisibleShape                            //表示・非表示（形状）
        {
            get => visibleShape;
            set => SetProperty(ref visibleShape, value);
        }
        public bool VisibleUnit                             //表示・非表示（コイル・枚数）
        {
            get => visibleUnit;
            set => SetProperty(ref visibleUnit, value);
        }
        public bool VisibleWeight                           //表示・非表示（重量）
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
        internal ViewModelTransportHistory()
        {
            Instance = this;
            Initialize();
        }

        //ロード時
        private void OnLoad()
        {
            CtrlWindow.Interface = this;
            ReadINI();
            DisplayCapution();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            CtrlWindow.ProcessWork = "合板引取履歴";
            CtrlWindow.VisiblePower = true;
            CtrlWindow.VisibleList = true;
            CtrlWindow.VisibleInfo = false;
            CtrlWindow.VisibleDefect = false;
            CtrlWindow.VisibleArrow = true;
            CtrlWindow.VisiblePlan = true;
            CtrlWindow.InitializeIcon();
            CtrlWindow.IconList = "ViewList";
            CtrlWindow.IconPlan = "TrayArrowUp";
            CtrlWindow.ProcessName = ProcessName;
            DataGridBehavior.Instance.Iselect = this;           
        }

        //初期化
        public void Initialize()
        {
            SelectedIndex = -1;
            InProcessCODE = string.Empty;
            TransportDate = SetToDay(DateTime.Now).ToStringDateDB();
        }

        //一覧表示
        private void DiaplayList()
        {
            var inProcess = new InProcess();
            SelectTable = inProcess.SelectListTransportHistory("合板","プレス",TransportDate);           
        }

        //選択処理
        public async void SelectList()
        {
            if(SelectedItem == null) { return; }
            TransportInfo.InProcessCODE = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            DisplayFramePage(new TransportInfo());
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    KeyDown("DiaplayPlan");
                    break;
            }
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayList":
                    //引取履歴
                    Initialize();
                    DiaplayList();
                    break;

                case "DisplayPlan":
                    //仕掛置場
                    DisplayFramePage(new TransportList());
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
    }
}
