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
        public string worker;
        public bool visivleProcess;
        public bool visivleAll;

        //プロパティ
        public static ViewModelControlWorker Instance       //インスタンス
        { get; set; } = new ViewModelControlWorker();
        public IWorker Iworker                              //インターフェース
        { get; set; }
        public string Worker                                //作業者
        {
            get { return worker; }
            set { SetProperty(ref worker, value); }
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

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        ViewModelControlWorker()
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
            value = Worker != null ? Worker.ToString() : string.Empty;
            if (Iworker == null) { return; }

            Sound.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            Iworker.SelectionItem(value);
        }

        //キー処理・一覧の作成
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
                    listSource.Process = null;
                    VisivleProcess = true;
                    VisivleAll = false;
                    break;
            }
            Workers = listSource.Workers;
        }
    }
}
