﻿using ClassBase;
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
        public ManufactureList(string date)
        {
            DataContext = new ViewModelManufactureList(date);
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelManufactureList : Common, IWindowBase, ISelect
    {
        //変数
        Manufacture manufacture = new Manufacture();
        string manufactureCODE;
        string manufactureDate;

        //プロパティ
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
        internal ViewModelManufactureList(string date)
        {
            Iselect = this;
            ReadINI();

            SelectedIndex = -1;
            ManufactureDate = date.ToStringDateDB();
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            DiaplayList();
        }

        //コントロールの設定
        private void DisplayCapution()
        {
            //WindowMain
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                VisiblePower = true,
                VisibleList = true,
                VisibleInfo = true,
                VisibleDefect = false,
                VisibleArrow = true,
                VisiblePrinter = false,
                VisibleQRcode = false,
                Process = ProcessName,
                ProcessWork = "作業実績"
            };
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectTable = manufacture.SelectHistoryListDate(ProcessName, ManufactureDate);
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem == null) { return; }
            var code = DATATABLE.SelectedRowsItem(SelectedItem, "製造CODE");
            DisplayFramePage(new ManufactureInfo(code, string.Empty));
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
                    DisplayFramePage(new ManufactureInfo(string.Empty, string.Empty));
                    break;

                case "DisplayList":

                    //搬入一覧画面
                    SelectedIndex = -1;
                    ManufactureDate = DateTime.Now.ToString("yyyyMMdd");
                    DisplayFramePage(new ManufactureList(ManufactureDate));
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
