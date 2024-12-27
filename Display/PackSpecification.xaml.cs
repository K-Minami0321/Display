using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using NPOI.Util.Collections;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class PackSpecification : UserControl
    {
        public PackSpecification()
        {
            DataContext = new ViewModelPackSpecification();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelPackSpecification : Common
    {
        //変数
        DataTable table;
        string containerCategory;
        string container;
        string carton;
        string palette;
        string containerComment;
        string imageSource1;
        string imageSource2;
        int lengthProductName = 20;
        bool visibileButton = true;
        bool focusLotProductName = false;

        //プロパティ
        public DataTable Table          //データテーブル
        {
            get { return table; }
            set { SetProperty(ref table, value); }
        }
        public string ContainerCategory //段ボール・ポリ箱
        {
            get { return containerCategory; }
            set { SetProperty(ref containerCategory, value); }
        }
        public string Container         //段ボール・ポリ箱 詳細
        {
            get { return container; }
            set { SetProperty(ref container, value); }
        }
        public string Carton            //詰数
        {
            get { return carton; }
            set { SetProperty(ref carton, value); }
        }
        public string Palette           //パレット
        {
            get { return palette; }
            set { SetProperty(ref palette, value); }
        }
        public string ContainerComment  //備考
        {
            get { return containerComment; }
            set { SetProperty(ref containerComment, value); }
        }
        public string ImageSource1      //画像1
        {
            get { return imageSource1; }
            set { SetProperty(ref imageSource1, value); }
        }
        public string ImageSource2      //画像2
        {
            get { return imageSource2; }
            set { SetProperty(ref imageSource2, value); }
        }
        public int LengthProductName    //文字数（品番）
        {
            get { return lengthProductName; }
            set { SetProperty(ref lengthProductName, value); }
        }
        public bool VisibileButton      //ボタン表示
        {
            get { return visibileButton; }
            set { SetProperty(ref visibileButton, value); }
        }
        public bool FocusProductName      //フォーカス（ロット番号）
        {
            get { return focusLotProductName; }
            set { SetProperty(ref focusLotProductName, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand lostFocus;
        public ICommand LostFocus => lostFocus ??= new ActionCommand(SetLostFocus);

        //コンストラクター
        public ViewModelPackSpecification()
        {
            ProductName = string.Empty;
            Product product = new Product(ProductName);
            CopyProperty(product, this);
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            FocusProductName = true;
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            ViewModelWindowMain windowMain = ViewModelWindowMain.Instance;
            windowMain.VisiblePower = true;
            windowMain.VisibleList = false;
            windowMain.VisibleInfo = false;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = false;
            windowMain.VisiblePlan = false;
            windowMain.InitializeIcon();
            windowMain.ProcessWork = "梱包仕様書";
            windowMain.ProcessName = "梱包";
        }

        //キーイベント
        public async void KeyDown(object value)
        {
            var result = false;
            switch (value)
            {
                case "ContainerCategory1":
                    DisplayPackSpecification(ProductName, "1");
                    break;

                case "ContainerCategory2":
                    DisplayPackSpecification(ProductName, "2");
                    break;
            }
        }

        //フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            FocusProductName = false;
            DisplayPackSpecification(ProductName);
        }

        //梱包仕様表示
        private void DisplayPackSpecification(string code, string no = "")
        {
            //データ取得
            Product product = new Product();
            Table = product.SelectPaking(code, no);          
            CopyProperty(product, this);

            if (string.IsNullOrEmpty(no)) { VisibileButton = !(product.DataCount == 1); }
            ProductName = code.ToUpper();
            if (product.DataCount > 0) { ContainerCategory = ContainerCategory == "P" ? "ポリ箱" : "段ボール"; }

            Container = STRING.ToTrim(Container.Replace("P", "").Replace("D", ""));





        }
    }
}
