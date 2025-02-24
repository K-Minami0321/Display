using ClassLibrary;
using System.Data;
using System.Windows.Controls;
using ZXing;

//---------------------------------------------------------------------------------------
//
//  現品票
//
//
//

#pragma warning disable
namespace Display
{

    public partial class SlipInspection : UserControl
    {
        public SlipInspection(DataTable datatable)
        {
            DataContext = new ViewModelSlipInspection(datatable);
            InitializeComponent();
        }
    }

    public class ViewModelSlipInspection : Common
    {
        //変数
        string lotNumber = string.Empty;
        string productName = string.Empty;
        string planNumber = string.Empty;
        string slipSEQ = string.Empty;
        string slipAll = string.Empty;
        string shirringUnit = string.Empty;
        string ironType = string.Empty;
        string powderType = string.Empty;
        string measurement = string.Empty;
        string coilWeight = string.Empty;
        string unit = string.Empty;
        string pressRNO = string.Empty;
        string press = string.Empty;
        string markCompletion = string.Empty;
        string flgAlloy = string.Empty;
        string flgFirst = string.Empty;
        string barcode = string.Empty;

        //プロパティ
        public string LotNumber           //ロット番号
        {
            get => lotNumber;
            set => SetProperty(ref lotNumber, value);
        }
        public string ProductName         //品番
        {
            get => productName;
            set => SetProperty(ref productName, value);
        }
        public string PlanNumber          //ロット数
        {
            get => planNumber;
            set => SetProperty(ref planNumber, value);
        }
        public string SlipSEQ             //現品票番号
        {
            get => slipSEQ;
            set => SetProperty(ref slipSEQ, value);
        }
        public string SlipAll             //現品票番号
        {
            get => slipAll;
            set => SetProperty(ref slipAll, value);
        }
        public string ShirringUnit        //コイル数
        {
            get => shirringUnit;
            set => SetProperty(ref shirringUnit, value);
        }
        public string IronType            //鉄種
        {
            get => ironType;
            set => SetProperty(ref ironType, value);
        }
        public string PowderType          //粉種
        {
            get => powderType;
            set => SetProperty(ref powderType, value);
        }
        public string Measurement         //寸法
        {
            get => measurement;
            set => SetProperty(ref measurement, value);
        }
        public string CoilWeight          //コイル重量
        {
            get => coilWeight;
            set => SetProperty(ref coilWeight, value);
        }
        public string Unit                //単位表示（枚・C）
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }
        public string PressRNO            //行先・RNO
        {
            get => pressRNO;
            set => SetProperty(ref pressRNO, value);
        }
        public string Press               //プレス工程
        {
            get => press;
            set => SetProperty(ref press, value);
        }
        public string MarkCompletion      //仕上記号
        {
            get => markCompletion;
            set => SetProperty(ref markCompletion, value);
        }
        public string FlgAlloy            //裏金付着表示
        {
            get => flgAlloy;
            set => SetProperty(ref flgAlloy, value);
        }
        public string FlgFirst            //初回品
        {
            get => flgFirst;
            set => SetProperty(ref flgFirst, value);
        }
        public string Barcode             //バーコード
        {
            get => barcode;
            set => SetProperty(ref barcode, value);
        }

        //コンストラクター
        public ViewModelSlipInspection(DataTable datatable)
        {
            var barCode = new BarCode();
            barCode.SetBarcodeFolder();

            foreach (DataRow row in datatable.Rows) 
            { 
                LotNumber = row["ロット番号"].ToTrim();
                ProductName = row["品番"].ToTrim();
                PlanNumber = row["計画数"].ToTrim();
                IronType = row["鉄種"].ToTrim();
                PowderType = row["粉種"].ToTrim();
                Measurement = row["鉄板規格"].ToTrim().Replace(" ","");
                CoilWeight = row["コイル重量"].ToTrim();
                Press = row["プレス工程"].ToTrim();
                FlgAlloy = row["裏金表記"].ToTrim();
                FlgFirst = row["初回品"].ToTrim() == "*" ? "初回品" : string.Empty;
                PressRNO = row["プレス行き"].ToTrim() + row["RNO"].ToTrim();

                //形状
                var seq = string.Empty;
                switch (row["形状"].ToTrim())
                {
                    case "シート":
                        Unit = "枚";
                        SlipSEQ = string.Empty;
                        SlipAll = string.Empty;
                        ShirringUnit = row["数量"].ToTrim();
                        seq = row["SEQ"].ToTrim();
                        break;

                    case "コイル":
                        Unit = "C";
                        SlipSEQ = row["SEQ"].ToTrim().ToCircleEnclosing();
                        SlipAll = row["数量"].ToTrim().ToCircleEnclosing();
                        ShirringUnit = row["数量"].ToTrim().ToCircleEnclosing();
                        seq = row["SEQ"].ToTrim();
                        break;
                }
                Barcode = FOLDER.ApplicationPath() + @"\" + barCode.GenerateBarcode(BarcodeFormat.QR_CODE, LotNumber + "-" + seq, 100, 100);
            }
        }
    }
}
