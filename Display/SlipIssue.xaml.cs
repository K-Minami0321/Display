using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System.Data;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        string processLabel;
        bool visibleWeight;
        bool visibleComment;

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
        public string ProcessLabel          //表示ラベル
        {
            get => processLabel;
            set => SetProperty(ref processLabel, value);
        }
        public bool VisibleWeight           //コイル重量表示
        {
            get => visibleWeight;
            set => SetProperty(ref visibleWeight, value);
        }
        public bool VisibleComment          //備考の表示
        {
            get => visibleComment;
            set => SetProperty(ref visibleComment, value);
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
            Initialize();
        }

        //初期化
        private void Initialize()
        {
            LotNumber = string.Empty;
            LotNumberSEQ = string.Empty;
            ProductName = "現品票のQRコードを読み込む";
            PlanNumber = string.Empty;
            ShirringUnit = string.Empty;
            IronType = string.Empty;
            PowderType = string.Empty;
            Measurement = string.Empty;
            CoilWeight = string.Empty;
            Unit = string.Empty;
            VisibleWeight = false;
            PageTable = null;
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
                VisibleQRcode = true,
                Process = ProcessName,
                ProcessWork = "現品票発行"
            };

            switch (ProcessName)
            {
                case "合板":
                    ProcessLabel = "素材課にて";
                    break;

                case "プレス":
                    ProcessLabel = "製造課プレスにて";
                    break;

                case "仕上":
                    ProcessLabel = "製造課仕上にて";
                    break;
            }
            VisibleComment = ProcessName == "合板";
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
        public async void KeyDown(object value)
        {
            switch (value)
            {
                case "Print":

                    if (string.IsNullOrEmpty(LotNumber)) { return; }

                    MessageControl = new ControlMessage();
                    MessageProperty = new PropertyMessage()
                    {
                        Message = "現品票を印刷します",
                        Contents = "カラー用紙をセットしOKボタンを押してください。",
                        Type = "警告"
                    };
                    var messege = (bool)await DialogHost.Show(MessageControl);

                    if (messege)
                    {
                        //印刷フォーマット
                        UIElement report;
                        switch (ProcessName)
                        {
                            case "合板":
                                report = new SlipBoard(SelectTable);
                                break;

                            case "プレス":
                                report = new SlipPress(SelectTable);
                                break;

                            case "仕上":
                                report = new SlipCompletion(SelectTable);
                                break;

                            default:
                                report = new SlipInspection(SelectTable);
                                break;
                        }

                        //印刷処理
                        var pageContent = new PageContent();
                        var printPage = new PrintPage();
                        printPage.PaperSize = new PageMediaSize(PageMediaSizeName.ISOA4);
                        printPage.PaperOrientation = PageOrientation.Landscape;
                        printPage.Document = new FixedDocument();
                        printPage.Page = new FixedPage() { Width = CONST.PRINT_LONG, Height = CONST.PRINT_SHORT };
                        printPage.Page.Children.Add(report);
                        pageContent.Child = printPage.Page;
                        printPage.Document.Pages.Add(pageContent);
                        printPage.Print();
                    }
                    MessageControl = null;
                    break;

                case "QRcode":

                    //初期化
                    Initialize();
                    break;
            }
        }
    }
}
