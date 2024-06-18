﻿using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class TransportList : Page
    {
        public static TransportList Instance
        { get; set; }
        public TransportList()
        {
            Instance = this;
            DataContext = ViewModelTransportList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelTransportList : Common, IKeyDown, ISelect
    {
        //プロパティ変数
        string _ProcessName;
        string _InProcessCODE;

        //プロパティ
        public static ViewModelTransportList Instance   //インスタンス
        { get; set; } = new ViewModelTransportList();
        public string ProcessName                       //工程区分
        {
            get { return inProcess.ProcessName; }
            set 
            { 
                SetProperty(ref _ProcessName, value);
                inProcess.ProcessName = value;
                iProcess = ProcessCategory.SetProcess(value);
            }
        }
        public string InProcessCODE                     //仕掛在庫CODE
        {
            get { return _InProcessCODE; }
            set { SetProperty(ref _InProcessCODE, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelTransportList()
        {
            inProcess = new InProcess();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;
            ViewModelWindowMain.Instance.ProcessName = INI.GetString("Page", "Process");

            //初期設定
            DisplayCapution();
            Initialize();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName = ViewModelWindowMain.Instance.ProcessName;
            ViewModelWindowMain.Instance.ProcessWork = "仕掛引取";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower = true;
            ViewModelWindowMain.Instance.VisibleList = true;
            ViewModelWindowMain.Instance.VisibleInfo = false;
            ViewModelWindowMain.Instance.VisibleDefect = false;
            ViewModelWindowMain.Instance.VisibleArrow = false;
            ViewModelWindowMain.Instance.VisiblePlan = false;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList = "refresh";
        }

        //初期化
        private void Initialize()
        {
            //初期設定
            InProcessCODE = string.Empty;
            SelectedIndex = -1;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //仕掛在庫移動登録
                    ViewModelWindowMain.Instance.FramePage.Navigate(new TransportInfo());
                    break;

                case "DisplayList":
                    //仕掛在庫移動一覧
                    ViewModelWindowMain.Instance.FramePage.Navigate(new TransportList());
                    break;
            }
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectTable = inProcess.SelectGetList();
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem == null) { return; }
            InProcessCODE = SelectedItem.Row.ItemArray[0].ToString();
            ViewModelWindowMain.Instance.FramePage.Navigate(new TransportInfo());
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Right":
                    KeyDown("DisplayInfo");
                    break;
            }
        }
    }
}