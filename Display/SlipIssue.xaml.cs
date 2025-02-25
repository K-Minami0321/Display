﻿using ClassBase;
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
    public class ViewModelSlipIssue : Common, IKeyDown, ISelect
    {
        //変数
        ViewModelWindowMain windowMain = ViewModelWindowMain.Instance;
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
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //ロード時
        private void OnLoad()
        {
            ReadINI();
            DisplayCapution();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            windowMain.VisiblePower = true;
            windowMain.VisiblePlan = true;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = false;
            windowMain.InitializeIcon();
            windowMain.ProcessName = ProcessName;
            windowMain.ProcessWork = ProcessName + "現品票発行";
            windowMain.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;

            //遷移ページ設定
            switch (Page)
            {
                case "PlanList":
                    //計画一覧
                    windowMain.VisibleList = false;
                    windowMain.VisibleInfo = false;
                    EnableSelect = false;
                    break;

                default:
                    windowMain.VisibleList = true;
                    windowMain.VisibleInfo = true;
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
