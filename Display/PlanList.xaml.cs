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
    public partial class PlanList : Page
    {
        public static PlanList Instance
        { get; set; }
        public PlanList()
        {
            Instance = this;
            DataContext = ViewModelPlanList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelPlanList : Common, IKeyDown, ISelect, IDisposable
    {
        //変数
        CompositeDisposable Disposable                      //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public static ViewModelPlanList Instance            //インスタンス
        { get; set; } = new ViewModelPlanList();
        public ReactivePropertySlim<string> LotNumber       //ロット番号
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ProcessName     //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> InProcessCODE   //搬入CODE
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> UpdateDate      //更新表示
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> File            //ファイル名
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> VisibleUnit       //表示・非表示（コイル・シート絞り込み）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleAmount     //表示・非表示（完了数）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> EnableSelect      //選択可能
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<int> SelectedIndex      //行選択
        { get; set; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<double> ScrollIndex     //スクロール位置
        { get; set; } = new ReactivePropertySlim<double>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ設定
            ProcessName = management.ToReactivePropertySlimAsSynchronized(x => x.ProcessName).AddTo(Disposable);
            InProcessCODE = management.ToReactivePropertySlimAsSynchronized(x => x.InProcessCODE).AddTo(Disposable);

            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                if (x == null) { return; }
                iProcess = ProcessCategory.SetProcess(x);
            }).AddTo(Disposable);
        }

        //コンストラクター
        internal ViewModelPlanList()
        {
            management = new Management();
            SelectedIndex.Value = -1;
            ScrollIndex.Value = 0;
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
            Initialize();
            DiaplayList();
        }

        //初期化
        private void Initialize()
        {
            //キャプション表示
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            ViewModelWindowMain.Instance.ProcessWork.Value = ProcessName.Value + "計画一覧";
            UpdateDate.Value = management.SelectFile() + "版";

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisiblePlan.Value = true;
            ViewModelWindowMain.Instance.VisibleDefect.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = false;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconPlan.Value = "refresh";
            ViewModelWindowMain.Instance.IconSize.Value = 35;

            //画面設定
            switch (INI.GetString("Page", "Initial"))
            {
                case "PlanList":
                    //計画一覧
                    ViewModelWindowMain.Instance.VisibleList.Value = false;
                    ViewModelWindowMain.Instance.VisibleInfo.Value = false;
                    EnableSelect.Value = false;
                    break;

                default:
                    ViewModelWindowMain.Instance.VisibleList.Value = true;
                    ViewModelWindowMain.Instance.VisibleInfo.Value = true;
                    EnableSelect.Value = true;
                    break;
            }

            //表示・非表示
            VisibleUnit.Value = ProcessName.Value == "合板" ? true : false;
            VisibleAmount.Value = !VisibleUnit.Value;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //作業登録画面
                    LotNumber.Value = null;
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new InProcessInfo());
                    break;

                case "DisplayList":
                    //仕掛在庫一覧画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new InProcessList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    SelectedIndex.Value = -1;
                    ScrollIndex.Value = 0;
                    management.SelectFile();
                    DiaplayList();
                    break;

                case "Sheet":
                    DiaplayList("シート");
                    break;

                case "Coil":
                    DiaplayList("コイル");
                    break;
            }
        }

        //一覧表示
        private void DiaplayList(string where = "")
        {
            var selectIndex = SelectedIndex.Value;
            management.SelectFile();
            SelectTable.Value = management.SelectPlanList(where, true);

            //行選択・スクロール設定
            DataGridBehavior.Instance.SetScrollViewer();
            DataGridBehavior.Instance.Scroll.ScrollToVerticalOffset(ScrollIndex.Value);
            SelectedIndex.Value = selectIndex;
        }

        //選択処理
        public void SelectList()
        {
            if (SelectedItem.Value.Row.ItemArray[14].ToString() == "完了") { return; }
            LotNumber.Value = SelectedItem.Value.Row.ItemArray[1].ToString();
            if (EnableSelect.Value) { DiaplayPage(); }
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
