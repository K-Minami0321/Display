using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
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
        public ControlWorker()
        {
            DataContext = new ViewModelControlWorker();
            InitializeComponent();
        }
    }

    //プロパティ
    public class PropertyWorker
    {
        public static ViewModelControlWorker ViewModel      //ViewModel
        { get; set; }
        public IWorker Iworker                              //インターフェース
        {
            get => ViewModel.Iworker;
            set => ViewModel.Iworker = value;
        }
        public string Worker                                //作業者
        {
            get => ViewModel.Worker;
            set => ViewModel.Worker = value;
        }
        public bool VisivleProcess                          //作業者
        {
            get => ViewModel.VisivleProcess;
            set => ViewModel.VisivleProcess = value;
        }
        public bool VisivleAll                              //すべて
        {
            get => ViewModel.VisivleAll;
            set => ViewModel.VisivleAll = value;
        }
    }

    //ViewModel
    public class ViewModelControlWorker : Common, IWorker
    {
        //変数
        public string worker;
        public bool visivleProcess;
        public bool visivleAll;

        //プロパティ
        public IWorker Iworker                              //インターフェース
        { get; set; }
        public string Worker                                //作業者
        {
            get => worker;
            set => SetProperty(ref worker, value);
        }
        public bool VisivleProcess                          //作業者
        {
            get => visivleProcess;
            set => SetProperty(ref visivleProcess, value);
        }
        public bool VisivleAll                              //すべて
        {
            get => visivleAll;
            set => SetProperty(ref visivleAll, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        public ViewModelControlWorker()
        {
            PropertyWorker.ViewModel = this;
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
            value = Worker != null ? Worker.ToString() : string.Empty;
            if (Iworker == null) { return; }

            var Sound = new SoundPlay();
            Sound.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            Iworker.SelectionItem(value);
        }

        //キー処理・一覧の作成
        private void KeyDown(object value)
        {
            switch (value)
            {
                case "Process":
                    ListSource.Process = ProcessName;
                    VisivleProcess = false;
                    VisivleAll = true;
                    break;

                case "All":
                    ListSource.Process = null;
                    VisivleProcess = true;
                    VisivleAll = false;
                    break;
            }
            Workers = ListSource.Workers;
        }
    }
}
