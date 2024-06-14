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
        //プロパティ変数
        string _Message;
        string _Contents;
        string _Type;
        string _ButtonOK;
        bool _IsButtonCancel;

        //プロパティ
        public static ViewModelControlMessage Instance  //インスタンス
        { get; set; } = new ViewModelControlMessage();
        public string Message                           //処理メッセージ
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }
        public string Contents                          //処理内容
        {
            get { return _Contents; }
            set { SetProperty(ref _Contents, value); }
        }
        public string Type                              //メッセージボックスタイプ
        {
            get { return _Type; }
            set { SetProperty(ref _Type, value); }
        }
        public string ButtonOK                          //ボタン表示名
        {
            get { return _ButtonOK; }
            set { SetProperty(ref _ButtonOK, value); }
        }
        public bool IsButtonCancel                      //ボタン表示
        {
            get { return _IsButtonCancel; }
            set { SetProperty(ref _IsButtonCancel, value); }
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
