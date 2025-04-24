using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
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
        public string ShippingDate                  //出荷日
        {
            get=> shippingDate;
            set
            {
                SetProperty(ref shippingDate, value);
                DiaplayList();
            }
        }
        public List<string> Customers               //取引先
        {
            get => customers;
            set => SetProperty(ref customers, value);
        }
        public static string CacheShippingDate      //キャッシュ（対象日）
        { get; set; }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        public ViewModelShippingList()
        {
            Iselect = this;
            ReadINI();

            SelectedIndex = -1;
            StateLoad();
            ListSource.Date = ShippingDate;
            Customers = ListSource.ShippingCustomers;
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
            //WindowMain
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                VisiblePlan = false,
                VisibleDefect = false,
                VisibleArrow = true,
                VisibleList = false,
                VisibleInfo = false,
                VisiblePrinter = false,
                VisibleQRcode = false,
                ProcessWork = "出庫一覧"
            };
        }

        //状態読込
        private void StateLoad()
        {
            ShippingDate = CacheShippingDate ?? DateTime.Now.ToString("yyyyMMdd");



        }

        //状態保存
        private void StateSave()
        {
            CacheShippingDate = ShippingDate;



        }

        //一覧表示
        private void DiaplayList(string where = "")
        {
            SelectTable = shippingStock.ShippingList(ShippingDate);
            ListSource.Date = ShippingDate;
            Customers = ListSource.ShippingCustomers;
        }

        //選択処理
        public void SelectList() 
        {
            if (SelectedItem == null) { return; }
            var code = DATATABLE.SelectedRowsItem(SelectedItem, "品番");
            StateSave();
            DisplayFramePage(new PackSpecification(code));
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
                    break;

                case "NextDate":

                    //次の日へ移動
                    ShippingDate = DATETIME.AddDate(ShippingDate, 1).ToString("yyyyMMdd");
                    break;
            }
        }
    }
}
