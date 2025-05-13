using ClassBase;
using ClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;

#pragma warning disable
namespace Display
{
    #region 共通クラス
    //インターフェース
    public interface IBarcode
    {
        void GetQRCode();
    }

    //共通クラス
    public class Common : Shared, INotifyPropertyChanged
    {
        //変数
        ControlMessage messageControl = null;
        ContentControl framePage;
        DataTable selectTable;
        DataRowView selectedItem;
        ISelect iselect;
        int indexNumber;
        int selectedIndex = -1;
        double scrollIndex;
        object focus;
        string receivedData = string.Empty;
        string nextFocus = string.Empty;
        string soundFolder = string.Empty;
        string page = string.Empty;
        string connection = string.Empty;
        string server = string.Empty;
        string processName = string.Empty;
        string processBefore = string.Empty;
        string mark = string.Empty;
        string equipmentCODE = string.Empty;
        string equipmentName = string.Empty;
        string worker = string.Empty;
        string processWork = string.Empty;
        string productName = string.Empty;
        string shapeName = string.Empty;
        string mode = string.Empty;
        string coil = string.Empty;
        bool isMessage;
        List<string> equipmentCODES;
        List<string> workers;
        List<string> workProcesses;

        //プロパティ
        public INIFile IniFile                                  //設定ファイル
        { get; set; } = new INIFile(CONST.SETTING_INI);
        public ControlMessage MessageControl                    //メッセージコントロール
        {
            get => messageControl;
            set
            {
                messageControl = value;
                IsMessage = value == null ? false : true;
                WindowProperty.IsMessage = IsMessage;
            }
        }
        public PropertyWindow WindowProperty                    //プロパティ（PropertyWindow）
        { get; set; }
        public PropertyMessage MessageProperty                  //プロパティ（PropertyMessage）
        { get; set; }
        public PropertyTenKey TenKeyProperty                    //プロパティ（PropertyTenKey）
        { get; set; }
        public PropertyWorker WorkerProperty                    //プロパティ（PropertyWorker）
        { get; set; }
        public PropertyWorkProcess WorkProcessProperty          //プロパティ（PropertyWorkProcess）
        { get; set; }
        public PropertyDefect DefectProperty                    //プロパティ（PropertyDefect）
        { get; set; }
        public PropertyDefectCategory DefectCategoryProperty    //プロパティ（PropertyDefect）
        { get; set; }
        public IBarcode Ibarcode                                //インターフェース（QRコード）
        { get; set; }
        public ISelect Iselect                                  //インターフェース（一覧データ）
        {
            get => iselect;
            set => SetProperty(ref iselect, value);
        }
        public DataTable SelectTable                            //一覧データ
        {
            get => selectTable;
            set => SetProperty(ref selectTable, value);
        }
        public DataRowView SelectedItem                         //選択した行
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
        public int IndexNumber                                  //表示データの行番号
        {
            get => indexNumber;
            set => SetProperty(ref indexNumber, value);
        }
        public int SelectedIndex                                //行選択
        {
            get => selectedIndex;
            set => SetProperty(ref selectedIndex, value);
        }
        public double ScrollIndex                               //スクロール位置
        {
            get => scrollIndex;
            set => SetProperty(ref scrollIndex, value);
        }
        public object Focus                                     //フォーカス
        {
            get => focus;
            set => SetProperty(ref focus, value);
        }
        public string ReceivedData                              //COMポートからの値
        {
            get => receivedData;
            set
            {
                receivedData = value;
                if (Ibarcode != null) { Ibarcode.GetQRCode(); }
            }
        }
        public string NextFocus                                 //次のフォーカス
        {
            get => nextFocus;
            set => SetProperty(ref nextFocus, value);
        }
        public string SoundFolder                               //サウンドフォルダ
        {
            get => FOLDER.ApplicationPath() + @"Sound\";
        }
        public string Page                                      //遷移するページ
        {
            get => page;
            set => SetProperty(ref page, value);
        }
        public string Connection                                //接続文字列
        {
            get => connection;
            set => SetProperty(ref connection, value);
        }
        public string Server                                    //サーバーIP
        {
            get => server;
            set => SetProperty(ref server, value);
        }
        public string ProcessName                               //工程区分
        {
            get => processName;
            set
            {
                SetProperty(ref processName, value);

                var process = new ProcessCategory(value);
                Mark = process.Mark;
                ProcessBefore = process.Before;

                ListSource.Process = value;
                Workers = ListSource.Workers;
                EquipmentCODES = ListSource.Equipments;
            }
        }
        public string ProcessBefore                             //前工程
        {
            get => processBefore;
            set => SetProperty(ref processBefore, value);
        }
        public string Mark                                      //接頭文字
        {
            get => mark;
            set => SetProperty(ref mark, value);
        }
        public string EquipmentCODE                             //設備CODE
        {
            get => equipmentCODE;
            set
            {
                SetProperty(ref equipmentCODE, value);

                Equipment equipment = new Equipment();
                equipment.EquipmentCODE = value;
                EquipmentName = equipment.EquipmentName;
            }
        }
        public string EquipmentName                             //設備名
        {
            get => equipmentName;
            set => SetProperty(ref equipmentName, value);
        }
        public string Worker                                    //担当者
        {
            get => worker;
            set => SetProperty(ref worker, value);
        }
        public string ProcessWork                               //工程表示
        {
            get => processWork;
            set => SetProperty(ref processWork, value);
        }
        public string ProductName                               //品番
        {
            get => productName;
            set
            {
                SetProperty(ref productName, value);

                if (!string.IsNullOrEmpty(value) && ProductName != value)
                {
                    var Sound = new SoundPlay();
                    Sound.PlayAsync(SoundFolder + CONST.SOUND_LOT);
                }
            }
        }
        public string ShapeName                                 //形式
        {
            get => shapeName;
            set => SetProperty(ref shapeName, value);
        }
        public string Mode                                      //入力状況
        {
            get => mode;
            set => SetProperty(ref mode, value);
        }
        public string Coil                                      //コイル数
        {
            get => coil;
            set => SetProperty(ref coil, value);
        }
        public bool IsMessage                                   //メッセージボックスを開いているかどうか
        {
            get => isMessage;
            set => isMessage = value;
        }
        public List<string> EquipmentCODES                      //設備コンボボックス
        {
            get => equipmentCODES;
            set => SetProperty(ref equipmentCODES, value);
        }
        public List<string> Workers                             //作業者コンボボックス
        {
            get => workers;
            set => SetProperty(ref workers, value);
        }
        public List<string> WorkProcesses                       //工程リスト
        {
            get => workProcesses;
            set => SetProperty(ref workProcesses, value);
        }

