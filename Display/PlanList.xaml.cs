using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class PlanList : UserControl
    {
        public PlanList()
        {
            DataContext = ViewModelPlanList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelPlanList : Common, IKeyDown, ISelect
    {
        //変数
        string processName;
        string lotNumber;
        string updateDate;
        string file;
        bool visibleUnit;
        bool visibleAmount;
        bool enableSelect;

        //プロパティ
        public static ViewModelPlanList Instance    //インスタンス
        { get; set; } = new ViewModelPlanList();
        public string ProcessName                   //工程区分
        {
            get { return plan.ProcessName; }
            set
            {
                SetProperty(ref processName, value);
                plan.ProcessName = value;
                iProcess = ProcessCategory.SetProcess(value);
            }
        }
        public string LotNumber                     //ロット番号
        {
            get { return lotNumber; }
            set { SetProperty(ref lotNumber, value); }
        }
        public string UpdateDate                    //更新表示
        {
            get { return updateDate; }
            set { SetProperty(ref updateDate, value); }
        }
        public string File                          //ファイル名
        {
            get { return file; }
            set { SetProperty(ref file, value); }
        }
        public bool VisibleUnit                     //表示・非表示（コイル・シート絞り込み）
        {
            get { return visibleUnit; }
            set { SetProperty(ref visibleUnit, value); }
        }
        public bool VisibleAmount                   //表示・非表示（完了数）
        {
            get { return visibleAmount; }
            set { SetProperty(ref visibleAmount, value); }
        }
        public bool EnableSelect                    //選択可能
        {
            get { return enableSelect; }
            set { SetProperty(ref enableSelect, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        internal ViewModelPlanList()
        {
            Instance = this;
            plan = new Plan();

            //デフォルト値設定
            SelectedIndex = -1;
            ScrollIndex = 0;
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
            //ボタン設定
            Initialize();
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconPlan = "refresh";
            ViewModelWindowMain.Instance.IconSize = 35;
            ViewModelWindowMain.Instance.ProcessWork = ProcessName + "計画一覧";
            DiaplayList();
        }

        //初期化
        private void Initialize()
        {
            //初期化
            LotNumber = string.Empty;
            ProcessName = INI.GetString("Page", "Process");
            ViewModelManufactureList.Instance.ManufactureCODE = string.Empty;
            ViewModelInProcessList.Instance.InProcessCODE = string.Empty;
            UpdateDate = plan.SelectFile() + "版";
            VisibleUnit = ProcessName == "合板" ? true : false;
            VisibleAmount = !VisibleUnit;

            //画面設定
            switch (INI.GetString("Page", "Initial"))
            {
                case "PlanList":
                    //計画一覧
                    ViewModelWindowMain.Instance.VisibleList = false;
                    ViewModelWindowMain.Instance.VisibleInfo = false;
                    EnableSelect = false;
                    break;

                default:
                    ViewModelWindowMain.Instance.VisibleList = true;
                    ViewModelWindowMain.Instance.VisibleInfo = true;
                    EnableSelect = true;
                    break;
            }
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //登録画面
                    StartPage(INI.GetString("Page", "Initial"));
                    break;

                case "DisplayList":
                    //一覧画面
                    StartPage(INI.GetString("Page", "Initial").Replace("Info", "List"));
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    SelectedIndex = -1;
                    ScrollIndex = 0;
                    plan.SelectFile();
                    DiaplayList();
                    break;

                case "Sheet":
                    DiaplayList("シート");
                    break;

                case "Coil":
                    DiaplayList("コイル");
                    break;
            }
        }

        //一覧表示
        private void DiaplayList(string where = "")
        {
            var selectIndex = SelectedIndex;
            plan.SelectFile();
            SelectTable = plan.SelectPlanList(where, true);

            //行選択・スクロール設定
            DataGridBehavior.Instance.SetScrollViewer();
            DataGridBehavior.Instance.Scroll.ScrollToVerticalOffset(ScrollIndex);
            SelectedIndex = selectIndex;
        }

        //選択処理
        public void SelectList()
        {
            if (SelectedItem == null) { return; }
            if (SelectedItem.Row.ItemArray[15].ToString() == "完了") { return; }
            LotNumber = DATATABLE.SelectedRowsItem(SelectedItem, "ロット番号");
            if (EnableSelect) { StartPage(INI.GetString("Page", "Initial")); }
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
