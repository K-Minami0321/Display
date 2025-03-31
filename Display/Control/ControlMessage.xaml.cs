using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Controls;
using System.Windows.Input;

//---------------------------------------------------------------------------------------
//
//  メッセージボックス
//
//
//

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class ControlMessage : UserControl
    {
        //コンストラクター
        public ControlMessage()
        {
            DataContext = new ViewModelControlMessage(this);
            InitializeComponent();
        }
    }

    //プロパティ
    //メッセージコントロールプロパティ
    public class PropertyMessageControl
    {
        public static ViewModelControlMessage ViewModel     //ViewModel
        { get; set; }
        public static ControlMessage PanelMessage           //画面
        { get; set; }
        public string Message                               //メッセージ
        {
            get => ViewModel.Message;
            set => ViewModel.Message = value;
        }
        public string Contents                              //処理内容
        {
            get => ViewModel.Contents;
            set => ViewModel.Contents = value;
        }
        public string Type                                  //メッセージボックスタイプ
        {
            get => ViewModel.Type;
            set => ViewModel.Type = value;
        }
    }

    //ViewModel
    public class ViewModelControlMessage : Common
    {
        //変数
        string message;
        string contents;
        string type;
        string buttonOK;
        bool isButtonCancel;

        //プロパティ
        public string Message                               //処理メッセージ
        {
            get => message;
            set => SetProperty(ref message, value);
        }
        public string Contents                              //処理内容
        {
            get => contents;
            set => SetProperty(ref contents, value);
        }
        public string Type                                  //メッセージボックスタイプ
        {
            get => type;
            set => SetProperty(ref type, value);
        }
        public string ButtonOK                              //ボタン表示名
        {
            get => buttonOK;
            set => SetProperty(ref buttonOK, value);
        }
        public bool IsButtonCancel                          //ボタン表示
        {
            get => isButtonCancel;
            set => SetProperty(ref isButtonCancel, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        public ViewModelControlMessage(ControlMessage control)
        {
            PropertyMessageControl.PanelMessage = control;
            PropertyMessageControl.ViewModel = this;
        }

        //ロード時
        private void OnLoad()
        {
            var Sound = new SoundPlay();

            switch (Type)
            {
                case "警告":
                    ButtonOK = "はい";
                    IsButtonCancel = true;
                    Sound.PlayAsync(SoundFolder + CONST.SOUND_WARNING);
                    break;

                case "確認":
                    ButtonOK = "OK";
                    IsButtonCancel = false;
                    Sound.PlayAsync(SoundFolder + CONST.SOUND_NOTICE);
                    break;
            }
        }
    }
}