        //INIファイル読み込み
        public void ReadINI()
        {
            Connection = IniFile.GetString("Database", "ConnectString");
            Server = GetServerIP(Connection);
            Page = IniFile.GetString("Page", "Initial");
            ProcessName = IniFile.GetString("Page", "Process");
            EquipmentCODE = IniFile.GetString("Page", "Equipment");
            Worker = IniFile.GetString("Page", "Worker");
            Mode = IniFile.GetString("Manufacture", "Mode");
        }

        //スタートページを表示
        public void StartPage(string page, string code = null, string lotnumber = null)
        {
            var windowProperty = new PropertyWindow();
            var type = Type.GetType("Display." + page);

            switch (page)
            {
                case "ManufactureInfo": case "InProcessInfo":
                    windowProperty.FramePage = (ContentControl)Activator.CreateInstance(type, code, lotnumber);
                    break;

                case "InProcessList": case "ManufactureList":
                    windowProperty.FramePage = (ContentControl)Activator.CreateInstance(type, DateTime.Now.ToString("yyyyMMdd"));
                    break;

                case "PackSpecification":
                    windowProperty.FramePage = (ContentControl)Activator.CreateInstance(type, string.Empty, string.Empty);
                    break;

                default:
                    windowProperty.FramePage = (ContentControl)Activator.CreateInstance(type);
                    break;
            }
        }

        //ページ移動
        public void DisplayFramePage(object framepage)
        {
            var windowProperty = new PropertyWindow();
            windowProperty.FramePage = (ContentControl)framepage;
        }

        //省略ロット番号取得
        public string GetLotNumber(string code)
        {
            var management = new Management();
            return management.GetLotNumber(code);
        }

        //ロット番号処理
        public void DisplayLot(string lotnumber, string inProcesscode = "")
        {
            lotnumber = GetLotNumber(lotnumber);
            var management = new Management();
            management.ProcessName = ProcessName;
            management.LotNumber = lotnumber;
            CopyProperty(management, this);

            //コイル数取得
            var inProcess = new InProcess();
            Coil = inProcess.InProcessCoil(lotnumber, inProcesscode);
        }

        //仕上記号単位の件数・枚数を算出
        public DataTable CalculatePage(DataTable datatable)
        {
            var query = from a in datatable.AsEnumerable()
                        orderby a.Field<string>("仕上記号") descending
                        group a by a.Field<string>("仕上記号") into b
                        select new
                        {
                            仕上記号 = b.Max(a => a.Field<string>("仕上記号")),
                            件数 = b.Count(),
                            枚数 = Math.Ceiling((double)b.Count() / 3),
                            色 = b.Max(a => a.Field<string>("色")),
                            文字色 = b.Max(a => a.Field<string>("文字色")),
                            ボーダー色 = b.Max(a => a.Field<string>("ボーダー色")),
                        };
            return query.CreateDataTable();
        }
    }
    #endregion

    #region 印刷処理
    public class PrintPage : Common, ISerialPort
    {
        //プロパティ
        public string PrintText                     //印刷画面名
        { get; set; }
        public PageMediaSize PaperSize              //用紙サイズ
        { get; set; }
        public PageOrientation PaperOrientation     //用紙の向き（縦：Portrait/横：Landscape）  
        { get; set; }
        public FixedDocument Document               //印刷データ
        { get; set; }
        public FixedPage Page                       //印刷ページ
        { get; set; }

        //印刷処理
        public void Print()
        {
            //各種オブジェクトの生成
            var localprintserver = new LocalPrintServer();
            var queue = localprintserver.DefaultPrintQueue;

            //用紙設定
            var printticket = queue.DefaultPrintTicket;
            printticket.PageMediaSize = PaperSize;
            printticket.PageOrientation = PaperOrientation;

            // 印刷します。
            var writer = PrintQueue.CreateXpsDocumentWriter(queue);
            writer.Write(Page, printticket);

        }
    }
    #endregion
}