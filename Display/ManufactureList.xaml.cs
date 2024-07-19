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
        string equipmentCODE;
        string processName;
        string manufactureCODE;
        string manufactureDate;

        //プロパティ
        public static ViewModelManufactureList Instance     //インスタンス
        { get; set; } = new ViewModelManufactureList();
        public string EquipmentCODE                         //設備CODE
        {
            get { return equipmentCODE; }
            set
            {
                equipmentCODE = value;

                equipment.EquipmentCODE = EquipmentCODE;
                equipment.Select();

                var name = equipment.EquipmentName;
                if (!string.IsNullOrEmpty(name))
                {
                    name = name + " - " + EquipmentCODE;
                }
                else
                {
                    name = iProcess.Name;
                }
                ViewModelWindowMain.Instance.ProcessWork = name;
            }
        }
        public string ProcessName                           //工程区分
        {
            get { return processName; }
            set 
            { 
                SetProperty(ref processName, value);
                iProcess = ProcessCategory.SetProcess(value);
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

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelManufactureList()
        {
            equipment = new Equipment();
            manufacture = new Manufacture();
            ManufactureDate = SetToDay(DateTime.Now);
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
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
            Initialize();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            EquipmentCODE = INI.GetString("Page", "Equipment");

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList = "refresh";
        }

        //初期化
        private void Initialize()
        {
            //初期設定
            ManufactureCODE = string.Empty;
            SelectedIndex = -1;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //搬入登録画面
                    ViewModelWindowMain.Instance.FramePage = new ManufactureInfo();
                    break;

                case "DisplayList":
                    //搬入一覧画面
                    ManufactureDate = DateTime.Now.ToString("yyyyMMdd");
                    ViewModelWindowMain.Instance.FramePage = new ManufactureList();
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.FramePage = new PlanList();
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
            manufacture.ManufactureDate = ManufactureDate;
            manufacture.Equipment1 = EquipmentCODE;
            if (iProcess == null) { return; }
            SelectTable = manufacture.SelectHistoryListDate(iProcess.Name);
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem == null) { return; }
            ManufactureCODE = DATATABLE.SelectedRowsItem(SelectedItem, "製造CODE");
            ViewModelWindowMain.Instance.FramePage = new ManufactureInfo();
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
