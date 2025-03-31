using ClassBase;
using ClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Controls;

#pragma warning disable
namespace Display
{
    //インターフェース
    public interface IBarcode
    {
        void SetBarcode();
    }

    //共通クラス
    public class Common : Shared, INotifyPropertyChanged
    {
        //変数
        ContentControl framePage;
        DataTable selectTable;
        DataRowView selectedItem;
        int indexNumber;
        int selectedIndex = -1;
        double scrollIndex;
        object focus;
        string receivedData;
        string nextFocus;
        string soundFolder;
        string page;
        string connection;
        string server;
        string processName;
        string processBefore;
        string mark;
        string equipmentCODE;
        string equipmentName;
        string worker;
        string processWork;
        string productName;
        string shapeName;
        string mode;
        string coil;
        List<string> equipmentCODES;
        List<string> workers;
        List<string> workProcesses;

        //プロパティ
        public ViewModelWindowMain CtrlWindow           //WindowMainInstance
        { get; set; } = ViewModelWindowMain.Instance;
        public ViewModelControlTenKey CtrlTenKey        //テンキーコントロール
        { get; set; } = ViewModelControlTenKey.Instance;
        public ViewModelControlWorker CtrlWorker        //作業者コントロール
        { get; set; } = ViewModelControlWorker.Instance;


        public INIFile IniFile                          //設定ファイル
        { get; set; } = new INIFile(CONST.SETTING_INI);
        public PropertyMessageControl PropertyMessage   //プロパティ（PropertyMessageControl）
        { get; set; }





        public IBarcode Ibarcode                        //インターフェース（QRコード）
        { get; set; }
        public ContentControl FramePage                 //画面ページ
        {
            get => framePage;
            set => SetProperty(ref framePage, value);
        }
        public DataTable SelectTable                    //一覧データ
        {
            get => selectTable;
            set => SetProperty(ref selectTable, value);
        }
        public DataRowView SelectedItem                 //選択した行
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
        public int IndexNumber                          //表示データの行番号
        {
            get => indexNumber;
            set => SetProperty(ref indexNumber, value);
        }
        public int SelectedIndex                        //行選択
        {
            get => selectedIndex;
            set => SetProperty(ref selectedIndex, value);
        }
        public double ScrollIndex                       //スクロール位置
        {
            get => scrollIndex;
            set => SetProperty(ref scrollIndex, value);
        }
        public object Focus                             //フォーカス
        {
            get => focus;
            set => SetProperty(ref focus, value);
        }
        public string ReceivedData                      //COMポートからの値
        {
            get => receivedData;
            set
            {
                receivedData = value;
                if (Ibarcode != null) { Ibarcode.SetBarcode(); }
            }
        }
        public string NextFocus                         //次のフォーカス
        {
            get => nextFocus;
            set => SetProperty(ref nextFocus, value);
        }
        public string SoundFolder                       //サウンドフォルダ
        {
            get => FOLDER.ApplicationPath() + @"Sound\";
        }
        public string Page                              //遷移するページ
        {
            get => page;
            set => SetProperty(ref page, value);
        }
        public string Connection                        //接続文字列
        {
            get => connection;
            set => SetProperty(ref connection, value);
        }
        public string Server                            //サーバーIP
        {
            get => server;
            set => SetProperty(ref server, value);
        }
        public string ProcessName                       //工程区分
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
        public string ProcessBefore                     //前工程
        {
            get => processBefore;
            set => SetProperty(ref processBefore, value);
        }
        public string Mark                              //接頭文字
        {
            get => mark;
            set => SetProperty(ref mark, value);
        }
        public string EquipmentCODE                     //設備CODE
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
        public string EquipmentName                     //設備名
        {
            get => equipmentName;
            set => SetProperty(ref equipmentName, value);
        }
        public string Worker                            //担当者
        {
            get => worker;
            set => SetProperty(ref worker, value);
        }
        public string ProcessWork                       //工程表示
        {
            get => processWork;
            set => SetProperty(ref processWork, value);
        }
        public string ProductName                       //品番
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
        public string ShapeName                         //形式
        {
            get => shapeName;
            set => SetProperty(ref shapeName, value);
        }
        public string Mode                              //入力状況
        {
            get => mode;
            set => SetProperty(ref mode, value);
        }
        public string Coil                              //コイル数
        {
            get => coil;
            set => SetProperty(ref coil, value);
        }
        public List<string> EquipmentCODES              //設備コンボボックス
        {
            get => equipmentCODES;
            set => SetProperty(ref equipmentCODES, value);
        }
        public List<string> Workers                     //作業者コンボボックス
        {
            get => workers;
            set => SetProperty(ref workers, value);
        }
        public List<string> WorkProcesses               //工程リスト
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
            ProcessWork = string.IsNullOrEmpty(EquipmentName) ? ProcessName + "実績" : EquipmentName + " - " + EquipmentCODE;
            Mode = IniFile.GetString("Manufacture", "Mode");
        }

        //スタートページを表示
        public void StartPage(string page)
        {
            var type = Type.GetType("Display." + page);
            CtrlWindow.FramePage = (ContentControl)Activator.CreateInstance(type);
        }

        //ページ移動
        public void DisplayFramePage(object framepage)
        {
            CtrlWindow.FramePage = (ContentControl)framepage;
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
    }
}
