using ClassBase;
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
            DataContext = ViewModelControlTenKey.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelControlTenKey : Common, ITenKey
    {

        //プロパティ変数
        string _InputString;

        //プロパティ
        public static ViewModelControlTenKey Instance   //インスタンス
        { get; set; } = new ViewModelControlTenKey();
        public ITenKey Itenkey                          //インターフェース
        { get; set; }
        public string InputString                       //入力文字
        {
            get { return _InputString; }
            set { SetProperty(ref _InputString, value); }
        }


        //イベント
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //Key処理
        public void KeyDown(object value)
        {
            //呼び出し元で実行
            if (Itenkey == null) { return; }
            SOUND.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            Itenkey.KeyDown(value); 
        }
    }
}
