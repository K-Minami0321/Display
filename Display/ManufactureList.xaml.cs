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
    public partial class ManufactureList : UserControl
    {
        public ManufactureList()
        {
            DataContext = ViewModelManufactureList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelManufactureList : Common, IWindowBase, ISelect
    {
        //変数
        string manufactureCODE;
        string manufactureDate;

        //プロパティ
        public static ViewModelManufactureList Instance     //インスタンス
        { get; set; } = new ViewModelManufactureList();
        public string ManufactureCODE                       //加工CODE
        {
            get => manufactureCODE;
            set => SetProperty(ref manufactureCODE, value);
        }
        public string ManufactureDate                       //作業日
        {
            get { return manufactureDate; }
            set 
            { 
                SetProperty(ref manufactureDate, value);
                DiaplayList();
            }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelManufactureList()
        {
            Instance = this;
            Initialize();
        }

        //ロード時
        private void OnLoad()
        {
            CtrlWindow.IwindowBase = this;

            SetControl();
            ReadINI();
            DiaplayList();
        }

        //初期化
        private void Initialize()
        {
            SelectedIndex = -1;
            ManufactureCODE = string.Empty;
            ManufactureDate = DateTime.Now.ToString("yyyyMMdd");
        }

        //コントロールの設定
        private void SetControl()
        {
            //WindowMain
            WindowProperty = new PropertyWindow()
            {
                VisiblePower = true,
                VisibleList = true,
                VisibleInfo = true,
                VisibleDefect = false,
                VisibleArrow = true,
                ProcessName = ProcessName,
                ProcessWork = ProcessWork
            };

            DataGridBehavior.Instance.Iselect = this;
        }

        //一覧表示
        private void DiaplayList()
        {
            var manufacture = new Manufacture();
            SelectTable = manufacture.SelectHistoryListDate(ProcessName, ManufactureDate);
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem == null) { return; }
            ManufactureInfo.ManufactureCODE = DATATABLE.SelectedRowsItem(SelectedItem, "製造CODE");
            DisplayFramePage(new ManufactureInfo());
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
                    DisplayFramePage(new ManufactureInfo());
                    break;

                case "DisplayList":
                    //搬入一覧画面
                    ManufactureDate = DateTime.Now.ToString("yyyyMMdd");
                    DisplayFramePage(new ManufactureList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    DisplayFramePage(new PlanList());
                    break;

                case "PreviousDate":
                    //前日へ移動
                    ManufactureDate = DATETIME.AddDate(ManufactureDate, -1).ToString("yyyyMMdd");
                    break;

                case "NextDate":
                    //次の日へ移動
                    ManufactureDate = DATETIME.AddDate(ManufactureDate, 1).ToString("yyyyMMdd");
                    break;

                case "Today":
                    //当日へ移動
                    ManufactureDate = DateTime.Now.ToString("yyyyMMdd");
                    break;
            }
        }
    }
}
