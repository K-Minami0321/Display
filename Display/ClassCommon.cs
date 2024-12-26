using ClassBase;
using ClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


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
        public INIFile IniFile                  //iniファイル
        { get; set; } = new INIFile(CONST.SETTING_INI);
        public SoundPlay Sound                  //WAVE再生処理
        { get; set; } = new SoundPlay();
        public ContentControl FramePage         //画面ページ
        {
            get { return framePage; }
            set { SetProperty(ref framePage, value); }
        }
        public DataTable SelectTable            //一覧データ
        {
            get { return selectTable; }
            set { SetProperty(ref selectTable, value); }
        }
        public DataRowView SelectedItem         //選択した行
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }
        public int SelectedIndex                //行選択
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value); }
        }
        public double ScrollIndex               //スクロール位置
        {
            get { return scrollIndex; }
            set { SetProperty(ref scrollIndex, value); }
        }
        public object Focus                     //フォーカス
        {
            get { return focus; }
            set { SetProperty(ref focus, value); }
        }
        public string NextFocus                 //次のフォーカス
        {
            get { return nextFocus; }
            set { SetProperty(ref nextFocus, value); }
        }
        public string SoundFolder               //サウンドフォルダ
        {
            get { return STRING.Empty(FOLDER.ApplicationPath()) + @"Sound\"; }
        }
        public string Page                      //遷移するページ
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }
        public string Connection                //接続文字列
        {
            get { return connection; }
            set { SetProperty(ref connection, value); }
        }
        public string Server                    //サーバーIP
        {
            get { return server; }
            set { SetProperty(ref server, value); }
        }
        public string ProcessName               //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);

                ProcessCategory process = new ProcessCategory(value);
                Mark = process.Mark;
                ProcessBefore = process.Before;

                ListSource listSource = new ListSource();
                listSource.Process = value;
                Workers = listSource.Workers;
                EquipmentCODES = listSource.Equipments;
            }
        }
        public string ProcessBefore             //前工程
        {
            get { return processBefore; }
            set { SetProperty(ref processBefore, value); }
        }
        public string Mark                      //接頭文字
        {
            get { return mark; }
            set { SetProperty(ref mark, value); }
        }
        public string EquipmentCODE             //設備CODE
        {
            get { return equipmentCODE; }
            set
            {
                SetProperty(ref equipmentCODE, value);
                Equipment equipment = new Equipment(value);
                EquipmentName = equipment.EquipmentName;
            }
        }
        public string EquipmentName             //設備名
        {
            get { return equipmentName; }
            set { SetProperty(ref equipmentName, value); }
        }
        public string Worker                    //担当者
        {
            get { return worker; }
            set { SetProperty(ref worker, value); }
        }
        public string ProcessWork               //工程
        {
            get { return processWork; }
            set { SetProperty(ref processWork, value); }
        }
        public string ProductName               //品番
        {
            get { return productName; }
            set
            {
                if (!string.IsNullOrEmpty(value) && ProductName != value) { Sound.PlayAsync(SoundFolder + CONST.SOUND_LOT); }
                SetProperty(ref productName, value);
            }
        }
        public string ShapeName                 //形式
        {
            get { return shapeName; }
            set { SetProperty(ref shapeName, value); }
        }
        public string Coil                      //コイル数
        {
            get { return coil; }
            set { SetProperty(ref coil, value); }
        }
        public List<string> EquipmentCODES      //設備コンボボックス
        {
            get { return equipmentCODES; }
            set { SetProperty(ref equipmentCODES, value); }
        }
        public List<string> Workers             //作業者コンボボックス
        {
            get { return workers; }
            set { SetProperty(ref workers, value); }
        }
        public List<string> WorkProcesses       //工程リスト
        {
            get { return workProcesses; }
            set { SetProperty(ref workProcesses, value); }
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
        }

        //スタートページを表示
        public void StartPage(string page)
        {
            Type type = Type.GetType("Display." + page);
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
            Management management = new Management();
            return management.GetLotNumber(code);
        }

        //ロット番号処理
        public void DisplayLot(string lotnumber, string inProcesscode = "")
        {
            lotnumber = GetLotNumber(lotnumber);
            Management management = new Management(lotnumber, ProcessName);
            CopyProperty(management, this);
            shape = new ProductShape(ShapeName);


            //コイル数取得
            InProcess inProcess = new InProcess();
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
