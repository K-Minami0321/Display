using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //プロパティ
        public string ShippingDate              //出荷日
        {
            get { return shippingDate; }
            set
            {
                SetProperty(ref shippingDate, value);
                DiaplayList();
            }
        }

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
            ShippingDate = DateTime.Now.ToString("yyyyMMdd");

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
                ProcessWork = "出荷一覧"
            };
        }

        //一覧表示
        private void DiaplayList(string where = "")
        {
            SelectTable = shippingStock.ShippingList(ShippingDate);
        }

        //選択処理
        public void SelectList() { return; }

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
