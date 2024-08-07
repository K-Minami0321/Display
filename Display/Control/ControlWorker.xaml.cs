using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.Generic;
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
    public class ViewModelControlWorker : Common, IWorker
    {
        //変数
        public string processName;
        public bool visivleProcess;
        public bool visivleAll;
        public string worker;
        public List<string> workers;

        //プロパティ
        public static ViewModelControlWorker Instance       //インスタンス
        { get; set; } = new ViewModelControlWorker();
        public IWorker Iworker                              //インターフェース
        { get; set; }
        public string ProcessName                           //工程区分
        {
            get { return processName; }
            set 
            { 
                SetProperty(ref processName, value);
                if (value == null) { return; }
                iProcess = ProcessCategory.SetProcess(value);
            }
        }
        public bool VisivleProcess                          //作業者
        {
            get { return visivleProcess; }
            set { SetProperty(ref visivleProcess, value);}
        }
        public bool VisivleAll                              //すべて
        {
            get { return visivleAll; }
            set { SetProperty(ref visivleAll, value); }
        }
        public string Worker                                //作業者
        {
            get { return worker; }
            set { SetProperty(ref worker, value); }
        }
        public List<string> Workers                         //作業者リスト
        {
            get { return workers; }
            set { SetProperty(ref workers, value); }
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
            value = Worker != null ? Worker.ToString() : string.Empty;
            if (Iworker == null) { return; }

            SOUND.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            Iworker.SelectionItem(value);
        }

        //キー処理
        private void KeyDown(object value)
        {
            switch (value)
            {
                case "Process":
                    Workers = iProcess.Workers;
                    VisivleProcess = false;
                    VisivleAll = true;
                    break;

                case "All":
                    Workers = SetComboBox.SetWorker();
                    VisivleProcess = true;
                    VisivleAll = false;
                    break;
            }
        }
    }
}
