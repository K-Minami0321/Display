using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
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
        }
    }

    //ViewModel
    public class ViewModelManual : Common
    {

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelManual()
        {



        }

        //ロード時
        private void OnLoad()
        {
            




        }


        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    StartPage(INI.GetString("Page", "Initial"));
                    break;
            }
        }
    }
}
