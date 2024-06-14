using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
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
            ViewModelControlMessage.Instance.Message.Value = messege;
            ViewModelControlMessage.Instance.Contents.Value = contents;
            ViewModelControlMessage.Instance.Type.Value = type;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelControlMessage : Common
    {
        //プロパティ
        public static ViewModelControlMessage Instance  //インスタンス
        { get; set; } = new ViewModelControlMessage();
        public ReactiveProperty<string> Message         //処理メッセージ
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Contents        //処理内容
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Type            //メッセージボックスタイプ
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ButtonOK        //ボタン表示名
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> IsButtonCancel    //ボタン表示
        { get; set; } = new ReactiveProperty<bool>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //ロード時
        private void OnLoad()
        {
            switch (Type.Value)
            {
                case "警告":
                    ButtonOK.Value = "はい";
                    IsButtonCancel.Value = true;
                    SOUND.PlayAsync(SoundFolder.Value + CONST.SOUND_WARNING);
                    break;

                case "確認":
                    ButtonOK.Value = "OK";
                    IsButtonCancel.Value = false;
                    SOUND.PlayAsync(SoundFolder.Value + CONST.SOUND_NOTICE);
                    break;
            }
        }
    }
}
