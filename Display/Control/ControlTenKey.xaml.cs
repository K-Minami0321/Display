﻿using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
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
        //プロパティ
        public static ViewModelControlTenKey Instance   //インスタンス
        { get; set; } = new ViewModelControlTenKey();
        public ITenKey Itenkey                          //インターフェース
        { get; set; }
        public ReactiveProperty<string> InputString     //入力文字
        { get; set; } = new ReactiveProperty<string>();

        //イベント
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //Key処理
        public void KeyDown(object value)
        {
            //呼び出し元で実行
            if (Itenkey == null) { return; }
            SOUND.PlayAsync(SoundFolder.Value + CONST.SOUND_TOUCH);
            Itenkey.KeyDown(value); 
        }
    }
}
