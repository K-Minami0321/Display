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
    public class ViewModelManufactureList : Common, IKeyDown, ISelect
    {
        //変数
        ViewModelWindowMain windowMain;
        DataGridBehavior dataGridBehavior;
        string processName;
        string manufactureCODE;
        string manufactureDate;
        string equipmentCODE;

        //プロパティ
        public static ViewModelManufactureList Instance     //インスタンス
        { get; set; } = new ViewModelManufactureList();
        public string ProcessName                           //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);
                process = new ProcessCategory(value);
            }
        }
        public string ManufactureCODE                       //加工CODE
        {
            get { return manufactureCODE; }
            set { SetProperty(ref manufactureCODE, value); }
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
        public string EquipmentCODE                         //設備CODE
        {
            get { return equipmentCODE; }
            set
            {
                equipment = new Equipment();
                var name = equipment.EquipmentName;
                windowMain.ProcessWork = string.IsNullOrEmpty(name) ? process.Name + "実績" : name + " - " + value;
            }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelManufactureList()
        {
            manufacture = new Manufacture();

            //デフォルト値設定
            ManufactureDate = DateTime.Now.ToString("yyyyMMdd");
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
            Initialize();
            windowMain.VisiblePower = true;
            windowMain.VisibleList = true;
            windowMain.VisibleInfo = true;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = true;
            windowMain.InitializeIcon();
            DiaplayList();
        }

        //初期化
        private void Initialize()
        {
            ProcessName = IniFile.GetString("Page", "Process");
            EquipmentCODE = IniFile.GetString("Page", "Equipment");
            ManufactureCODE = string.Empty;

            ManufactureInfo.ManufactureCODE = null;
            ManufactureInfo.LotNumber = null;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //搬入登録画面
                    windowMain.FramePage = new ManufactureInfo();
                    break;

                case "DisplayList":
                    //搬入一覧画面
                    ManufactureDate = DateTime.Now.ToString("yyyyMMdd");
                    windowMain.FramePage = new ManufactureList();
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    windowMain.FramePage = new PlanList();
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

        //一覧表示
        private void DiaplayList()
        {
            if (process == null) { return; }
            SelectTable = manufacture.SelectHistoryListDate(process.Name, ManufactureDate);
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem == null) { return; }
            ManufactureInfo.ManufactureCODE = DATATABLE.SelectedRowsItem(SelectedItem, "製造CODE");
            windowMain.FramePage = new ManufactureInfo();
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
