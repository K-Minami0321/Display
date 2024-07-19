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
        string lotNumber;
        string processName;
        string inProcessCODE;
        string updateDate;
        string file;
        bool visibleUnit;
        bool visibleAmount;
        bool enableSelect;

        //プロパティ
        public static ViewModelPlanList Instance            //インスタンス
        { get; set; } = new ViewModelPlanList();
        public string LotNumber       //ロット番号
        {
            get { return lotNumber; }
            set { SetProperty(ref lotNumber, value); }
        }
        public string ProcessName     //工程区分
        {
            get { return plan.ProcessName; }
            set 
            { 
                SetProperty(ref processName, value);
                plan.ProcessName = value;
                iProcess = ProcessCategory.SetProcess(value);
            }
        }
        public string InProcessCODE   //搬入CODE
        {
            get { return plan.InProcessCODE; }
            set 
            { 
                SetProperty(ref inProcessCODE, value);
                plan.InProcessCODE = value;
            }
        }
        public string UpdateDate      //更新表示
        {
            get { return updateDate; }
            set { SetProperty(ref updateDate, value); }
        }
        public string File            //ファイル名
        {
            get { return file; }
            set { SetProperty(ref file, value); }
        }
        public bool VisibleUnit       //表示・非表示（コイル・シート絞り込み）
        {
            get { return visibleUnit; }
            set { SetProperty(ref visibleUnit, value); }
        }
        public bool VisibleAmount     //表示・非表示（完了数）
        {
            get { return visibleAmount; }
            set { SetProperty(ref visibleAmount, value); }
        }
        public bool EnableSelect      //選択可能
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
            plan = new Plan();
            SelectedIndex = -1;
            ScrollIndex = 0;
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;
            ViewModelWindowMain.Instance.ProcessName = INI.GetString("Page", "Process");

            //初期設定
            Initialize();
            DiaplayList();
        }

        //初期化
        private void Initialize()
        {
            //キャプション表示
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            ViewModelWindowMain.Instance.ProcessWork = ProcessName + "計画一覧";
            UpdateDate = plan.SelectFile() + "版";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconPlan = "refresh";
            ViewModelWindowMain.Instance.IconSize = 35;

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

            //表示・非表示
            VisibleUnit = ProcessName == "合板" ? true : false;
            VisibleAmount = !VisibleUnit;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //作業登録画面
                    LotNumber = null;
                    ViewModelWindowMain.Instance.FramePage = new InProcessInfo();
                    break;

                case "DisplayList":
                    //仕掛在庫一覧画面
                    ViewModelWindowMain.Instance.FramePage = new InProcessList();
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
            if (SelectedIndex == null) { return; }
            if (SelectedItem.Row.ItemArray[14].ToString() == "完了") { return; }
            LotNumber = DATATABLE.SelectedRowsItem(SelectedItem, "ロット番号");
            if (EnableSelect) { StartPage(); }
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
