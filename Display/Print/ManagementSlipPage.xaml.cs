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

    public partial class ManagementSlipPage : UserControl
    {
        public ManagementSlipPage(DataTable datatable)
        {
            DataContext = new ViewModelManagementSlipPage(datatable);
            InitializeComponent();
        }
    }

    public class ViewModelManagementSlipPage : Common
    {
        //変数
        string[] lotNumber = new string[3];
        string[] productName = new string[3];
        string[] planNumber = new string[3];
        string[] slipSEQ = new string[3];
        string[] slipAll = new string[3];
        string[] shirringUnit = new string[3];
        string[] ironType = new string[3];
        string[] powderType = new string[3];
        string[] measurement = new string[3];
        string[] coilWeight = new string[3];
        string[] unit = new string[3];
        string[] pressRNO = new string[3];
        string[] press = new string[3];
        string[] markCompletion = new string[3];
        string[] flgAlloy = new string[3];
        string[] flgFirst = new string[3];
        string[] barcode = new string[3];

        //プロパティ
        public string[] LotNumber           //ロット番号
        {
            get => lotNumber;
            set => SetProperty(ref lotNumber, value);
        }
        public string[] ProductName         //品番
        {
            get => productName;
            set => SetProperty(ref productName, value);
        }
        public string[] PlanNumber          //ロット数
        {
            get => planNumber;
            set => SetProperty(ref planNumber, value);
        }
        public string[] SlipSEQ             //現品票番号
        {
            get => slipSEQ;
            set => SetProperty(ref slipSEQ, value);
        }
        public string[] SlipAll             //現品票番号
        {
            get => slipAll;
            set => SetProperty(ref slipAll, value);
        }
        public string[] ShirringUnit        //コイル数
        {
            get => shirringUnit;
            set => SetProperty(ref shirringUnit, value);
        }
        public string[] IronType            //鉄種
        {
            get => ironType;
            set => SetProperty(ref ironType, value);
        }
        public string[] PowderType          //粉種
        {
            get => powderType;
            set => SetProperty(ref powderType, value);
        }
        public string[] Measurement         //寸法
        {
            get => measurement;
            set => SetProperty(ref measurement, value);
        }
        public string[] CoilWeight          //コイル重量
        {
            get => coilWeight;
            set => SetProperty(ref coilWeight, value);
        }
        public string[] Unit                //単位表示（枚・C）
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }
        public string[] PressRNO            //行先・RNO
        {
            get => pressRNO;
            set => SetProperty(ref pressRNO, value);
        }
        public string[] Press               //プレス工程
        {
            get => press;
            set => SetProperty(ref press, value);
        }
        public string[] MarkCompletion      //仕上記号
        {
            get => markCompletion;
            set => SetProperty(ref markCompletion, value);
        }
        public string[] FlgAlloy            //裏金付着表示
        {
            get => flgAlloy;
            set => SetProperty(ref flgAlloy, value);
        }
        public string[] FlgFirst            //初回品
        {
            get => flgFirst;
            set => SetProperty(ref flgFirst, value);
        }
        public string[] Barcode             //バーコード
        {
            get => barcode;
            set => SetProperty(ref barcode, value);
        }

        //コンストラクター
        public ViewModelManagementSlipPage(DataTable datatable)
        {
            var barCode = new BarCode();
            barCode.SetBarcodeFolder();

            var i = 0;
            foreach (DataRow row in datatable.Rows) 
            { 
                LotNumber[i] = row["ロット番号"].ToTrim();
                ProductName[i] = row["品番"].ToTrim();
                PlanNumber[i] = row["計画数"].ToTrim();
                IronType[i] = row["鉄種"].ToTrim();
                PowderType[i] = row["粉種"].ToTrim();
                Measurement[i] = row["鉄板規格"].ToTrim().Replace(" ","");
                CoilWeight[i] = row["コイル重量"].ToTrim();
                Press[i] = row["プレス工程"].ToTrim();
                FlgAlloy[i] = row["裏金表記"].ToTrim();
                FlgFirst[i] = row["初回品"].ToTrim() == "*" ? "初回品" : string.Empty;
                PressRNO[i] = row["プレス行き"].ToTrim() + row["RNO"].ToTrim();

                //形状
                var seq = string.Empty;
                switch (row["形状"].ToTrim())
                {
                    case "シート":
                        Unit[i] = "枚";
                        SlipSEQ[i] = string.Empty;
                        SlipAll[i] = string.Empty;
                        ShirringUnit[i] = row["数量"].ToTrim();
                        seq = row["SEQ"].ToTrim();
                        break;

                    case "コイル":
                        Unit[i] = "C";
                        SlipSEQ[i] = row["SEQ"].ToTrim().ToCircleEnclosing();
                        SlipAll[i] = row["数量"].ToTrim().ToCircleEnclosing();
                        ShirringUnit[i] = row["数量"].ToTrim().ToCircleEnclosing();
                        seq = row["SEQ"].ToTrim();
                        break;
                }
                Barcode[i] = FOLDER.ApplicationPath() + @"\" + barCode.GenerateBarcode(BarcodeFormat.QR_CODE, LotNumber[i] + "-" + seq, 100, 100);
                i++;
                if (i > 2) { break; }
            }
        }
    }
}
