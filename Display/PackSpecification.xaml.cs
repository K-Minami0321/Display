using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
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
        




        string comment;
        string imageFolder;
        string imageSource1;
        string imageSource2;
        int lengthProductName = 20;

        //プロパティ
        





        public string Comment               //備考
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }
        public string ImageFolder           //画像フォルダ
        {
            get { return CONST.SERVER_FILE + CONST.FORDER_PACKAGING; }
        }
        public string ImageSource1          //画像1
        {
            get { return imageSource1; }
            set { SetProperty(ref imageSource1, value); }
        }
        public string ImageSource2          //画像2
        {
            get { return imageSource2; }
            set { SetProperty(ref imageSource2, value); }
        }
        public int LengthProductName        //文字数（品番）
        {
            get { return lengthProductName; }
            set { SetProperty(ref lengthProductName, value); }
        }


        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand gotFocus;
        public ICommand GotFocus => gotFocus ??= new ActionCommand(SetGotFocus);
        ActionCommand lostFocus;
        public ICommand LostFocus => lostFocus ??= new ActionCommand(SetLostFocus);

        //コンストラクター
        public ViewModelPackSpecification()
        {
            ProductName = string.Empty;


        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
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






            }
        }

        //フォーカス処理（GotFocus）
        private void SetGotFocus(object value)
        {






        }

        //フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            DisplayPackSpecification(ProductName);
        }

        //梱包仕様表示
        private void DisplayPackSpecification(string product)
        {






            Comment = "ビニール 太いゴム +2個 50×1ラベル入 100×2";
            ImageSource1 = ImageFolder + "083G11050_1.jpg";
            ImageSource2 = ImageFolder + "083G11050_2.jpg";
        }
    }
}
