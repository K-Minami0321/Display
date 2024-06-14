using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //インターフェース
    public interface IWorker
    {
        void SelectionItem(object value);
    }

    //画面クラス
    public partial class ControlWorker : UserControl
    {
        public static ControlWorker Instance
        { get; set; }
        public ControlWorker()
        {
            Instance = this;
            DataContext = ViewModelControlWorker.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelControlWorker : Common, IWorker, IDisposable
    {
        //変数
        CompositeDisposable Disposable                      //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelControlWorker Instance       //インスタンス
        { get; set; } = new ViewModelControlWorker();
        public IWorker Iworker                              //インターフェース
        { get; set; }
        public ReactiveProperty<string> ProcessName         //工程区分
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> VisivleProcess        //作業者
        { get; set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> VisivleAll            //すべて
        { get; set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<string> Worker              //作業者
        { get; set; }= new ReactiveProperty<string>();
        public ReactiveProperty<List<string>> Workers       //作業者リスト
        { get; set; } = new ReactiveProperty<List<string>>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                if (x == null) { return; }
                iProcess = ProcessCategory.SetProcess(x);
            }).AddTo(Disposable);
        }

        //コンストラクター
        public ViewModelControlWorker()
        {
            SetProperty();
        }

        //ロード時
        private void OnLoad()
        {
            Instance = this;
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            KeyDown("Process");
        }

        //選択処理
        public void SelectionItem(object value)
        {
            //呼び出し元で実行
            value = Worker.Value != null ? Worker.Value.ToString() : string.Empty;
            if (Iworker == null) { return; }

            SOUND.PlayAsync(SoundFolder.Value + CONST.SOUND_TOUCH);
            Iworker.SelectionItem(value);
        }

        //キー処理
        private void KeyDown(object value)
        {
            switch (value)
            {
                case "Process":
                    Workers.Value = iProcess.Workers;
                    VisivleProcess.Value = false;
                    VisivleAll.Value = true;
                    break;

                case "All":
                    Workers.Value = Employee.SetWorker();
                    VisivleProcess.Value = true;
                    VisivleAll.Value = false;
                    break;
            }
        }

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
