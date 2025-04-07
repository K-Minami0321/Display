using ClassBase;
using ClassLibrary;
using MathNet.Numerics.Financial;
using Microsoft.Xaml.Behaviors.Core;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System.Data;
using System.Diagnostics.Metrics;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class SlipIssue : UserControl
    {
        public SlipIssue()
        {
            DataContext = new ViewModelSlipIssue();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelSlipIssue : Common, IWindowBase, ISelect, IBarcode
    {
        //変数
        ManagementSlip managementSlip = new ManagementSlip();
        DataTable pageTable;
        string lotNumber;
        string lotNumberSEQ;
        string planNumber;
        string shirringUnit;
        string ironType;
        string powderType;
        string measurement;
        string coilWeight;
        string unit;
        bool visibleWeight;

        //プロパティ
        public DataTable PageTable          //印刷用紙カラー
        {
            get => pageTable;
            set => SetProperty(ref pageTable, value);
        }
        public string LotNumber             //ロット番号
        {
            get => lotNumber;
            set => SetProperty(ref lotNumber, value);
        }
        public string LotNumberSEQ          //SEQ
        {
            get => lotNumberSEQ;
            set => SetProperty(ref lotNumberSEQ, value);
        }
        public string PlanNumber            //計画数
        {
            get => planNumber;
            set => SetProperty(ref planNumber, value);
        }
        public string ShirringUnit          //コイル数
        {
            get => shirringUnit;
            set => SetProperty(ref shirringUnit, value);
        }
        public string IronType              //鉄種
        {
            get => ironType;
            set => SetProperty(ref ironType, value);
        }
        public string PowderType            //粉種
        {
            get => powderType;
            set => SetProperty(ref powderType, value);
        }
        public string Measurement           //寸法
        {
            get => measurement;
            set => SetProperty(ref measurement, value);
        }
        public string CoilWeight            //コイル重量
        {
            get => coilWeight;
            set => SetProperty(ref coilWeight, value);
        }
        public string Unit                  //単位表示（枚・C）
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }
        public bool VisibleWeight           //コイル重量表示
        {
            get => visibleWeight;
            set => SetProperty(ref visibleWeight, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;

        //コンストラクター
        public ViewModelSlipIssue()
        {
            Ibarcode = this;
            Iselect = this;

            ReadINI();
            SelectedIndex = -1;
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //WindowMain
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                VisibleList = false,
                VisibleInfo = false,
                VisibleDefect = false,
                VisibleArrow = false,
                VisiblePlan = false,
                VisiblePrinter = true,
                Process = ProcessName,
                ProcessWork = "現品票発行"
            };
        }

        //選択処理
        public void SelectList() { return; }

        //QRコード処理
        public void GetQRCode()
        {
            //ロット番号
            if (CONVERT.IsLotNumber(ReceivedData))
            {
                LotNumber = ReceivedData.StringLeft(10);
                LotNumberSEQ = ReceivedData.StringRight(ReceivedData.Length - 11);
                SelectTable = managementSlip.Select(LotNumber);
            }
            PageTable = CalculatePage(SelectTable);

            //表示
            foreach (DataRow datarow in SelectTable.Rows)
            {
                LotNumber = datarow["ロット番号"].ToTrim();
                LotNumberSEQ = datarow["SEQ"].ToTrim();
                ProductName = datarow["品番"].ToTrim();
                PlanNumber = datarow["計画数"].ToTrim();
                IronType = datarow["鉄種"].ToTrim();
                PowderType = datarow["粉種"].ToTrim();
                Measurement = datarow["鉄板規格"].ToTrim();
                CoilWeight = datarow["コイル重量"].ToTrim();
                ShirringUnit = datarow["数量"].ToTrim();
                Unit = datarow["形状"].ToTrim() == "シート" ? "枚" : "Ｃ";
                VisibleWeight = datarow["形状"].ToTrim() == "コイル" ? true : false;
            }
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":

                    break;
            }
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {



            }
        }
    }
}
