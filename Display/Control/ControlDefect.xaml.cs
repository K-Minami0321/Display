using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //インターフェース
    public interface IDefect
    {
        void SelectionItem(object value);
    }

    //画面クラス
    public partial class ControlDefect : UserControl
    {
        public static ControlDefect Instance
        { get; set; }
        public ControlDefect()
        {
            Instance = this;
            DataContext = ViewModelControlDefect.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelControlDefect : Common, IDefect, IDisposable
    {
        //変数
        CompositeDisposable Disposable                      //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelControlDefect Instance       //インスタンス
        { get; set; } = new ViewModelControlDefect();
        public IDefect Idefect                              //インターフェース
        { get; set; }
        public ReactiveProperty<string> ProcessName         //工程区分
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Defect              //不良内容
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<List<string>> Defects       //不良内容リスト
        { get; set; } = new ReactiveProperty<List<string>>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                if (x == null) { return; }
                iProcess = ProcessCategory.SetProcess(x);
                if (ViewModelWindowMain.Instance.ProcessWork.Value == "仕掛搬出")
                {
                    x = iProcess.Next;
                    iProcess = ProcessCategory.SetProcess(x);
                }
            }).AddTo(Disposable);
        }

        //コンストラクター
        public ViewModelControlDefect()
        {
            SetProperty();
        }

        //ロード時
        private void OnLoad()
        {
            Instance = this;
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;

            //不良内容追加
            Defects.Value = new List<string>();
            Defects.Value.Add("ハガレ");
            Defects.Value.Add("ワレ");
            Defects.Value.Add("巣");
        }

        //選択処理
        public void SelectionItem(object value)
        {
            //呼び出し元で実行
            value = Defect.Value.ToString();
            if (Idefect == null) { return; }

            SOUND.PlayAsync(SoundFolder.Value + CONST.SOUND_TOUCH);
            Idefect.SelectionItem(value);
        }

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
