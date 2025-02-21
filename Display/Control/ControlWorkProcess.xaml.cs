using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.Generic;
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
    public class ViewModelControlWorkProcess : Common, IWorkProcess
    {
        //変数
        bool visivleProcess;
        bool visivleAll;
        string workProcess;

        //プロパティ
        public static ViewModelControlWorkProcess Instance      //インスタンス
        { get; set; } = new ViewModelControlWorkProcess();
        public IWorkProcess IworkProcess                        //インターフェース
        { get; set; }
        public bool VisivleProcess                              //設備
        {
            get => visivleProcess;
            set => SetProperty(ref visivleProcess, value);
        }
        public bool VisivleAll                                  //すべて
        {
            get => visivleAll;
            set => SetProperty(ref visivleAll, value);
        }
        public string WorkProcess                               //工程
        {
            get => workProcess;
            set => SetProperty(ref workProcess, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        ViewModelControlWorkProcess()
        {
            Instance = this;
        }

        //ロード時
        private void OnLoad()
        {
            ReadINI();
            KeyDown("Process");
        }

        //選択処理
        public void SelectionItem(object value)
        {
            //呼び出し元で実行
            value = WorkProcess != null ? WorkProcess.ToString() : string.Empty;
            if (IworkProcess == null) { return; }

            var Sound = new SoundPlay();
            Sound.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            IworkProcess.SelectionItem(value);
        }

        //キー処理
        private void KeyDown(object value)
        {
            var listSource = new ListSource();
            switch (value)
            {
                case "Process":
                    ListSource.Process = ProcessName;
                    VisivleProcess = false;
                    VisivleAll = true;
                    break;

                case "All":
                    ListSource.Process = string.Empty;
                    VisivleProcess = true;
                    VisivleAll = false;
                    break;
            }
            WorkProcesses = ListSource.WorkProcesses;
        }
    }
}
