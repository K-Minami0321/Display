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
        //変数
        InProcess inProcess = new InProcess();

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
        }

        //ロード時
        private void OnLoad()
        {
            CtrlWindow.IwindowBase = this;
            ReadINI();
            DisplayCapution();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            CtrlWindow.VisiblePower = true;
            CtrlWindow.VisibleList = true;
            CtrlWindow.VisibleInfo = false;
            CtrlWindow.VisibleDefect = false;
            CtrlWindow.VisibleArrow = false;
            CtrlWindow.VisiblePlan = true;
            CtrlWindow.InitializeIcon();
            CtrlWindow.IconList = "ViewList";
            CtrlWindow.IconPlan = "TrayArrowUp";
            CtrlWindow.ProcessWork = "合板倉庫";
            CtrlWindow.ProcessName = ProcessBefore;
            DataGridBehavior.Instance.Iselect = this;
        }

        //一覧表示
        private void DiaplayList()
        {
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
