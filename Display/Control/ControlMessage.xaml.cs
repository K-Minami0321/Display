using ClassBase;
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
        public ControlMessage(string messege,string contents, string type)
        {
            DataContext = ViewModelControlMessage.Instance;
            ViewModelControlMessage.Instance.Message = messege;
            ViewModelControlMessage.Instance.Contents = contents;
            ViewModelControlMessage.Instance.Type = type;
            InitializeComponent();
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
        public static ViewModelControlMessage Instance  //インスタンス
        { get; set; } = new ViewModelControlMessage();
        public string Message                           //処理メッセージ
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }
        public string Contents                          //処理内容
        {
            get { return contents; }
            set { SetProperty(ref contents, value); }
        }
        public string Type                              //メッセージボックスタイプ
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }
        public string ButtonOK                          //ボタン表示名
        {
            get { return buttonOK; }
            set { SetProperty(ref buttonOK, value); }
        }
        public bool IsButtonCancel                      //ボタン表示
        {
            get { return isButtonCancel; }
            set { SetProperty(ref isButtonCancel, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //ロード時
        private void OnLoad()
        {
            switch (Type)
            {
                case "警告":
                    ButtonOK = "はい";
                    IsButtonCancel = true;
                    SOUND.PlayAsync(SoundFolder + CONST.SOUND_WARNING);
                    break;

                case "確認":
                    ButtonOK = "OK";
                    IsButtonCancel = false;
                    SOUND.PlayAsync(SoundFolder + CONST.SOUND_NOTICE);
                    break;
            }
        }
    }
}
