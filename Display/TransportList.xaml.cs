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
    public class ViewModelTransportList : Common, IWindowBase, ISelect
    {
        //プロパティ
        public static ViewModelTransportList Instance   //インスタンス
        { get; set; } = new ViewModelTransportList();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelTransportList()
        {
            Instance = this;
            windowMain.Interface = this;

            Initialize();
        }

        //ロード時
        private void OnLoad()
        {
            ReadINI();
            DisplayCapution();
            DiaplayList();
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
            windowMain.IconPlan = "TrayArrowUp";
            windowMain.ProcessWork = "合板倉庫";
            windowMain.ProcessName = ProcessBefore;
            DataGridBehavior.Instance.Iselect = this;
        }

        //初期化
        public void Initialize()
        {
            windowMain.Interface = this;

            SelectedIndex = -1;
        }

        //一覧表示
        private void DiaplayList()
        {
            InProcess inProcess = new InProcess();
            SelectTable = inProcess.SelectListTransport(ProcessBefore);
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem == null) { return; }
            TransportInfo.InProcessCODE = DATATABLE.SelectedRowsItem(SelectedItem, "仕掛CODE");
            DisplayFramePage(new TransportInfo());
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
