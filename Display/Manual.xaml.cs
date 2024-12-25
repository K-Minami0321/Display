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
    public class ViewModelManual : Common
    {
        //変数
        string processName;
        string file;

        //プロパティ
        public string ProcessName           //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);
                process = new ProcessCategory(value);
            }
        }
        public string File          //再生ファイル
        {
            get { return file; }
            set { SetProperty(ref file, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        public ViewModelManual()
        {
            File = @"\\b2000-m\共有\中小企業振興公社\工場見学\サンキャスト\IMG_0351.MOV";





        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();




        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.ProcessWork = "動画マニュアル";
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
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
    }
}
