using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //インターフェース
    public interface ITenKey
    {
        void KeyDown(object value);
    }

    //画面クラス
    public partial class ControlTenKey : UserControl
    {
         public ControlTenKey()
        {
            DataContext = new ViewModelControlTenKey();
            InitializeComponent();
        }
    }

    //プロパティ
    public class PropertyTenKey
    {
        public static ViewModelControlTenKey ViewModel      //ViewModel
        { get; set; }
        public ITenKey Itenkey                              //インターフェース
        {
            get => ViewModel.Itenkey;
            set => ViewModel.Itenkey = value;
        }
        public string InputString                       //入力文字
        {
            get => ViewModel.InputString;
            set => ViewModel.InputString = value;
        }
    }

    //ViewModel
    public class ViewModelControlTenKey : Common, ITenKey
    {
        //変数
        string inputString;

        //プロパティ
        public ITenKey Itenkey                          //インターフェース
        { get; set; }
        public string InputString                       //入力文字
        {
            get => inputString;
            set => SetProperty(ref inputString, value);
        }

        //イベント
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        public ViewModelControlTenKey()
        {
            PropertyTenKey.ViewModel = this;
        }

        //Key処理
        public void KeyDown(object value)
        {
            //呼び出し元で実行
            if (Itenkey == null) { return; }

            var Sound = new SoundPlay();
            Sound.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            Itenkey.KeyDown(value); 
        }
    }
}
