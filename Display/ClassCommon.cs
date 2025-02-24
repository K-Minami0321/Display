using ClassBase;
using ClassLibrary;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using ZXing.QrCode.Internal;
using ZXing.QrCode;
using ZXing;

#pragma warning disable
namespace Display
{
    //共通クラス
    public class Common : Shared, INotifyPropertyChanged
    {
        //変数
        ContentControl framePage;
        DataTable selectTable;
        DataRowView selectedItem;
        int indexNumber;
        int selectedIndex;
        double scrollIndex;
        object focus;
        string nextFocus;
        string soundFolder;
        string page;
        string connection;
        string server;
        string mode;
        string processName;
        string processBefore;
        string mark;
        string equipmentCODE;
        string equipmentName;
        string worker;
        string processWork;
        string productName;
        string shapeName;
        string coil;
        List<string> equipmentCODES;
        List<string> workers;
        List<string> workProcesses;

        //プロパティ
        public ContentControl FramePage         //画面ページ
        {
            get => framePage;
            set => SetProperty(ref framePage, value);
        }
        public DataTable SelectTable            //一覧データ
        {
            get => selectTable;
            set => SetProperty(ref selectTable, value);
        }
        public DataRowView SelectedItem         //選択した行
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
        public int IndexNumber                  //表示データの行番号
        {
            get => indexNumber;
            set => SetProperty(ref indexNumber, value);
        }
        public int SelectedIndex                //行選択
        {
            get => selectedIndex;
            set => SetProperty(ref selectedIndex, value);
        }
        public double ScrollIndex               //スクロール位置
        {
            get => scrollIndex;
            set => SetProperty(ref scrollIndex, value);
        }
        public object Focus                     //フォーカス
        {
            get => focus;
            set => SetProperty(ref focus, value);
        }
        public string NextFocus                 //次のフォーカス
        {
            get => nextFocus;
            set => SetProperty(ref nextFocus, value);
        }
        public string SoundFolder               //サウンドフォルダ
        {
            get => FOLDER.ApplicationPath() + @"Sound\";
        }
        public string Page                      //遷移するページ
        {
            get => page;
            set => SetProperty(ref page, value);
        }
        public string Connection                //接続文字列
        {
            get => connection;
            set => SetProperty(ref connection, value);
        }
        public string Server                    //サーバーIP
        {
            get => server;
            set => SetProperty(ref server, value);
        }
        public string ProcessName               //工程区分
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
        public string ProcessBefore             //前工程
        {
            get => processBefore;
            set => SetProperty(ref processBefore, value);
        }
        public string Mark                      //接頭文字
        {
            get => mark;
            set => SetProperty(ref mark, value);
        }
        public string EquipmentCODE             //設備CODE
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
        public string EquipmentName             //設備名
        {
            get => equipmentName;
            set => SetProperty(ref equipmentName, value);
        }
        public string Worker                    //担当者
        {
            get => worker;
            set => SetProperty(ref worker, value);
        }
        public string ProcessWork               //工程表示
        {
            get => processWork;
            set => SetProperty(ref processWork, value);
        }
        public string ProductName               //品番
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
        public string ShapeName                 //形式
        {
            get => shapeName;
            set => SetProperty(ref shapeName, value);
        }
        public string Coil                      //コイル数
        {
            get => coil;
            set => SetProperty(ref coil, value);
        }
        public List<string> EquipmentCODES      //設備コンボボックス
        {
            get => equipmentCODES;
            set => SetProperty(ref equipmentCODES, value);
        }
        public List<string> Workers             //作業者コンボボックス
        {
            get => workers;
            set => SetProperty(ref workers, value);
        }
        public List<string> WorkProcesses       //工程リスト
        {
            get => workProcesses;
            set => SetProperty(ref workProcesses, value);
        }

        //INIファイル読み込み
        public void ReadINI()
        {
            var IniFile = new INIFile(CONST.SETTING_INI);
            Connection = IniFile.GetString("Database", "ConnectString");
            Server = GetServerIP(Connection);
            Page = IniFile.GetString("Page", "Initial");
            ProcessName = IniFile.GetString("Page", "Process");
            EquipmentCODE = IniFile.GetString("Page", "Equipment");
            Worker = IniFile.GetString("Page", "Worker");
            ProcessWork = string.IsNullOrEmpty(EquipmentName) ? ProcessName + "実績" : EquipmentName + " - " + EquipmentCODE;
        }

        //スタートページを表示
        public void StartPage(string page)
        {
            var type = Type.GetType("Display." + page);
            ViewModelWindowMain.Instance.FramePage = (ContentControl)Activator.CreateInstance(type);
        }

        //ページ移動
        public void DisplayFramePage(object framepage)
        {
            ViewModelWindowMain.Instance.FramePage = (ContentControl)framepage;
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

        //データ初期化
        public void DataInitialize()
        {
            ManufactureInfo.ManufactureCODE = string.Empty;
            ManufactureInfo.LotNumber = string.Empty;
            InProcessInfo.InProcessCODE = string.Empty;
            InProcessInfo.LotNumber = string.Empty;
        }

        #region バーコード・QRコード
        //バーコード・QRコード
        public class BarCode : Common
        {
            string path = "barcode";

            //バーコード・QRコードの生成
            public string GenerateBarcode(BarcodeFormat format, string text, int width, int height)
            {
                var barcodewriter = new BarcodeWriter
                {
                    Format = format,
                    Options = new QrCodeEncodingOptions
                    {
                        QrVersion = 1,
                        ErrorCorrection = ErrorCorrectionLevel.L,
                        CharacterSet = "UTF-8",
                        Width = width,
                        Height = height,
                        Margin = 2,
                        PureBarcode = true
                    },
                };

                //バーコード・QRコード画像保存
                var file = string.Empty;
                using (var barcodeBitmap = barcodewriter.Write(text))
                {
                    file = path + @"\barcode" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                    barcodeBitmap.Save(file, ImageFormat.Png);
                }
                return file;
            }

            //バーコード保存フォルダ
            public void SetBarcodeFolder()
            {
                try
                {
                    //フォルダ設定
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    foreach (FileInfo getfile in new DirectoryInfo(path).GetFiles())
                    {
                        if ((getfile.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) { getfile.Attributes = FileAttributes.Normal; }
                        getfile.Delete();
                    }
                }
                catch
                {
                    //エラー処理
                }
            }
        }
        #endregion
    }
}
