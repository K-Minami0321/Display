using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    public class ViewModelSetting : Common, IKeyDown, IDisposable
    {
        //変数
        CompositeDisposable Disposable                  //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ変数
        string _EquipmentCODE;

        //プロパティ
        public static ViewModelSetting Instance         //インスタンス
        { get; set; } = new ViewModelSetting();



        public ReactivePropertySlim<string> Version                 //バージョン
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Connection              //接続文字列
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Server                  //サーバーIP
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ProcessName             //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Worker                  //担当者
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> LogText                 //表示ログ
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Log                     //ログファイル
        { get; set; } = new ReactivePropertySlim<string>(CONST.SQL_LOG);
        public ReactivePropertySlim<bool> IsServer                  //サーバーコンボボックス
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsProcessName             //工程区分コンボボックス
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsEquipment               //設備コンボボックス
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsWorker                  //作業者コンボボックス
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsServerOpen              //コンボボックスが開いているかどうか
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsProcessOpen             //コンボボックスが開いているかどうか
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsEquipmentOpen           //コンボボックスが開いているかどうか
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsWorkerOpen              //コンボボックスが開いているかどうか
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<List<string>> ProcessNames      //工程区分コンボボックス
        { get; set; } = new ReactivePropertySlim<List<string>>();
        public ReactivePropertySlim<List<string>> Workers           //作業者コンボボックス
        { get; set; } = new ReactivePropertySlim<List<string>>();
        public ReactivePropertySlim<List<string>> EquipmentCODES    //設備コンボボックス
        { get; set; } = new ReactivePropertySlim<List<string>>();
        public ReactivePropertySlim<List<string>> Servers           //サーバーコンボックス
        { get; set; } = new ReactivePropertySlim<List<string>>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                iProcess = ProcessCategory.SetProcess(x);
                EquipmentCODES.Value = Equipment.SetEquipment(x);
                Workers.Value = Employee.SetWorker(x);
            }).AddTo(Disposable);
            Log.Subscribe(x =>
            {
                LogText.Value = string.Empty; DisplayLog();
            }).AddTo(Disposable);
        }

        //コンストラクター
        internal ViewModelSetting()
        {
            manufacture = new Manufacture();
            SetProperty();
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
            ViewModelWindowMain.Instance.ProcessWork.Value = "設定画面";
            ViewModelWindowMain.Instance.ProcessName.Value = "設定";
            Version.Value = CONST.DISPLAY_VERSION;

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisiblePlan.Value = true;
            ViewModelWindowMain.Instance.VisibleList.Value = false;
            ViewModelWindowMain.Instance.VisibleInfo.Value = true;
            ViewModelWindowMain.Instance.VisibleDefect.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = false;
            ViewModelWindowMain.Instance.InitializeIcon();

            //初期表示
            ProcessNames.Value = ProcessCategory.ProcessList();     //コンボボックス設定
            Servers.Value = Maintenance.SetServer();        //サーバー設定
            Setting.Instance.Server.Focus();                //フォーカス
        }

        //データ表示
        private void DisplayData()
        {
            Connection.Value = INI.GetString("Database", "ConnectString");
            Server.Value = SQL.GetServerIP(Connection.Value);
            ProcessName.Value = INI.GetString("Page", "Process");
            EquipmentCODE = INI.GetString("Page", "Equipment");
            Worker.Value = INI.GetString("Page", "Worker");
        }

        //ログ表示
        private void DisplayLog()
        {
            var file = FOLDER.ApplicationPath() + @"log\" + Log.Value;
            if (File.Exists(file))
            {
                StreamReader reader = new StreamReader(file, Encoding.GetEncoding("utf-8"));
                if (reader != null) { LogText.Value = reader.ReadToEnd(); }
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
                    IsServer.Value = true;
                    IsServerOpen.Value = true;
                    break;

                case "ProcessName":
                    //工程区分コンボボックス
                    IsProcessName.Value = true;
                    IsProcessOpen.Value = true;
                    break;

                case "Equipment":
                    //設備コンボボックス
                    IsEquipment.Value = true;
                    IsEquipmentOpen.Value = true;
                    break;

                case "Worker":
                    //作業者コンボボックス
                    IsWorker.Value = true;
                    IsWorkerOpen.Value = true;
                    break;

                case "SQL":
                    //SQLログ
                    Log.Value = CONST.SQL_LOG;
                    break;

                case "Error":
                    //Errorログ
                    Log.Value = CONST.ERROR_LOG;
                    break;

                case "Debug":
                    //Debugログ
                    Log.Value = CONST.DEBUG_LOG;
                    break;

                case "Grid":
                    IsServer.Value = IsServerOpen.Value;
                    IsProcessName.Value = IsProcessOpen.Value;
                    IsEquipment.Value = IsEquipmentOpen.Value;
                    IsWorker.Value = IsWorkerOpen.Value;
                    IsServerOpen.Value = false;
                    IsProcessOpen.Value = false;
                    IsEquipmentOpen.Value = false;
                    IsWorkerOpen.Value = false;
                    break;

                case "DisplayInfo":
                    //戻る
                    StartPage();
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.ProcessName.Value = ProcessName.Value;
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new PlanList());
                    break;
            }
        }

        //登録処理
        public void RegistData()
        {
            //サーバー情報取得
            var server = SQL.GetServerIP(Connection.Value);
            var connection = Connection.Value.Replace(server, Server.Value);

            //INIファイル書き込み
            INI.WriteString("Database", "ConnectString", connection);
            INI.WriteString("Page", "Process", ProcessName.Value);
            INI.WriteString("Page", "Equipment", EquipmentCODE);
            INI.WriteString("Page", "Worker", Worker.Value);
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

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
