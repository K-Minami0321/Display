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
    public partial class Setting : Page
    {
        public static Setting Instance
        { get; set; }
        public Setting()
        {
            DataContext = new ViewModelSetting();
            Instance = this;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelSetting : Common, IKeyDown
    {
        //プロパティ変数
        string _Version;
        string _Connection;
        string _Server;
        string _ProcessName;
        string _Worker;
        string _LogText;
        string _Log = CONST.SQL_LOG;
        bool _IsServer;
        bool _IsProcessName;
        bool _IsEquipment;
        bool _IsWorker;
        bool _IsServerOpen;
        bool _IsProcessOpen;
        bool _IsEquipmentOpen;
        bool _IsWorkerOpen;
        List<string> _ProcessNames;
        List<string> _Workers;
        List<string> _EquipmentCODES;
        List<string> _Servers;

        //プロパティ
        public static ViewModelSetting Instance     //インスタンス
        { get; set; } = new ViewModelSetting();
        public string Version                       //バージョン
        {
            get { return _Version; }
            set { SetProperty(ref _Version, value); }
        }
        public string Connection                    //接続文字列
        {
            get { return _Connection; }
            set { SetProperty(ref _Connection, value); }
        }
        public string Server                        //サーバーIP
        {
            get { return _Server; }
            set { SetProperty(ref _Server, value); }
        }
        public string ProcessName                   //工程区分
        {
            get { return _ProcessName; }
            set 
            { 
                SetProperty(ref _ProcessName, value);

                iProcess = ProcessCategory.SetProcess(value);
                EquipmentCODES = iProcess.Equipments;
                Workers = iProcess.Workers;
            }
        }
        public string Worker                        //担当者
        {
            get { return _Worker; }
            set { SetProperty(ref _Worker, value); }
        }
        public string LogText                       //表示ログ
        {
            get { return _LogText; }
            set { SetProperty(ref _LogText, value); }
        }
        public string Log                           //ログファイル
        {
            get { return _Log; }
            set 
            { 
                SetProperty(ref _Log, value);
                LogText = string.Empty; DisplayLog();
            }
        }
        public bool IsServer                        //サーバーコンボボックス
        {
            get { return _IsServer; }
            set { SetProperty(ref _IsServer, value); }
        }
        public bool IsProcessName                   //工程区分コンボボックス
        {
            get { return _IsProcessName; }
            set { SetProperty(ref _IsProcessName, value); }
        }
        public bool IsEquipment                     //設備コンボボックス
        {
            get { return _IsEquipment; }
            set { SetProperty(ref _IsEquipment, value); }
        }
        public bool IsWorker                        //作業者コンボボックス
        {
            get { return _IsWorker; }
            set { SetProperty(ref _IsWorker, value); }
        }
        public bool IsServerOpen                    //コンボボックスが開いているかどうか
        {
            get { return _IsServerOpen; }
            set { SetProperty(ref _IsServerOpen, value); }
        }
        public bool IsProcessOpen                   //コンボボックスが開いているかどうか
        {
            get { return _IsProcessOpen; }
            set { SetProperty(ref _IsProcessOpen, value); }
        }
        public bool IsEquipmentOpen                 //コンボボックスが開いているかどうか
        {
            get { return _IsEquipmentOpen; }
            set { SetProperty(ref _IsEquipmentOpen, value); }
        }
        public bool IsWorkerOpen                    //コンボボックスが開いているかどうか
        {
            get { return _IsWorkerOpen; }
            set { SetProperty(ref _IsWorkerOpen, value); }
        }
        public List<string> ProcessNames            //工程区分コンボボックス
        {
            get { return _ProcessNames; }
            set { SetProperty(ref _ProcessNames, value); }
        }
        public List<string> Workers                 //作業者コンボボックス
        {
            get { return _Workers; }
            set { SetProperty(ref _Workers, value); }
        }
        public List<string> EquipmentCODES          //設備コンボボックス
        {
            get { return _EquipmentCODES; }
            set { SetProperty(ref _EquipmentCODES, value); }
        }
        public List<string> Servers                 //サーバーコンボックス
        {
            get { return _Servers; }
            set { SetProperty(ref _Servers, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        internal ViewModelSetting()
        {
            manufacture = new Manufacture();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;

            //初期設定
            DisplayCapution();
            DisplayData();
            DisplayLog();
        }

        //初期化
        private void DisplayCapution()
        {
            //キャプション表示
            ViewModelWindowMain.Instance.ProcessWork = "設定画面";
            ViewModelWindowMain.Instance.ProcessName = "設定";
            Version = CONST.DISPLAY_VERSION;

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisiblePlan = true;
            ViewModelWindowMain.Instance.VisibleList = false;
            ViewModelWindowMain.Instance.VisibleInfo = true;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.InitializeIcon();

            //初期表示
            ProcessNames = ProcessCategory.ProcessList();       //コンボボックス設定
            Servers = Maintenance.SetServer();                  //サーバー設定
            Setting.Instance.Server.Focus();                    //フォーカス
        }

        //データ表示
        private void DisplayData()
        {
            Connection = INI.GetString("Database", "ConnectString");
            Server = SQL.GetServerIP(Connection);
            ProcessName = INI.GetString("Page", "Process");
            EquipmentCODE = INI.GetString("Page", "Equipment");
            Worker = INI.GetString("Page", "Worker");
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

        //キーイベント
        public async void KeyDown(object value)
        {
            var result = false;
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
                    if (result) { StartPage(); }
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
                    StartPage();
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.ProcessName = ProcessName;
                    ViewModelWindowMain.Instance.FramePage.Navigate(new PlanList());
                    break;
            }
        }

        //登録処理
        public void RegistData()
        {
            //サーバー情報取得
            var server = SQL.GetServerIP(Connection);
            var connection = Connection.Replace(server, Server);

            //INIファイル書き込み
            INI.WriteString("Database", "ConnectString", connection);
            INI.WriteString("Page", "Process", ProcessName);
            INI.WriteString("Page", "Equipment", EquipmentCODE);
            INI.WriteString("Page", "Worker", Worker);
            StartPage();
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    StartPage();
                    break;
            }
        }
    }
}
