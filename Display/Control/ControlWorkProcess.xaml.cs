using ClassBase;
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
        //プロパティ変数
        string _ProcessName;
        bool _VisivleProcess;
        bool _VisivleAll;
        string _WorkProcess;
        List<string> _WorkProcesses;

        //プロパティ
        public static ViewModelControlWorkProcess Instance      //インスタンス
        { get; set; } = new ViewModelControlWorkProcess();
        public IWorkProcess IworkProcess                        //インターフェース
        { get; set; }
        public string ProcessName                               //工程区分
        {
            get { return _ProcessName; }
            set 
            { 
                SetProperty(ref _ProcessName, value);
                if (value == null) { return; }
                iProcess = ProcessCategory.SetProcess(value);
            }
        }
        public bool VisivleProcess                              //設備
        {
            get { return _VisivleProcess; }
            set { SetProperty(ref _VisivleProcess, value); }
        }
        public bool VisivleAll                                  //すべて
        {
            get { return _VisivleAll; }
            set { SetProperty(ref _VisivleAll, value); }
        }
        public string WorkProcess                               //工程
        {
            get { return _WorkProcess; }
            set { SetProperty(ref _WorkProcess, value); }
        }
        public List<string> WorkProcesses                       //工程リスト
        {
            get { return _WorkProcesses; }
            set { SetProperty(ref _WorkProcesses, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //ロード時
        private void OnLoad()
        {
            Instance = this;
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            KeyDown("Process");
        }

        //選択処理
        public void SelectionItem(object value)
        {
            //呼び出し元で実行
            value = WorkProcess != null ? WorkProcess.ToString() : string.Empty;
            if (IworkProcess == null) { return; }
 
            SOUND.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            IworkProcess.SelectionItem(value);
        }

        //キー処理
        private void KeyDown(object value)
        {
            switch (value)
            {
                case "Process":
                    WorkProcesses = iProcess.WorkProcesses;
                    VisivleProcess = false;
                    VisivleAll = true;
                    break;

                case "All":
                    WorkProcesses = iProcess.WorkProcesses;
                    VisivleProcess = true;
                    VisivleAll = false;
                    break;
            }
        }
    }
}
