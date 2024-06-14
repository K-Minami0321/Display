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
    public interface IWorkProcess
    {
        void SelectionItem(object value);
    }

    //画面クラス
    public partial class ControlWorkProcess : UserControl
    {
        public static ControlWorkProcess Instance
        { get; set; }
        public ControlWorkProcess()
        {
            Instance = this;
            DataContext = ViewModelControlWorkProcess.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelControlWorkProcess : Common, IWorkProcess, IDisposable
    {
        //変数
        CompositeDisposable Disposable                          //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelControlWorkProcess Instance      //インスタンス
        { get; set; } = new ViewModelControlWorkProcess();
        public IWorkProcess IworkProcess                        //インターフェース
        { get; set; }
        public ReactiveProperty<string> ProcessName             //工程区分
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> VisivleProcess            //設備
        { get; set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> VisivleAll                //すべて
        { get; set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<string> WorkProcess             //工程
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<List<string>> WorkProcesses     //工程リスト
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
        public ViewModelControlWorkProcess()
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
            value = WorkProcess.Value != null ? WorkProcess.Value.ToString() : string.Empty;
            if (IworkProcess == null) { return; }
 
            SOUND.PlayAsync(SoundFolder.Value + CONST.SOUND_TOUCH);
            IworkProcess.SelectionItem(value);
        }

        //キー処理
        private void KeyDown(object value)
        {
            switch (value)
            {
                case "Process":
                    WorkProcesses.Value = iProcess.WorkProcesses;
                    VisivleProcess.Value = false;
                    VisivleAll.Value = true;
                    break;

                case "All":
                    WorkProcesses.Value = iProcess.WorkProcesses;
                    VisivleProcess.Value = true;
                    VisivleAll.Value = false;
                    break;
            }
        }

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
