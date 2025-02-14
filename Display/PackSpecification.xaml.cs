using ClassBase;
using ClassLibrary;
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
        int lengthProductName = 20;
        BitmapSource image1;
        BitmapSource image2;
        ObservableCollection<string> nameButton = new ObservableCollection<string> { "ポリ箱", "段ボール", "段ボール" };
        ObservableCollection<string> iconButton = new ObservableCollection<string> { "Package", "PackageVariant", "PackageVariant" };
        bool visibileButton1 = true;
        bool visibileButton2 = false;
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
        public bool VisibileButton1                         //ボタン表示
        {
            get => visibileButton1;
            set => SetProperty(ref visibileButton1, value);
        }
        public bool VisibileButton2                         //ボタン表示
        {
            get => visibileButton2;
        set => SetProperty(ref visibileButton2, value);
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
        public ViewModelPackSpecification()
        {           
            var productPackingStyle = new ProductPackingStyle();
            CopyProperty(productPackingStyle, this);
            DisplayImage(productPackingStyle);
            SelectTable = productPackingStyle.SelectPakingIndex();
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
            }
        }

        //フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            FocusProductName = false;
            DisplayPackStyle(ProductName);
        }

        //梱包仕様表示
        private void DisplayPackStyle(string code, string no = "")
        {
            //データ取得
            var productPackingStyle = new ProductPackingStyle();
            ListTable = productPackingStyle.SelectPaking(code, no);
            CopyProperty(productPackingStyle, this);

            //表示
            ProductName = code;
            if (productPackingStyle.DataCount > 0) { ContainerCategory = SetName(ContainerCategory); }
            Container = STRING.ToTrim(Container.Replace("P", "").Replace("D", ""));

            if (!string.IsNullOrEmpty(code))
            {
                //ボタン制御
                VisibileButton1 = productPackingStyle.DataCount >= 2;
                VisibileButton2 = productPackingStyle.DataCount == 3;

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
            DisplayImage(productPackingStyle);
        }

        //画像表示処理
        private void DisplayImage(ProductPackingStyle product)
        {
            //画像1
            var img1 = File.Exists(product.ImageFolder + product.ImageSource1) ? product.ImageSource1 : "nophoto.jpg";
            var source1 = new Mat(product.ImageFolder + img1);
            var mat1 = new Mat();
            Cv2.Resize(source1, mat1, new OpenCvSharp.Size(800, 600), 0, 0, InterpolationFlags.Cubic);
            Image1 = BitmapSourceConverter.ToBitmapSource(mat1);
            mat1.Dispose();
            source1.Dispose();

            //画像2
            var img2 = File.Exists(product.ImageFolder + product.ImageSource2) ? product.ImageSource2 : "nophoto.jpg";
            var source2 = new Mat(product.ImageFolder + img2);
            var mat2 = new Mat();
            Cv2.Resize(source2, mat2, new OpenCvSharp.Size(800, 600), 0, 0, InterpolationFlags.Cubic);
            Image2 = BitmapSourceConverter.ToBitmapSource(mat2);
            mat2.Dispose();
            source2.Dispose();

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
