using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.ObjectModel;
using System.Data;
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
        DataTable selectTable;
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

                case "ContainerCategory3":
                    DisplayPackSpecification(ProductName, "3");
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
            SelectTable = product.SelectPaking(code, no);
            CopyProperty(product, this);

            //表示
            ProductName = code.ToUpper();
            if (product.DataCount > 0) { ContainerCategory = SetName(ContainerCategory); }
            Container = STRING.ToTrim(Container.Replace("P", "").Replace("D", ""));

            //ボタン制御
            if (!string.IsNullOrEmpty(code))
            {
                VisibileButton1 = product.DataCount >= 2;
                VisibileButton2 = product.DataCount == 3;

                var count = 0;
                foreach (DataRow datarow in SelectTable.Rows)
                {
                    NameButton[count] = SetName(STRING.ToTrim(datarow["容器"]));
                    IconButton[count] = SetIcon(STRING.ToTrim(datarow["容器"]));
                    count++;
                }
            }

            string SetName(string value) => value == "P" ? "ポリ箱" : "段ボール";
            string SetIcon(string value) => value == "P" ? "Package" : "PackageVariant";
        }
    }
}
