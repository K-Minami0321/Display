using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    public class ViewModelTransportList : Common, IKeyDown, ISelect, IDisposable
    {
        //変数
        CompositeDisposable Disposable                      //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelTransportList Instance       //インスタンス
        { get; set; } = new ViewModelTransportList();
        public ReactivePropertySlim<string> ProcessName     //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> InProcessCODE   //仕掛在庫CODE
        { get; set; } = new ReactivePropertySlim<string>();

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ設定
            ProcessName = inProcess.ToReactivePropertySlimAsSynchronized(x => x.ProcessName).AddTo(Disposable);

            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                if (x == null) { return; }
                iProcess = ProcessCategory.SetProcess(x);
            }).AddTo(Disposable);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //コンストラクター
        internal ViewModelTransportList()
        {
            inProcess = new InProcess();
            SetProperty();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;
            ViewModelWindowMain.Instance.ProcessName.Value = INI.GetString("Page", "Process");

            //初期設定
            DisplayCapution();
            Initialize();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            ViewModelWindowMain.Instance.ProcessWork.Value = "仕掛引取";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisibleList.Value = true;
            ViewModelWindowMain.Instance.VisibleInfo.Value = false;
            ViewModelWindowMain.Instance.VisibleDefect.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = false;
            ViewModelWindowMain.Instance.VisiblePlan.Value = false;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList.Value = "refresh";
        }

        //初期化
        private void Initialize()
        {
            //初期設定
            InProcessCODE.Value = string.Empty;
            SelectedIndex.Value = -1;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //仕掛在庫移動登録
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new TransportInfo());
                    break;

                case "DisplayList":
                    //仕掛在庫移動一覧
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new TransportList());
                    break;
            }
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectTable.Value = inProcess.SelectGetList();
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem.Value == null) { return; }
            InProcessCODE.Value = SelectedItem.Value.Row.ItemArray[0].ToString();
            ViewModelWindowMain.Instance.FramePage.Value.Navigate(new TransportInfo());
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

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
