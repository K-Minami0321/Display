using ClassBase;
using ClassLibrary;
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
    public partial class InProcessList : Page
    {
        public static InProcessList Instance
        { get; set; }
        public InProcessList()
        {
            Instance = this;
            DataContext = ViewModelInProcessList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelInProcessList : Common, IKeyDown, ISelect, IDisposable
    {
        //変数
        CompositeDisposable Disposable                      //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelInProcessList Instance       //インスタンス
        { get; set; } = new ViewModelInProcessList();
        public ReactivePropertySlim<string> ProcessName     //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> InProcessCODE   //仕掛在庫CODE
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> InProcessDate   //作業日
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> VisibleShape      //表示・非表示（形状）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleUnit       //表示・非表示（コイル・枚数）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleWeight     //表示・非表示（重量）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<string> HeaderUnit      //コイル・枚数
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> HeaderWeight    //焼結重量・単重
        { get; set; } = new ReactivePropertySlim<string>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ設定
            ProcessName = inProcess.ToReactivePropertySlimAsSynchronized(x => x.ProcessName).AddTo(Disposable);
            InProcessDate = inProcess.ToReactivePropertySlimAsSynchronized(x => x.InProcessDate).AddTo(Disposable);

            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                if (x == null) { return; }
                iProcess = ProcessCategory.SetProcess(x);
                switch (x)
                {
                    case "合板":
                        VisibleShape.Value = true;
                        VisibleUnit.Value = true;
                        VisibleWeight.Value = false;
                        HeaderUnit.Value = "数量";
                        HeaderAmount.Value = "重量";
                        break;

                    case "プレス":
                        VisibleShape.Value = false;
                        VisibleUnit.Value = false;
                        VisibleWeight.Value = true;
                        HeaderAmount.Value = "数量";
                        HeaderWeight.Value = "単重";
                        break;

                    default:
                        VisibleShape.Value = false;
                        VisibleUnit.Value = false;
                        VisibleWeight.Value = false;
                        HeaderAmount.Value = "数量";
                        break;
                }
            }).AddTo(Disposable);
            InProcessDate.Subscribe(x =>
            {
                DiaplayList();
            }).AddTo(Disposable);
        }

        //コンストラクター
        internal ViewModelInProcessList()
        {
            inProcess = new InProcess();
            SetProperty();

            //初期設定
            InProcessDate.Value = STRING.ToDateDB(SetToDay(DateTime.Now));
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            DataGridBehavior.Instance.Iselect = this;

            //初期設定
            DisplayCapution();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            ViewModelWindowMain.Instance.ProcessWork.Value = "搬入履歴";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisibleList.Value = true;
            ViewModelWindowMain.Instance.VisibleInfo.Value = true;
            ViewModelWindowMain.Instance.VisibleDefect.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = true;
            ViewModelWindowMain.Instance.VisiblePlan.Value = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList.Value = "refresh";
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //仕掛在庫登録画面
                    InProcessCODE.Value = null;
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new InProcessInfo());
                    break;

                case "DisplayList":
                    //仕掛在庫一覧画面
                    InProcessDate.Value = DateTime.Now.ToString("yyyyMMdd");
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new InProcessList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new PlanList());
                    break;

                case "PreviousDate":
                    //前日へ移動
                    InProcessDate.Value = DATETIME.AddDate(InProcessDate.Value, -1).ToString("yyyyMMdd");
                    break;
                
                case "NextDate":
                    //次の日へ移動
                    InProcessDate.Value = DATETIME.AddDate(InProcessDate.Value, 1).ToString("yyyyMMdd");
                    break;

                case "Today":
                    //当日へ移動
                    InProcessDate.Value = DateTime.Now.ToString("yyyyMMdd");
                    break;
            }
        }

        //一覧表示
        private void DiaplayList()
        {
            SelectedIndex.Value = -1;
            SelectTable.Value = inProcess.SelectList(null, null);           
        }

        //選択処理
        public async void SelectList()
        {
            if(SelectedItem.Value == null) { return; }
            InProcessCODE.Value = SelectedItem.Value.Row.ItemArray[0].ToString();
            ViewModelPlanList.Instance.LotNumber.Value = null;
            ViewModelWindowMain.Instance.FramePage.Value.Navigate(new InProcessInfo());
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
