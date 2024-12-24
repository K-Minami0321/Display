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
        string processName;
        string inProcessCODE;

        //プロパティ
        public static ViewModelTransportList Instance   //インスタンス
        { get; set; } = new ViewModelTransportList();
        public string ProcessName                       //工程区分
        {
            get { return ProcessName; }
            set 
            {
                SetProperty(ref processName, value);
                process = new ProcessCategory(value);
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
            Initialize();
            DiaplayList();
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            ViewModelWindowMain windowMain = ViewModelWindowMain.Instance;
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
            windowMain.Ikeydown = this;
            windowMain.ProcessName = process.Before;

            DataGridBehavior dataGridBehavior = DataGridBehavior.Instance;
            dataGridBehavior.Iselect = this;

            Instance = this;
        }

        //初期化
        public void Initialize()
        {
            ProcessName = IniFile.GetString("Page", "Process");
            SelectedIndex = -1;
            InProcessCODE = string.Empty;
        }

        //一覧表示
        private void DiaplayList()
        {
            InProcess inProcess = new InProcess();
            SelectTable = inProcess.SelectListTransport(process.Before);
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem == null) { return; }
            var code = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            DisplayFramePage(new TransportInfo(code));
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    KeyDown("DisplayList");
                    break;
            }
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayList":
                    //引取履歴
                    DisplayFramePage(new TransportHistory());
                    break;

                case "DiaplayPlan":
                    //仕掛置場
                    DisplayFramePage(new TransportList());
                    break;
            }
        }
    }
}
