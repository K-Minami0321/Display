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
        string _ProcessName;
        string _InProcessCODE;

        //プロパティ
        public static ViewModelTransportList Instance   //インスタンス
        { get; set; } = new ViewModelTransportList();
        public override string ProcessName              //工程区分
        {
            get { return inProcess.ProcessName; }
            set 
            {
                SetProperty(ref _ProcessName, value);
                inProcess.ProcessName = value;
                iProcess = ProcessCategory.SetProcess(value);
                ViewModelWindowMain.Instance.ProcessName = iProcess.Before;
            }
        }
        public override string InProcessCODE            //仕掛在庫CODE
        {
            get { return _InProcessCODE; }
            set { SetProperty(ref _InProcessCODE, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelTransportList()
        {
            inProcess = new InProcess();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;

            //初期設定
            DisplayCapution();
            Initialize();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName = INI.GetString("Page", "Process"); ;
            ViewModelWindowMain.Instance.ProcessWork = "仕掛引取";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = false;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList = "ViewList";
            ViewModelWindowMain.Instance.IconPlan = "FileDocumentArrowRightOutline";
        }

        //初期化
        private void Initialize()
        {
            //初期設定
            InProcessCODE = string.Empty;
            SelectedIndex = -1;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //引取登録
                    ViewModelWindowMain.Instance.FramePage = new TransportInfo();
                    break;

                case "DisplayList":
                    //引取履歴
                    ViewModelWindowMain.Instance.FramePage = new TransportHistory();
                    break;

                case "DiaplayPlan":
                    //仕掛置場
                    ViewModelWindowMain.Instance.FramePage = new TransportList();
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
            InProcessCODE = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            ViewModelWindowMain.Instance.FramePage = new TransportInfo();
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
