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
        //変数
        string processName;
        bool visivleProcess;
        bool visivleAll;
        string workProcess;
        List<string> workProcesses;

        //プロパティ
        public static ViewModelControlWorkProcess Instance      //インスタンス
        { get; set; } = new ViewModelControlWorkProcess();
        public IWorkProcess IworkProcess                        //インターフェース
        { get; set; }
        public string ProcessName                               //工程区分
        {
            get { return processName; }
            set 
            { 
                SetProperty(ref processName, value);
                if (value == null) { return; }
                process = new ProcessCategory(value);
            }
        }
        public bool VisivleProcess                              //設備
        {
            get { return visivleProcess; }
            set { SetProperty(ref visivleProcess, value); }
        }
        public bool VisivleAll                                  //すべて
        {
            get { return visivleAll; }
            set { SetProperty(ref visivleAll, value); }
        }
        public string WorkProcess                               //工程
        {
            get { return workProcess; }
            set { SetProperty(ref workProcess, value); }
        }
        public List<string> WorkProcesses                       //工程リスト
        {
            get { return workProcesses; }
            set { SetProperty(ref workProcesses, value); }
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
            ProcessName = IniFile.GetString("Page", "Process");
            Instance = this;
        }

        //ロード時
        private void OnLoad()
        {
            KeyDown("Process");
        }

        //選択処理
        public void SelectionItem(object value)
        {
            //呼び出し元で実行
            value = WorkProcess != null ? WorkProcess.ToString() : string.Empty;
            if (IworkProcess == null) { return; }

            Sound.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            IworkProcess.SelectionItem(value);
        }

        //キー処理
        private void KeyDown(object value)
        {
            ListSource listSource = new ListSource();
            switch (value)
            {
                case "Process":
                    listSource.Process = ProcessName;
                    VisivleProcess = false;
                    VisivleAll = true;
                    break;

                case "All":
                    listSource.Process = string.Empty;
                    VisivleProcess = true;
                    VisivleAll = false;
                    break;
            }
            WorkProcesses = listSource.WorkProcesses;
        }
    }
}
