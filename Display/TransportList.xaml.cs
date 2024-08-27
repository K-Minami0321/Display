using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class TransportList : UserControl
    {
        public TransportList()
        {
            DataContext = ViewModelTransportList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportList : Common, IKeyDown, ISelect
    {
        //プロパティ変数
        ViewModelWindowMain windowMain;
        DataGridBehavior dataGridBehavior;
        string processName;
        string inProcessCODE;

        //プロパティ
        public static ViewModelTransportList Instance   //インスタンス
        { get; set; } = new ViewModelTransportList();
        public string ProcessName                       //工程区分
        {
            get { return inProcess.ProcessName; }
            set 
            {
                SetProperty(ref processName, value);
                inProcess.ProcessName = value;
                process = new ProcessCategory(value);
                windowMain.ProcessName = process.Before;
            }
        }
        public string InProcessCODE                     //仕掛在庫CODE
        {
            get { return inProcessCODE; }
            set { SetProperty(ref inProcessCODE, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelTransportList()
        {
            inProcess = new InProcess();
            SelectedIndex = -1;
        }

        //ロード時
        private void OnLoad()
        {
            SetInterface();
            DisplayCapution();
        }

        //インターフェース設定
        private void SetInterface()
        {
            windowMain = ViewModelWindowMain.Instance;
            dataGridBehavior = DataGridBehavior.Instance;

            windowMain.Ikeydown = this;
            dataGridBehavior.Iselect = this;
            Instance = this;
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            Initialize();
            windowMain.VisiblePower = true;
            windowMain.VisibleList = true;
            windowMain.VisibleInfo = false;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = false;
            windowMain.VisiblePlan = true;
            windowMain.InitializeIcon();
            windowMain.IconList = "ViewList";
            windowMain.IconPlan = "FileDocumentArrowRightOutline";
            windowMain.ProcessWork = "仕掛引取";
            DiaplayList();
        }

        //初期化
        private void Initialize()
        {
            //初期設定
            ProcessName = IniFile.GetString("Page", "Process");
            InProcessCODE = string.Empty;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //引取登録
                    windowMain.FramePage = new TransportInfo();
                    break;

                case "DisplayList":
                    //引取履歴
                    windowMain.FramePage = new TransportHistory();
                    break;

                case "DiaplayPlan":
                    //仕掛置場
                    windowMain.FramePage = new TransportList();
                    break;
            }
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectTable = inProcess.SelectListTransport();
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem == null) { return; }
            TransportInfo.InProcessCODE = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            windowMain.FramePage = new TransportInfo();
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    KeyDown("DisplayInfo");
                    break;
            }
        }
    }
}
