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
    public partial class Manual : UserControl
    {
        public Manual()
        {
            DataContext = new ViewModelManual();
            InitializeComponent();

            mediaElement.Source = new Uri(@"\\Sb2000-m\共有\中小企業振興公社\ミーティング資料\マニュアル\品質マニュアル\品質管理マニュアル基礎編動画方式.mp4");
            mediaElement.Play();
        }
    }

    //ViewModel
    public class ViewModelManual : Common, IWindowBase
    {
        //変数
        string processName;
        string file;

        //プロパティ
        public string ProcessName   //工程区分
        {
            get => processName;
            set => SetProperty(ref processName, value);
        }
        public string File          //再生ファイル
        {
            get => file;
            set=> SetProperty(ref file, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        public ViewModelManual()
        {
            File = @"\\Sb2000-m\共有\中小企業振興公社\ミーティング資料\マニュアル\品質マニュアル\品質管理マニュアル基礎編動画方式.mp4";





        }

        //ロード時
        private void OnLoad()
        {
            ReadINI();
            DisplayCapution();




        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {


            //WindowMain
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                VisiblePower = false,
                VisibleList = false,
                VisibleInfo = false,
                VisibleDefect = false,
                VisibleArrow = false,
                VisiblePlan = false,
                VisiblePrinter = false,
                VisibleQRcode = false,
                ProcessWork = "動画マニュアル"
            };


        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    StartPage(IniFile.GetString("Page", "Initial"));
                    break;
            }
        }

        //キーイベント
        public void KeyDown(object value)
        {



        }
    }
}
