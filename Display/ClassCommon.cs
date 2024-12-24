using ClassBase;
using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Controls;


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
        string processName;
        string equipmentCODE;
        string equipmentName;
        string worker;
        string processWork;

        //プロパティ
        public INIFile IniFile              //iniファイル
        { get; set; } = new INIFile(CONST.SETTING_INI);
        public SoundPlay Sound              //WAVE再生処理
        { get; set; } = new SoundPlay();
        public ContentControl FramePage     //画面ページ
        {
            get { return framePage; }
            set { SetProperty(ref framePage, value); }
        }
        public DataTable SelectTable        //一覧データ
        {
            get { return selectTable; }
            set { SetProperty(ref selectTable, value); }
        }
        public DataRowView SelectedItem     //選択した行
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }
        public int SelectedIndex            //行選択
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value); }
        }
        public double ScrollIndex           //スクロール位置
        {
            get { return scrollIndex; }
            set { SetProperty(ref scrollIndex, value); }
        }
        public object Focus                 //フォーカス
        {
            get { return focus; }
            set { SetProperty(ref focus, value); }
        }
        public string NextFocus             //次のフォーカス
        {
            get { return nextFocus; }
            set { SetProperty(ref nextFocus, value); }
        }
        public string SoundFolder           //サウンドフォルダ
        {
            get { return STRING.Empty(FOLDER.ApplicationPath()) + @"Sound\"; }
        }

        public string Page                  //遷移するページ
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }
        public string Connection            //接続文字列
        {
            get { return connection; }
            set { SetProperty(ref connection, value); }
        }
        public string Server                //サーバーIP
        {
            get { return server; }
            set { SetProperty(ref server, value); }
        }
        public string ProcessName           //工程区分
        {
            get { return processName; }
            set
            {
                SetProperty(ref processName, value);
                process = new ProcessCategory(value);
            }
        }
        public string EquipmentCODE         //設備CODE
        {
            get { return equipmentCODE; }
            set
            {
                SetProperty(ref equipmentCODE, value);
                equipment = new Equipment(value);
                EquipmentName = equipment.EquipmentName;

            }
        }
        public string EquipmentName         //設備名
        {
            get { return equipmentName; }
            set { SetProperty(ref equipmentName, value); }
        }
        public string Worker                //担当者
        {
            get { return worker; }
            set { SetProperty(ref worker, value); }
        }
        public string ProcessWork           //
        {
            get { return processWork; }
            set { SetProperty(ref processWork, value); }
        }

        //スタートページを表示
        public void StartPage(string page)
        {
            ViewModelWindowMain windowMain = ViewModelWindowMain.Instance;
            Type type = Type.GetType("Display." + page);
            windowMain.FramePage = (ContentControl)Activator.CreateInstance(type);
        }

        //ページ移動
        public void DisplayFramePage(object framepage)
        {
            ViewModelWindowMain windowMain = ViewModelWindowMain.Instance;
            windowMain.FramePage = (ContentControl)framepage;
        }

        //省略ロット番号取得
        public string GetLotNumber(string code)
        {
            Management management = new Management();
            return management.GetLotNumber(code);
        }

        //データ初期化
        public void DataInitialize()
        {
            ManufactureInfo.ManufactureCODE = string.Empty;
            ManufactureInfo.LotNumber = string.Empty;
            InProcessInfo.InProcessCODE = string.Empty;
            InProcessInfo.LotNumber = string.Empty;
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
    }
}
