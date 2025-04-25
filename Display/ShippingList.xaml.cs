using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class ShippingList : UserControl
    {
        public ShippingList()
        {
            DataContext = new ViewModelShippingList();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelShippingList : Common, IWindowBase, ISelect
    {
        //変数
        ShippingStock shippingStock = new ShippingStock();
        string shippingDate;
        List<string> customers;

        //プロパティ
        public string ShippingDate              //出荷日
        {
            get=> shippingDate;
            set
            {
                SetProperty(ref shippingDate, value);
                DiaplayList();
            }
        }
        public List<string> Customers           //取引先
        {
            get => customers;
            set => SetProperty(ref customers, value);
        }
        public static string CacheDate          //キャッシュ（対象日）
        { get; set; }
        public static int CacheSelectedIndex    //キャッシュ（選択行）
        { get; set; }
        public static double CacheScrollIndex   //キャッシュ（スクロール位置）
        { get; set; }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand commandCheck;
        public ICommand CommandCheck => commandCheck ??= new ActionCommand(OnCheck);

        //コンストラクター
        public ViewModelShippingList()
        {
            Iselect = this;

            ReadINI();
            ShippingDate = CacheDate ?? DateTime.Now.ToString("yyyyMMdd");
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            StateLoad();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                VisiblePlan = false,
                VisibleDefect = false,
                VisibleArrow = true,
                VisibleList = true,
                VisibleInfo = false,
                VisiblePrinter = false,
                VisibleQRcode = false,
                ProcessWork = "出庫一覧"
            };
        }

        //状態読込
        private void StateLoad()
        {
            SelectedIndex = CacheSelectedIndex;
            ScrollIndex = CacheScrollIndex;
        }

        //状態保存
        private void StateSave()
        {
            CacheDate = ShippingDate;
            CacheSelectedIndex = SelectedIndex;
            CacheScrollIndex = ScrollIndex;
        }

        //一覧表示
        private void DiaplayList(string where = "")
        {
            SelectTable = shippingStock.ShippingList(ShippingDate);
        }

        //選択処理
        public void SelectList() 
        {
            if (SelectedItem == null) { return; }
            StateSave();
            DisplayFramePage(new PackSpecification(DATATABLE.SelectedRowsItem(SelectedItem, "品番"), DATATABLE.SelectedRowsItem(SelectedItem, "顧客部品番号")));
        }

        //チェック処理
        private void OnCheck()
        {
            foreach (DataRow datarow in SelectTable.Rows)
            {
                shippingStock.ProcessInspection(datarow["受注CODE"].ToTrim(), datarow["検品済"].ToTrim());
            }
        }

        //スワイプ処理
        public void Swipe(object value) { return; }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "PreviousDate":

                    //前日へ移動
                    ShippingDate = DATETIME.AddDate(ShippingDate, -1).ToString("yyyyMMdd");
                    SelectedIndex = 0;
                    ScrollIndex = 0;
                    break;

                case "NextDate":

                    //次の日へ移動
                    ShippingDate = DATETIME.AddDate(ShippingDate, 1).ToString("yyyyMMdd");
                    SelectedIndex = 0;
                    ScrollIndex = 0;
                    break;
            }
            StateSave();
        }
    }
}
