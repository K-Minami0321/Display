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
    public partial class TransportList : UserControl
    {
        //コンストラクター
        public TransportList()
        {
            DataContext = new ViewModelTransportList();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportList : Common, IWindowBase, ISelect
    {
        //変数
        InProcess inProcess;
        string processName = string.Empty;
        string inProcessCODE = string.Empty;
        string transportDate = string.Empty;
        string headerUnit = string.Empty;
        string headerWeight = string.Empty;
        string headerAmount = string.Empty;
        bool visibleShape;
        bool visibleUnit;
        bool visibleWeight;

        //プロパティ
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
        ActionCommand? commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand? commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        internal ViewModelTransportList()
        {
            inProcess = new();
            
            ReadINI();
            SelectedIndex = -1;
            TransportDate = SetToDay(DateTime.Now).ToStringDateDB();
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
            WindowProperty = new()
            {
                IwindowBase = this,
                ProcessWork = "引取履歴",
                VisibleList = true,
                VisibleInfo = true,
                VisibleDefect = false,
                VisibleArrow = true,
                VisiblePlan = false,
                VisiblePrinter = false,
                VisibleQRcode = false,
                IconList = "ViewList"
            };
            Iselect = this;
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectTable = inProcess.SelectListTransportHistory("合板","プレス",TransportDate);           
        }

        //選択処理
        public async void SelectList() { return; }

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
                    SelectedIndex = -1;
                    TransportDate = SetToDay(DateTime.Now).ToStringDateDB();

                    DiaplayList();
                    break;

                case "DisplayInfo":

                    //引取登録
                    DisplayFramePage(new Transport());
                    break;

                case "PreviousDate":

                    //前日へ移動
                    TransportDate = DATETIME.AddDate(TransportDate, -1).ToString("yyyyMMdd");
                    SelectedIndex = 0;
                    ScrollIndex = 0;
                    break;

                case "NextDate":

                    //次の日へ移動
                    TransportDate = DATETIME.AddDate(TransportDate, 1).ToString("yyyyMMdd");
                    SelectedIndex = 0;
                    ScrollIndex = 0;
                    break;
            }
        }
    }
}
