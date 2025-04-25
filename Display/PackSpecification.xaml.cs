using ClassBase;
using ClassLibrary;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Xaml.Behaviors.Core;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class PackSpecification : UserControl
    {
        public PackSpecification(string code)
        {
            DataContext = new ViewModelPackSpecification(code);
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelPackSpecification : Common, IWindowBase, IBarcode
    {
        //変数
        ProductPackingStyle productPackingStyle = new ProductPackingStyle();
        Management management = new Management();
        DataTable selectTable;
        DataTable listTable;
        string containerCategory;
        string container;
        string productCustomer;
        string carton;
        string palette;
        string containerComment;
        string rackNo;
        int lengthProductName = 20;
        BitmapSource image1;
        BitmapSource image2;
        ObservableCollection<string> nameButton = new ObservableCollection<string> { "ポリ箱", "段ボール", "段ボール" };
        ObservableCollection<string> iconButton = new ObservableCollection<string> { "Package", "PackageVariant", "PackageVariant" };
        string colorButton1 = CONST.BUTTON_COLOR;
        string colorButton2 = CONST.BUTTON_COLOR;
        string colorButton3 = CONST.BUTTON_COLOR;
        bool visibleButton1 = true;
        bool visibleButton2 = true;
        bool visibleButton3 = false;
        bool visibleProduct = true;
        bool focusLotProductName = false;

        //プロパティ
        public DataTable SelectTable                        //データテーブル
        {
            get => selectTable;
            set => SetProperty(ref selectTable, value);
        }
        public DataTable ListTable                          //一覧テーブル
        {
            get => listTable;
            set => SetProperty(ref listTable, value);
        }
        public string ContainerCategory                     //段ボール・ポリ箱
        {
            get => containerCategory;
            set => SetProperty(ref containerCategory, value);
        }
        public string Container                             //段ボール・ポリ箱 詳細
        {
            get => container;
            set => SetProperty(ref container, value);
        }
        public string ProductCustomer                       //客先部品番号
        {
            get => productCustomer;
            set => SetProperty(ref productCustomer, value);
        }
        public string Carton                                //詰数
        {
            get => carton;
            set => SetProperty(ref carton, value);
        }
        public string Palette                               //パレット
        {
            get => palette;
            set => SetProperty(ref palette, value);
        }
        public string ContainerComment                      //備考
        {
            get => containerComment;
            set => SetProperty(ref containerComment, value);
        }
        public string RackNo                                //棚番
        {
            get => rackNo;
            set => SetProperty(ref rackNo, value);
        }
        public int LengthProductName                        //文字数（品番）
        {
            get => lengthProductName;
            set => SetProperty(ref lengthProductName, value);
        }
        public BitmapSource Image1                          //画像1
        {
            get => image1;
            set => SetProperty(ref image1, value);
        }
        public BitmapSource Image2                          //画像2
        {
            get => image2;
            set => SetProperty(ref image2, value);
        }
        public ObservableCollection<string> NameButton      //ボタン名称
        {
            get => nameButton;
            set => SetProperty(ref nameButton, value);
        }
        public ObservableCollection<string> IconButton      //ボタンアイコン
        {
            get => iconButton;
            set => SetProperty(ref iconButton, value);
        }
        public string ColorButton1                          //ボタン背景色
        {
            get => colorButton1;
            set => SetProperty(ref colorButton1, value);
        }
        public string ColorButton2                          //ボタン背景色
        {
            get => colorButton2;
            set => SetProperty(ref colorButton2, value);
        }
        public string ColorButton3                          //ボタン背景色
        {
            get => colorButton3;
            set => SetProperty(ref colorButton3, value);
        }
        public bool VisibleButton1                          //ボタン表示
        {
            get => visibleButton1;
            set => SetProperty(ref visibleButton1, value);
        }
        public bool VisibleButton2                          //ボタン表示
        {
            get => visibleButton2;
            set => SetProperty(ref visibleButton2, value);
        }
        public bool VisibleButton3                          //ボタン表示
        {
            get => visibleButton3;
            set => SetProperty(ref visibleButton3, value);
        }
        public bool VisibleProduct                          //表示・非表示
        {
            get => visibleProduct;
            set => SetProperty(ref visibleProduct, value);
        }
        public bool FocusProductName                        //フォーカス（品番）
        {
            get => focusLotProductName;
            set => SetProperty(ref focusLotProductName, value);
        }
        

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand lostFocus;
        public ICommand LostFocus => lostFocus ??= new ActionCommand(SetLostFocus);

        //コンストラクター
        public ViewModelPackSpecification(string code)
        {
            Ibarcode = this;

            CopyProperty(productPackingStyle, this);
            DisplayImage();

            SelectTable = productPackingStyle.SelectPakingIndex();
            ProductName = code;
            VisibleProduct = string.IsNullOrEmpty(ProductName);
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            DisplayPackStyle(ProductName, "1");
            FocusProductName = true;
        }

        //コントロールの設定
        private void DisplayCapution()
        {
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                VisibleList = !string.IsNullOrEmpty(ProductName),
                VisibleInfo = false,
                VisibleDefect = false,
                VisibleArrow = VisibleProduct,
                VisiblePlan = false,
                VisiblePrinter = false,
                VisibleQRcode = false,
                ProcessWork = "梱包仕様書",
                Process = "梱包"
            };
        }

        //キーイベント
        public async void KeyDown(object value)
        {
            switch (value)
            {
                case "ContainerCategory1":

                    DisplayPackStyle(ProductName, "1");
                    break;

                case "ContainerCategory2":

                    DisplayPackStyle(ProductName, "2");
                    break;

                case "ContainerCategory3":

                    DisplayPackStyle(ProductName, "3");
                    break;

                case "PreviousDate":

                    //前のデータ
                    SetAroundCode("Back");
                    break;

                case "NextDate":

                    //次のデータ
                    SetAroundCode("Forword");
                    break;

                case "DisplayList":

                    //仕掛在庫一覧画面
                    DisplayFramePage(new ShippingList());
                    break;
            }
        }

        //フォーカス処理
        private void SetLostFocus()
        {
            FocusProductName = false;
            DisplayPackStyle(ProductName);
        }

        //梱包仕様表示
        private void DisplayPackStyle(string code, string no = "1")
        {
            //初期値
            VisibleButton1 = true;
            VisibleButton2 = true;
            VisibleButton3 = false;

            //データ取得
            ListTable = productPackingStyle.SelectPaking(code, no);
            CopyProperty(productPackingStyle, this);

            //表示
            ProductName = code;
            if (productPackingStyle.DataCount > 0) { ContainerCategory = SetName(ContainerCategory); }
            DisplayImage();

            if (!string.IsNullOrEmpty(code))
            {
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
                if (querry.Count() > 0)
                {
                    IndexNumber = INT.ToInt(querry.FirstOrDefault().連番);
                    VisibleButton1 = productPackingStyle.DataCount >= 1;
                    VisibleButton2 = productPackingStyle.DataCount >= 2;
                    VisibleButton3 = productPackingStyle.DataCount == 3;
                }
            }
            string SetName(string value) => value == "P" ? "ポリ箱" : "段ボール";
            string SetIcon(string value) => value == "P" ? "Package" : "PackageVariant";
            WindowProperty.VisibleArrow = VisibleProduct && productPackingStyle.DataCount > 0;

            //ボタンの色
            switch (no)
            {
                case "1": case "":
                    ColorButton1 = CONST.BUTTON_SELECT;
                    ColorButton2 = CONST.BUTTON_FORCUS;
                    ColorButton3 = CONST.BUTTON_FORCUS;
                    break;

                case "2":
                    ColorButton1 = CONST.BUTTON_FORCUS;
                    ColorButton2 = CONST.BUTTON_SELECT;
                    ColorButton3 = CONST.BUTTON_FORCUS;
                    break;

                case "3":
                    ColorButton1 = CONST.BUTTON_FORCUS;
                    ColorButton2 = CONST.BUTTON_FORCUS;
                    ColorButton3 = CONST.BUTTON_SELECT;
                    break;
            }
        }

        //画像表示処理
        private async void DisplayImage()
        {
            //画像1
            var img1 = File.Exists(productPackingStyle.ImageFolder + productPackingStyle.ImageSource1) ? productPackingStyle.ImageSource1 : "nophoto.jpg";
            var source1 = new Mat(productPackingStyle.ImageFolder + img1);
            var mat1 = new Mat();
            Cv2.Resize(source1, mat1, new OpenCvSharp.Size(800, 600), 0, 0, InterpolationFlags.Cubic);
            Image1 = BitmapSourceConverter.ToBitmapSource(mat1);
            mat1.Dispose();
            source1.Dispose();

            //画像2
            var img2 = File.Exists(productPackingStyle.ImageFolder + productPackingStyle.ImageSource2) ? productPackingStyle.ImageSource2 : "nophoto.jpg";
            var source2 = new Mat(productPackingStyle.ImageFolder + img2);
            var mat2 = new Mat();
            Cv2.Resize(source2, mat2, new OpenCvSharp.Size(800, 600), 0, 0, InterpolationFlags.Cubic);
            Image2 = BitmapSourceConverter.ToBitmapSource(mat2);
            mat2.Dispose();
            source2.Dispose();
        }

        //QRコード設定
        public void GetQRCode()
        {
            //ロット番号
            if (CONVERT.IsLotNumber(ReceivedData))
            {
                management.LotNumber = ReceivedData.StringLeft(10);
                ProductName = management.ProductName;
                //DisplayPackStyle(ProductName);
            }
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

                    if (IndexNumber + 1 < SelectTable.Rows.Count) { IndexNumber = IndexNumber + 1; }
                    break;

                case "Forword":

                    IndexNumber = IndexNumber - 1;
                    break;
            }

            //CODEの設定
            if (IndexNumber < 0) { IndexNumber = 0; }
            DataRow dr = SelectTable.Rows[IndexNumber];
            var code = dr["品番"].ToString();
            DisplayPackStyle(dr["品番"].ToString());
        }
    }
}
