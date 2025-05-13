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
            DataContext = new ViewModelPlanList();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelPlanList : Common, IWindowBase, ISelect
    {
        //変数
        Plan plan;
        string lotNumber;
        string updateDate;
        string file;
        bool visibleUnit;
        bool visibleAmount;

        //プロパティ
        public static ViewModelPlanList Instance    //インスタンス
        { get; set; } = new ViewModelPlanList();
        public string LotNumber                     //ロット番号
        {
            get => lotNumber; 
            set => SetProperty(ref lotNumber, value); 
        }
        public string UpdateDate                    //更新表示
        {
            get => updateDate;
            set => SetProperty(ref updateDate, value);
        }
        public string File                          //ファイル名
        {
            get => file;
            set => SetProperty(ref file, value); 
        }
        public bool VisibleUnit                     //表示・非表示（コイル・シート絞り込み）
        {
            get => visibleUnit; 
            set => SetProperty(ref visibleUnit, value); 
        }
        public bool VisibleAmount                   //表示・非表示（完了数）
        {
            get => visibleAmount; 
            set => SetProperty(ref visibleAmount, value); 
        }

        //イベント
        ActionCommand? commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand? commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        internal ViewModelPlanList()
        {
            plan = new();

            ReadINI();
            SelectedIndex = -1;
            LotNumber = string.Empty;
            VisibleUnit = ProcessName == "合板" ? true : false;
            VisibleAmount = !VisibleUnit;
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
                VisiblePlan = true,
                VisibleDefect = false,
                VisibleArrow = false,
                VisibleList = true,
                VisibleInfo = true,
                VisiblePrinter = false,
                VisibleQRcode = false,
                Process = ProcessName,
                ProcessWork = ProcessName + "計画一覧"
            };
            Iselect = this;
        }

        //一覧表示
        private void DiaplayList(string where = "")
        {
            var selectIndex = SelectedIndex;
            UpdateDate = plan.SelectFile(ProcessName) + "版";
            SelectTable = plan.SelectPlanList(ProcessName, where, true);

            //行選択・スクロール設定
            //DataGridBehavior.Instance.SetScrollViewer();
            //DataGridBehavior.Instance.Scroll.ScrollToVerticalOffset(ScrollIndex);
            SelectedIndex = selectIndex;
        }

        //選択処理
        public void SelectList()
        {
            if (SelectedItem == null) { return; }
            if (SelectedItem.Row.ItemArray[15].ToString() == "完了") { return; }

            //ロット番号設定
            LotNumber = DATATABLE.SelectedRowsItem(SelectedItem, "ロット番号");
            ChangePage();
        }

        //ページ遷移
        private void ChangePage()
        {
            StartPage(Page, string.Empty, LotNumber);
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

                    //登録画面
                    ChangePage();
                    break;

                case "DisplayList":

                    //一覧画面
                    Page = Page.Replace("Info", "List");
                    ChangePage();
                    break;

                case "DisplayPlan":

                    //計画一覧画面
                    SelectedIndex = -1;
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
    }
}
