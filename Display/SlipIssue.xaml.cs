using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class SlipIssue : UserControl
    {
        public SlipIssue()
        {
            DataContext = new ViewModelSlipIssue();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelSlipIssue : Common, IWindowBase, ISelect
    {
        //変数
        string lotNumber;
        string updateDate;
        string file;
        bool visibleUnit;
        bool visibleAmount;
        bool enableSelect;

        //プロパティ
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
        public bool EnableSelect                    //選択可能
        {
            get => enableSelect; 
            set => SetProperty(ref enableSelect, value); 
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;

        //ロード時
        private void OnLoad()
        {
            CtrlWindow.IwindowBase = this;
            ReadINI();
            DisplayCapution();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            CtrlWindow.VisiblePower = true;
            CtrlWindow.VisiblePlan = true;
            CtrlWindow.VisibleDefect = false;
            CtrlWindow.VisibleArrow = false;
            CtrlWindow.InitializeIcon();
            CtrlWindow.ProcessName = ProcessName;
            CtrlWindow.ProcessWork = ProcessName + "現品票発行";
            DataGridBehavior.Instance.Iselect = this;

            //遷移ページ設定
            switch (Page)
            {
                case "PlanList":
                    //計画一覧
                    CtrlWindow.VisibleList = false;
                    CtrlWindow.VisibleInfo = false;
                    EnableSelect = false;
                    break;

                default:
                    CtrlWindow.VisibleList = true;
                    CtrlWindow.VisibleInfo = true;
                    EnableSelect = true;
                    break;
            }
        }

        //一覧表示
        private void DiaplayList(string where = "")
        {
            var selectIndex = SelectedIndex;
            var plan = new Plan();
            SelectTable = plan.SelectPlanList(ProcessName, where, true);

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

            //ロット番号設定
            LotNumber = DATATABLE.SelectedRowsItem(SelectedItem, "ロット番号");
            ChangePage();
        }

        //ページ遷移
        private void ChangePage()
        {
            switch (Page)
            {
                case "InProcessInfo":
                    InProcessInfo.InProcessCODE = string.Empty;
                    InProcessInfo.LotNumber = LotNumber;
                    break;

                case "ManufactureInfo":

                    ManufactureInfo.ManufactureCODE = string.Empty;
                    ManufactureInfo.LotNumber = LotNumber;
                    break;
            }
            if (EnableSelect) { StartPage(Page); }
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":


                    break;
            }
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {



            }
        }
    }
}
