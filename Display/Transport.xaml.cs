using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class Transport : UserControl
    {
        //コンストラクター
        public Transport()
        {
            DataContext = new ViewModelTransport();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransport : Common, IWindowBase, IBarcode
    {
        //変数
        ManagementSlip managementSlip = new ManagementSlip();
        string transportDate = DateTime.Now.ToString();
        string headerUnit;
        string headerWeight;
        string headerAmount;
        bool visibleShape;
        bool visibleUnit;
        bool visibleWeight;

        //プロパティ
        public string TransportDate             //作業日
        {
            get => transportDate;
            set => SetProperty(ref transportDate, value);
        }
        public string LotNumber                 //ロット番号
        { get; set; }
        public string LotNumberSEQ              //ロット番号SEQ
        { get; set; }
        public string HeaderUnit                //コイル・枚数
        {
            get => headerUnit;
            set => SetProperty(ref headerUnit, value);
        }
        public string HeaderWeight              //焼結重量・単重
        {
            get => headerWeight;
            set => SetProperty(ref headerWeight, value);
        }
        public string HeaderAmount              //ヘッダー（重量・数量）
        {
            get => headerAmount;
            set => SetProperty(ref headerAmount, value);
        }
        public bool VisibleShape                //表示・非表示（形状）
        {
            get => visibleShape;
            set => SetProperty(ref visibleShape, value);
        }
        public bool VisibleUnit                 //表示・非表示（コイル・枚数）
        {
            get => visibleUnit;
            set => SetProperty(ref visibleUnit, value);
        }
        public bool VisibleWeight               //表示・非表示（重量）
        {
            get => visibleWeight;
            set => SetProperty(ref visibleWeight, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        internal ViewModelTransport()
        {
            Ibarcode = this;
        }

        //ロード時
        private void OnLoad()
        {
            CtrlWindow.Interface = this;
            ReadINI();
            DisplayCapution();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            CtrlWindow.ProcessWork = "引取処理";
            CtrlWindow.VisiblePower = true;
            CtrlWindow.VisibleList = false;
            CtrlWindow.VisibleInfo = false;
            CtrlWindow.VisibleDefect = false;
            CtrlWindow.VisibleArrow = false;
            CtrlWindow.VisiblePlan = false;
            CtrlWindow.InitializeIcon();
            CtrlWindow.IconList = "ViewList";
            CtrlWindow.IconPlan = "TrayArrowUp";
            CtrlWindow.ProcessName = ProcessName;
        }

        //QRコード処理
        public void SetBarcode()
        {
            if (CONVERT.IsLotNumber(ReceivedData)) 
            {
                //ロット番号
                LotNumber = ReceivedData.StringLeft(10);
                LotNumberSEQ = ReceivedData.StringRight(ReceivedData.Length - 11);
                SelectTable = managementSlip.Select(SelectTable, LotNumber, LotNumberSEQ);
            }
            else
            {
                //作業者
                Worker = ReceivedData;

            }
        }

        //スワイプ処理
        public void Swipe(object value) { return; }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayList":
                    //引取履歴

                    break;

                case "DisplayPlan":
                    //仕掛置場
                    DisplayFramePage(new TransportList());
                    break;
            }
        }
    }
}
