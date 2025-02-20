using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class Setting : UserControl
    {
        public Setting()
        {
            DataContext = new ViewModelSetting();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelSetting : Common, IKeyDown
    {
        //変数
        string version;
        string processName;
        string logText;
        string log = CONST.SQL_LOG;
        bool isServer;
        bool isProcessName;
        bool isEquipment;
        bool isWorker;
        bool isServerOpen;
        bool isProcessOpen;
        bool isEquipmentOpen;
        bool isWorkerOpen;
        bool isFocusServer;
        List<string> servers;
        List<string> processNames;

        //プロパティ
        public string Version                   //バージョン
        {
            get => version;
            set => SetProperty(ref version, value);
        }
        public string LogText                   //表示ログ
        {
            get => logText;
            set => SetProperty(ref logText, value);
        }
        public string Log                       //ログファイル
        {
            get => log;
            set 
            { 
                SetProperty(ref log, value);
                LogText = string.Empty; DisplayLog();
            }
        }
        public bool IsServer                    //サーバーコンボボックス
        {
            get => isServer;
            set => SetProperty(ref isServer, value);
        }
        public bool IsProcessName               //工程区分コンボボックス
        {
            get => isProcessName;
            set => SetProperty(ref isProcessName, value);
        }
        public bool IsEquipment                 //設備コンボボックス
        {
            get => isEquipment;
            set => SetProperty(ref isEquipment, value);
        }
        public bool IsWorker                    //作業者コンボボックス
        {
            get => isWorker;
            set => SetProperty(ref isWorker, value);
        }
        public bool IsServerOpen                //コンボボックスが開いているかどうか
        {
            get => isServerOpen;
            set => SetProperty(ref isServerOpen, value);
        }
        public bool IsProcessOpen               //コンボボックスが開いているかどうか
        {
            get => isProcessOpen;
            set => SetProperty(ref isProcessOpen, value);
        }
        public bool IsEquipmentOpen             //コンボボックスが開いているかどうか
        {
            get => isEquipmentOpen;
            set => SetProperty(ref isEquipmentOpen, value);
        }
        public bool IsWorkerOpen                //コンボボックスが開いているかどうか
        {
            get => isWorkerOpen;
            set => SetProperty(ref isWorkerOpen, value);
        }
        public bool IsFocusServer               //フォーカス（サーバー設定）
        {
            get => isFocusServer;
            set => SetProperty(ref isFocusServer, value);
        }
        public List<string> Servers             //サーバーコンボックス
        {
            get 
            {
                ListSource listSource = new ListSource();
                return listSource.Servers;
            }
            set => SetProperty(ref servers, value);
        }
        public List<string> ProcessNames        //工程区分コンボボックス
        {
            get 
            {
                ListSource listSource = new ListSource();
                return listSource.Processes;
            }
            set => SetProperty(ref processNames, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        public ViewModelSetting() 
        {
            Initialize();
        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            DisplayLog();
            ReadINI();
        }

        //初期化
        private void DisplayCapution()
        {
            //ボタン設定
            var windowMain = ViewModelWindowMain.Instance;
            windowMain.VisiblePower = true;
            windowMain.VisiblePlan = true;
            windowMain.VisibleList = false;
            windowMain.VisibleInfo = true;
            windowMain.VisibleDefect = false;
            windowMain.VisibleArrow = false;
            windowMain.Ikeydown = this;
            windowMain.InitializeIcon();
            windowMain.ProcessWork = "設定画面";
            windowMain.ProcessName = "設定";
            Version = CONST.DISPLAY_VERSION;
        }

        //初期化
        public void Initialize()
        {
            IsFocusServer = true;                       //フォーカス
        }

        //ログ表示
        private void DisplayLog()
        {
            var file = FOLDER.ApplicationPath() + @"log\" + Log;
            if (File.Exists(file))
            {
                StreamReader reader = new StreamReader(file, Encoding.GetEncoding("utf-8"));
                if (reader != null) { LogText = reader.ReadToEnd(); }
            }  
        }

        //登録処理
        public void RegistData()
        {
            //サーバー情報取得
            var server = GetServerIP(Connection);
            var connection = Connection.Replace(server, Server);

            //INIファイル書き込み
            var IniFile = new INIFile(CONST.SETTING_INI);
            IniFile.WriteString("Database", "ConnectString", connection);
            IniFile.WriteString("Page", "Process", ProcessName);
            IniFile.WriteString("Page", "Equipment", EquipmentCODE);
            IniFile.WriteString("Page", "Worker", Worker);

            ViewModelWindowMain.Instance.ProcessName = IniFile.GetString("Page", "Process");

            //画面遷移先設定
            StartPage(IniFile.GetString("Page", "Initial"));
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    var IniFile = new INIFile(CONST.SETTING_INI);
                    StartPage(IniFile.GetString("Page", "Initial"));
                    break;
            }
        }

        //キーイベント
        public async void KeyDown(object value)
        {
            var result = false;
            var IniFile = new INIFile(CONST.SETTING_INI);

            switch (value)
            {
                case "Regist":
                    //登録
                    result = (bool)await DialogHost.Show(new ControlMessage("登録します", "※入力されたものを反映します。", "警告"));
                    if (result) { RegistData(); }
                    break;

                case "Cancel":
                    //取消
                    result = (bool)await DialogHost.Show(new ControlMessage("修正を破棄します", "※入力されたものは設定に反映されません。", "警告"));
                    if (result) 
                    {
                        StartPage(IniFile.GetString("Page", "Initial")); 
                    }
                    break;

                case "Server":
                    //サーバーコンボボックス
                    IsServer = true;
                    IsServerOpen = true;
                    break;

                case "ProcessName":
                    //工程区分コンボボックス
                    IsProcessName = true;
                    IsProcessOpen = true;
                    break;

                case "Equipment":
                    //設備コンボボックス
                    IsEquipment = true;
                    IsEquipmentOpen = true;
                    break;

                case "Worker":
                    //作業者コンボボックス
                    IsWorker = true;
                    IsWorkerOpen = true;
                    break;

                case "SQL":
                    //SQLログ
                    Log = CONST.SQL_LOG;
                    break;

                case "Error":
                    //Errorログ
                    Log = CONST.ERROR_LOG;
                    break;

                case "Debug":
                    //Debugログ
                    Log = CONST.DEBUG_LOG;
                    break;

                case "Grid":
                    IsServer = IsServerOpen;
                    IsProcessName = IsProcessOpen;
                    IsEquipment = IsEquipmentOpen;
                    IsWorker = IsWorkerOpen;
                    IsServerOpen = false;
                    IsProcessOpen = false;
                    IsEquipmentOpen = false;
                    IsWorkerOpen = false;
                    break;

                case "DisplayInfo":
                    //戻る
                    StartPage(IniFile.GetString("Page", "Initial"));
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    DisplayFramePage(new PlanList());
                    break;
            }
        }
    }
}
