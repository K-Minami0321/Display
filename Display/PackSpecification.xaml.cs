using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
    public class ViewModelPackSpecification : Common, IKeyDown
    {
        //変数
        DataTable selectTable;
        DataTable listTable;
        string containerCategory;
        string container;
        string carton;
        string palette;
        string containerComment;
        string imageSource1;
        string imageSource2;
        int lengthProductName = 20;
        ObservableCollection<string> nameButton = new ObservableCollection<string> { "ポリ箱", "段ボール", "段ボール" };
        ObservableCollection<string> iconButton = new ObservableCollection<string> { "Package", "PackageVariant", "PackageVariant" };
        bool visibileButton1 = true;
        bool visibileButton2 = false;
        bool focusLotProductName = false;

        //プロパティ
        public DataTable SelectTable                        //データテーブル
        {
            get { return selectTable; }
            set { SetProperty(ref selectTable, value); }
        }
        public DataTable ListTable                          //一覧テーブル
        { 
            get { return listTable; }
            set { SetProperty(ref listTable, value); }
        }
        public string ContainerCategory                     //段ボール・ポリ箱
        {
            get { return containerCategory; }
            set { SetProperty(ref containerCategory, value); }
        }
        public string Container                             //段ボール・ポリ箱 詳細
        {
            get { return container; }
            set { SetProperty(ref container, value); }
        }
        public string Carton                                //詰数
        {
            get { return carton; }
            set { SetProperty(ref carton, value); }
        }
        public string Palette                               //パレット
        {
            get { return palette; }
            set { SetProperty(ref palette, value); }
        }
        public string ContainerComment                      //備考
        {
            get { return containerComment; }
            set { SetProperty(ref containerComment, value); }
        }
        public string ImageSource1                          //画像1
        {
            get { return imageSource1; }
            set { SetProperty(ref imageSource1, value); }
        }
        public string ImageSource2                          //画像2
        {
            get { return imageSource2; }
            set { SetProperty(ref imageSource2, value); }
        }
        public int LengthProductName                        //文字数（品番）
        {
            get { return lengthProductName; }
            set { SetProperty(ref lengthProductName, value); }
        }
        public ObservableCollection<string> NameButton      //ボタン名称
        {
            get { return nameButton; }
            set { SetProperty(ref nameButton, value); }
        }
        public ObservableCollection<string> IconButton      //ボタンアイコン
        {
            get { return iconButton; }
            set { SetProperty(ref iconButton, value); }
        }
        public bool VisibileButton1                         //ボタン表示
        {
            get { return visibileButton1; }
            set { SetProperty(ref visibileButton1, value); }
        }
        public bool VisibileButton2                         //ボタン表示
        {
            get { return visibileButton2; }
            set { SetProperty(ref visibileButton2, value); }
        }
        public bool FocusProductName                        //フォーカス（ロット番号）
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
            SelectTable = product.SelectPakingIndex();
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
            windowMain.VisibleArrow = true;
            windowMain.VisiblePlan = false;
            windowMain.Ikeydown = this;
            windowMain.InitializeIcon();
            windowMain.ProcessWork = "梱包仕様書";
            windowMain.ProcessName = "梱包";
        }

        //キーイベント
        public async void KeyDown(object value)
        {
            switch (value)
            {
                case "ContainerCategory1":
                    DisplayPackSpecification(ProductName, "1");
                    break;

                case "ContainerCategory2":
                    DisplayPackSpecification(ProductName, "2");
                    break;

                case "ContainerCategory3":
                    DisplayPackSpecification(ProductName, "3");
                    break;

                case "PreviousDate":
                    //前のデータ
                    SetAroundCode("Back");
                    break;

                case "NextDate":
                    //次のデータ
                    SetAroundCode("Forword");
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
            ListTable = product.SelectPaking(code, no);
            CopyProperty(product, this);

            //表示
            ProductName = code;
            if (product.DataCount > 0) { ContainerCategory = SetName(ContainerCategory); }
            Container = STRING.ToTrim(Container.Replace("P", "").Replace("D", ""));

            if (!string.IsNullOrEmpty(code))
            {
                //ボタン制御
                VisibileButton1 = product.DataCount >= 2;
                VisibileButton2 = product.DataCount == 3;

                var count = 0;
                foreach (DataRow datarow in ListTable.Rows)
                {
                    NameButton[count] = SetName(STRING.ToTrim(datarow["容器"]));
                    IconButton[count] = SetIcon(STRING.ToTrim(datarow["容器"]));
                    count++;
                }

                //Index設定
                var querry = from a in SelectTable.AsEnumerable()
                             where a.Field<string>("品番") == code
                             select new { 連番 = a.Field<Int64>("連番") };
                IndexNumber = INT.ToInt(querry.FirstOrDefault().連番);
            }

            string SetName(string value) => value == "P" ? "ポリ箱" : "段ボール";
            string SetIcon(string value) => value == "P" ? "Package" : "PackageVariant";
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    SetAroundCode("Back");
                    break;

                case "Left":
                    SetAroundCode("Forword");
                    break;
            }
        }

        //前後のコード取得
        private void SetAroundCode(string value)
        {
            switch (value)
            {
                case "Back":
                    if (IndexNumber < SelectTable.Rows.Count) { IndexNumber = IndexNumber + 1; }
                    break;
                case "Forword":
                    IndexNumber = IndexNumber - 1;
                    break;
            }

            //CODEの設定
            if (IndexNumber < 0) { IndexNumber = 0; }
            DataRow dr = SelectTable.Rows[IndexNumber];

            var code = dr["品番"].ToString();
            DisplayPackSpecification(dr["品番"].ToString());
        }
    }
}
