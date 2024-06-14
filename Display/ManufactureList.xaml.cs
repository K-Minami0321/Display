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
using static NPOI.HSSF.Util.HSSFColor;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class ManufactureList : Page
    {
        public static ManufactureList Instance
        { get; set; }
        public ManufactureList()
        {
            Instance = this;
            DataContext = ViewModelManufactureList.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelManufactureList : Common, IKeyDown, ISelect, IDisposable
    {
        //変数
        CompositeDisposable Disposable                      //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ変数
        string _EquipmentCODE;

        //プロパティ
        public static ViewModelManufactureList Instance     //インスタンス
        { get; set; } = new ViewModelManufactureList();
        public string EquipmentCODE                         //設備CODE
        {
            get { return _EquipmentCODE; }
            set
            {
                _EquipmentCODE = value;

                equipment.EquipmentCODE = EquipmentCODE;
                equipment.Select();

                var name = equipment.EquipmentName;
                if (!string.IsNullOrEmpty(name))
                {
                    name = name + " - " + EquipmentCODE;
                }
                else
                {
                    name = iProcess.Name;
                }
                ViewModelWindowMain.Instance.ProcessWork.Value = name;
            }
        }



        public ReactivePropertySlim<string> ProcessName     //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ManufactureCODE //加工CODE
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ManufactureDate //作業日
        { get; set; } = new ReactivePropertySlim<string>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ定義
            ProcessName.Subscribe(x =>
            {
                if (x == null) { return; }
                iProcess = ProcessCategory.SetProcess(x);
            }).AddTo(Disposable);
            ManufactureDate.Subscribe(x =>
            {
                DiaplayList();

            }).AddTo(Disposable);

        }

        //コンストラクター
        internal ViewModelManufactureList()
        {
            equipment = new Equipment();
            manufacture = new Manufacture();
            ManufactureDate.Value = SetToDay(DateTime.Now);
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            SetProperty();
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
            Initialize();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //キャプション表示
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            EquipmentCODE = INI.GetString("Page", "Equipment");

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisibleList.Value = true;
            ViewModelWindowMain.Instance.VisibleInfo.Value = true;
            ViewModelWindowMain.Instance.VisibleDefect.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = true;
            ViewModelWindowMain.Instance.InitializeIcon();
            ViewModelWindowMain.Instance.IconList.Value = "refresh";
        }

        //初期化
        private void Initialize()
        {
            //初期設定
            ManufactureCODE.Value = string.Empty;
            SelectedIndex.Value = -1;
        }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {
                case "DisplayInfo":
                    //搬入登録画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new ManufactureInfo());
                    break;

                case "DisplayList":
                    //搬入一覧画面
                    ManufactureDate.Value = DateTime.Now.ToString("yyyyMMdd");
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new ManufactureList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new PlanList());
                    break;

                case "PreviousDate":
                    //前日へ移動
                    ManufactureDate.Value = DATETIME.AddDate(ManufactureDate.Value, -1).ToString("yyyyMMdd");
                    break;

                case "NextDate":
                    //次の日へ移動
                    ManufactureDate.Value = DATETIME.AddDate(ManufactureDate.Value, 1).ToString("yyyyMMdd");
                    break;

                case "Today":
                    //当日へ移動
                    ManufactureDate.Value = DateTime.Now.ToString("yyyyMMdd");
                    break;
            }
        }

        //一覧表示
        private void DiaplayList()
        {
            manufacture.ManufactureDate = ManufactureDate.Value;
            manufacture.Equipment1 = EquipmentCODE;
            SelectTable.Value = manufacture.SelectHistoryListDate(iProcess.Name);
        }

        //選択処理
        public async void SelectList()
        {
            if (SelectedItem.Value == null) { return; }
            ManufactureCODE.Value = SelectedItem.Value.Row.ItemArray[0].ToString();
            ViewModelWindowMain.Instance.FramePage.Value.Navigate(new ManufactureInfo());
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
