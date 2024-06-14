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
    public interface IDefectCategory
    {
        void SelectionItem(object value);
    }

    //画面クラス
    public partial class ControlDefectCategory : UserControl
    {
        public static ControlDefectCategory Instance
        { get; set; }
        public ControlDefectCategory()
        {
            Instance = this;
            DataContext = ViewModelControlDefectCategory.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelControlDefectCategory : Common, IDefectCategory, IDisposable
    {
        //変数
        CompositeDisposable Disposable                              //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelControlDefectCategory Instance       //インスタンス
        { get; set; } = new ViewModelControlDefectCategory();
        public IDefectCategory IdefectCategory                      //インターフェース
        { get; set; }
        public ReactiveProperty<string> ProcessName                 //工程区分
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> DefectCategory              //不良分類
        { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<List<string>> DefectCategorys       //不良分類リスト
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
            }).AddTo(Disposable);
        }

        //コンストラクター
        public ViewModelControlDefectCategory()
        {
            SetProperty();
        }

        //ロード時
        private void OnLoad()
        {
            Instance = this;
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            DefectCategorys.Value = iProcess.DefectClasses;
        }

        //選択処理
        public void SelectionItem(object value)
        {
            //呼び出し元で実行
            value = DefectCategory.Value.ToString();
            if (IdefectCategory == null) { return; }

            SOUND.PlayAsync(SoundFolder.Value + CONST.SOUND_TOUCH);
            IdefectCategory.SelectionItem(value);

        }

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
