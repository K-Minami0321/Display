using ClassBase;
using ClassLibrary;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Data;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class ManufactureInfo : Page
    {
        public static ManufactureInfo Instance
        { get; set; }
        public ManufactureInfo()
        {
            Instance = this;
            DataContext = ViewModelManufactureInfo.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelManufactureInfo : Common, IKeyDown, ITenKey, IWorker, IWorkProcess, IDisposable
    {
        //変数
        CompositeDisposable Disposable                      //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ変数
        string _EquipmentCODE;

        //プロパティ
        public static ViewModelManufactureInfo Instance     //インスタンス
        { get; set; } = new ViewModelManufactureInfo();
        public string EquipmentCODE                         //設備CODE
        {
            get { return _EquipmentCODE; }
            set 
            { 
                 _EquipmentCODE = value;

                equipment.EquipmentCODE = value;
                equipment.Select();

                var name = equipment.EquipmentName;
                if (!string.IsNullOrEmpty(equipment.EquipmentName))
                {
                    name = name + " - " + EquipmentCODE;
                    Team.Value = equipment.Team;
                }
                else
                {
                    if (iProcess == null) { return; }
                    name = iProcess.Name;
                }
                ViewModelWindowMain.Instance.ProcessWork.Value = name;
                Equipment1.Value = value;
            }
        }

        public ReactivePropertySlim<DataTable> DefectList   //不良データ
        { get; set; }= new ReactivePropertySlim<DataTable>();
        public static ReactivePropertySlim<bool> EditFlg    //編集中フラグ
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> RegFlg            //新規・既存フラグ(true:新規、false:既存)
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<string> Status              //入力状況
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ProcessName         //工程区分
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ManufactureCODE     //加工CODE
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ManufactureDate     //作業日
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Equipment1          //設備
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Equipment2          //設備
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Team                //班名
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> LotNumber       //ロット番号
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> WorkProcess         //工程
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Worker              //担当者
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> StartTime           //作業開始時刻
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> EndTime             //作業終了時刻
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> WorkTime            //作業時間
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Amount              //数量
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Weight              //重量
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Comment             //コメント
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> IsCompleted           //完了チェック
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsSales               //売上チェック
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> IsOutsourced          //外注チェック
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> EnabledControl1       //コントロール使用可能
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> EnabledControl2       //コントロール使用可能
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<int> LotNumberLength        //文字数（ロット番号）
        { get; set; } = new ReactivePropertySlim<int>(10);
        public ReactivePropertySlim<int> AmountLength           //文字数（数量）
        { get; set; } = new ReactivePropertySlim<int>(5);
        public ReactivePropertySlim<string> BreakName           //中断ボタン名
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> VisibleTime           //表示・非表示（時間の区切り）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisiblePackaging      //表示・非表示（数量)
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleButton         //表示・非表示（下部ボタン）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleButtonStart    //開始ボタン表示・非表示
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleButtonEnd      //終了ボタン表示・非表示
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleButtonCancel   //取消ボタン表示・非表示
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleButtonBreak    //中断ボタン表示・非表示
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleEdit           //表示・非表示（削除ボタン）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleTenKey         //表示・非表示（テンキー）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleWorker         //表示・非表示（作業者）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> VisibleWorkProcess    //表示・非表示（工程）
        { get; set; } = new ReactivePropertySlim<bool>();

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);
        ActionCommand gotFocus;
        public ICommand GotFocus => gotFocus ??= new ActionCommand(SetGotFocus);
        ActionCommand lostFocus;
        public ICommand LostFocus => lostFocus ??= new ActionCommand(SetLostFocus);

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ設定
            EditFlg = new ReactivePropertySlim<bool>();
            ProcessName = manufacture.ToReactivePropertySlimAsSynchronized(x => x.ProcessName).AddTo(Disposable);
            ManufactureCODE = manufacture.ToReactivePropertySlimAsSynchronized(x => x.ManufactureCODE).AddTo(Disposable);
            ManufactureDate = manufacture.ToReactivePropertySlimAsSynchronized(x => x.ManufactureDate).AddTo(Disposable);
            ProductName = manufacture.ToReactivePropertySlimAsSynchronized(x => x.ProductName).AddTo(Disposable);
            LotNumber = manufacture.ToReactivePropertySlimAsSynchronized(x => x.LotNumber).AddTo(Disposable);
            Equipment1 = manufacture.ToReactivePropertySlimAsSynchronized(x => x.Equipment1).AddTo(Disposable);
            Equipment2 = manufacture.ToReactivePropertySlimAsSynchronized(x => x.Equipment2).AddTo(Disposable);
            WorkProcess = manufacture.ToReactivePropertySlimAsSynchronized(x => x.WorkProcess).AddTo(Disposable);
            Worker = manufacture.ToReactivePropertySlimAsSynchronized(x => x.Worker).AddTo(Disposable);
            StartTime = manufacture.ToReactivePropertySlimAsSynchronized(x => x.StartTime).AddTo(Disposable);
            EndTime = manufacture.ToReactivePropertySlimAsSynchronized(x => x.EndTime).AddTo(Disposable);
            WorkTime = manufacture.ToReactivePropertySlimAsSynchronized(x => x.WorkTime).AddTo(Disposable);
            Weight = manufacture.ToReactivePropertySlimAsSynchronized(x => x.WorkTime).AddTo(Disposable);
            Amount = manufacture.ToReactivePropertySlimAsSynchronized(x => x.Amount).AddTo(Disposable);
            Comment = manufacture.ToReactivePropertySlimAsSynchronized(x => x.Comment).AddTo(Disposable);
            IsCompleted = manufacture.ToReactivePropertySlimAsSynchronized(x => x.IsCompleted).AddTo(Disposable);
            IsSales = manufacture.ToReactivePropertySlimAsSynchronized(x => x.IsSales).AddTo(Disposable);
            IsOutsourced = manufacture.ToReactivePropertySlimAsSynchronized(x => x.IsOutsourced).AddTo(Disposable);

            //プロパティ定義
            RegFlg.Subscribe(x =>
            {
                if (!x) { DisplayData(); }
                VisibleEdit.Value = !x;
            }).AddTo(Disposable);
            Status.Subscribe(x =>
            {
                SetStatus();
                EditFlg.Value = Status.Value == "作業中" ? true : false;
            }).AddTo(Disposable);
            ProcessName.Subscribe(x =>
            {
                iProcess = ProcessCategory.SetProcess(x);
                switch (x)
                {
                    case "合板":
                    case "プレス":
                    case "仕上":
                        VisiblePackaging.Value = true;
                        break;

                    default:
                        VisiblePackaging.Value = false;
                        break;
                }
            }).AddTo(Disposable);
            ManufactureDate.Subscribe(x =>
            {
                VisibleButton.Value = x != STRING.ToDate(SetToDay(DateTime.Now)) ? false : true;
            }).AddTo(Disposable);
            StartTime.Subscribe(x =>
            {
                VisibleTime.Value = !string.IsNullOrEmpty(x);
            }).AddTo(Disposable);
            EndTime.Subscribe(x =>
            {
                WorkTime.Value = manufacture.CalculateWorkingTime(ManufactureDate.Value, StartTime.Value, x);
            }).AddTo(Disposable);
        }

        //コンストラクター
        internal ViewModelManufactureInfo()
        {
            manufacture = new Manufacture();
            management = new Management();
            equipment = new Equipment();
            //SetProperty();
        }

        //ロード時
        private void OnLoad()
        {
            //インスタンス
            Instance = this;
            ViewModelWindowMain.Instance.Ikeydown = this;
            ViewModelControlTenKey.Instance.Itenkey = this;
            ViewModelControlWorker.Instance.Iworker = this;
            ViewModelControlWorkProcess.Instance.IworkProcess = this;
            ViewModelWindowMain.Instance.ProcessName.Value = INI.GetString("Page", "Process");
            DisplayCapution();
            SetGotFocus("LotNumber");
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {          
            //キャプション
            ProcessName.Value = ViewModelWindowMain.Instance.ProcessName.Value;
            EquipmentCODE = INI.GetString("Page", "Equipment");
            

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisibleList.Value = true;
            ViewModelWindowMain.Instance.VisibleInfo.Value = true;
            ViewModelWindowMain.Instance.VisibleDefect.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = false;
            ViewModelWindowMain.Instance.VisiblePlan.Value = true;
            ViewModelWindowMain.Instance.InitializeIcon();

            //一覧からデータ読み込み（修正）
            ManufactureCODE.Value = ViewModelManufactureList.Instance.ManufactureCODE.Value;
            DefectList.Value = ViewModelDefectInfo.Instance.DefectList.Value;
            RegFlg.Value = string.IsNullOrEmpty(ManufactureCODE.Value);

            if (RegFlg.Value)
            {
                //予定表からロット番号取得
                LotNumber.Value = ViewModelPlanList.Instance.LotNumber.Value;     //データ取得
                if (LotNumber.Value == null) { LotNumber.Value = string.Empty; }
                DisplayLotNumber(LotNumber.Value);
                SetGotFocus("Worker");
            }
            if (string.IsNullOrEmpty(ProductName.Value)) { SetGotFocus("LotNumber"); }

        }

        //初期化
        public void Initialize()
        {
            ManufactureDate.Value = SetToDay(DateTime.Now);
            ManufactureCODE.Value = string.Empty;
            ProductName.Value = string.Empty;
            LotNumber.Value = string.Empty;
            Equipment1.Value = string.Empty;
            Equipment2.Value = string.Empty;
            WorkProcess.Value = string.Empty;
            Worker.Value = INI.GetString("Page", "Worker");
            StartTime.Value = string.Empty;
            EndTime.Value = string.Empty;
            WorkTime.Value = string.Empty;
            Amount.Value = string.Empty;
            Weight.Value = string.Empty;
            IsCompleted.Value = false;
            IsSales.Value = false;
            IsOutsourced.Value = false;
            AmountLabel.Value = "数 量";
        }

        //データ表示
        private void DisplayData()
        {
            manufacture.Select();
            DisplayLotNumber(LotNumber.Value);        //ロット情報の取得
        }

        //キーイベント
        public async void KeyDown(object value)
        {
            var result = false;

            switch (value)
            {
                case "WorkStart":
                    //作業開始
                    if (await IsRequiredRegist())
                    {
                        result = (bool)await DialogHost.Show(new ControlMessage("作業を開始します。", "※「はい」ボタンを押して作業を開始します。", "警告"));
                        await System.Threading.Tasks.Task.Delay(100);
                        if (result)
                        {
                            //ボタン処理
                            Status.Value = "作業中";
                            SetGotFocus("Amount");
                            StartTime.Value = DateTime.Now.ToString("HH:mm");
                        }
                    }
                    break;

                case "WorkEnd":
                    //作業終了処理
                    EndTime.Value = DateTime.Now.ToString("HH:mm");
                    result = (bool)await DialogHost.Show(new ControlMessage("作業を完了します。", "※登録後、次の作業の準備をしてください。", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result)
                    {
                        RegistData();
                        Status.Value = "準備";
                    }
                    else
                    {
                        EndTime.Value = string.Empty;
                        WorkTime.Value = string.Empty;
                    }
                    break;

                case "WorkBreak":
                    //中断処理・再開処理
                    Status.Value = (Status.Value == "中断") ? "作業中" : "中断";
                    break;

                case "Cancel":
                    //取消
                    result = (bool)await DialogHost.Show(new ControlMessage("この作業を取消します。", "※入力されたものが消去されます", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result) { 
                        Initialize(); 
                        Status.Value = "準備"; 
                    }
                    break;

                case "Regist":
                    result = (bool)await DialogHost.Show(new ControlMessage("作業データを修正します。", "※「はい」ボタンを押して作業データを修正します。", "警告"));
                    if (result) 
                    { 
                        RegistData();
                        ViewModelWindowMain.Instance.FramePage.Value.Navigate(new ManufactureList());
                    }
                    break;

                case "Delete":
                    result = (bool)await DialogHost.Show(new ControlMessage("作業データを削除します", "※削除されたデータは復元できません", "警告"));
                    await System.Threading.Tasks.Task.Delay(100);
                    SetGotFocus(Focus);
                    if (result) 
                    { 
                        DeleteDate();
                        ViewModelWindowMain.Instance.FramePage.Value.Navigate(new ManufactureList());
                    }
                    break;

                case "Enter":
                    //フォーカス移動
                    SetNextFocus();
                    break;

                case "BS":
                    //バックスペース処理
                    BackSpaceText();
                    break;

                case "CLEAR":
                    //文字列消去
                    ClearText();
                    break;

                case "1": case "2": case "3": case "4": case "5": case "6": case "7": case "8": case "9": case "0": case "-":
                    //テンキー処理
                    DisplayText(value);
                    break;

                case "Completed":
                    //完了
                    IsCompleted.Value = !IsCompleted.Value;
                    break;

                case "Sales":
                    //売上
                    IsSales.Value = !IsSales.Value;
                    break;

                case "DefectList":
                    //不良一覧
                    VisibleTenKey.Value = false;
                    VisibleWorker.Value = false;
                    VisibleWorkProcess.Value = false;
                    break;

                case "DisplayInfo":
                    //加工登録画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new ManufactureInfo());
                    break;

                case "DisplayList":
                    //加工一覧画面

                    //編集モードの場合はデータクリア
                    if (!string.IsNullOrEmpty(ManufactureCODE.Value)) { Initialize(); }

                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new ManufactureList());
                    break;

                case "DisplayPlan":
                    //計画一覧画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new PlanList());
                    break;

                case "DefectInfo":
                    //不良登録画面
                    ViewModelWindowMain.Instance.FramePage.Value.Navigate(new DefectInfo());
                    break;
            }
        }

        //選択処理
        public void SelectionItem(object value)
        {
            switch (Focus.Value)
            {
                case "Worker":
                    Worker.Value = value.ToString();
                    break;

                case "WorkProcess":
                    WorkProcess.Value = value.ToString();
                    break;
            }
            //SetNextFocus();
        }

        //状態により設定する
        private void SetStatus()
        {
            switch (Status.Value)
            {
                case "準備":
                    EnabledControl1.Value = true;
                    EnabledControl2.Value = false;
                    VisibleButtonStart.Value = true;
                    VisibleButtonEnd.Value = false;
                    VisibleButtonBreak.Value = false;
                    VisibleButtonCancel.Value = false;

                    ViewModelWindowMain.Instance.VisibleList.Value = true;
                    ViewModelWindowMain.Instance.VisibleDefect.Value = false;
                    ViewModelWindowMain.Instance.VisibleArrow.Value = false;
                    SetGotFocus("LotNumber");
                    break;

                case "作業中":
                    EnabledControl1.Value = false;
                    EnabledControl2.Value = true;
                    VisibleButtonStart.Value = false;
                    VisibleButtonEnd.Value = true;
                    VisibleButtonBreak.Value = true;
                    VisibleButtonCancel.Value = false;
                    BreakName.Value = "中　断";

                    ViewModelWindowMain.Instance.VisibleList.Value = true;
                    ViewModelWindowMain.Instance.VisibleDefect.Value = false;
                    SetGotFocus("Amount");

                    //一覧からデータ読み込み
                    ManufactureCODE.Value = ViewModelManufactureList.Instance.ManufactureCODE.Value;
                    RegFlg.Value = string.IsNullOrEmpty(ManufactureCODE.Value);
                    break;

                case "中断":
                    EnabledControl1.Value = false;
                    EnabledControl2.Value = false;
                    VisibleButtonStart.Value = false;
                    VisibleButtonEnd.Value = false;
                    VisibleButtonBreak.Value = true;
                    VisibleButtonCancel.Value = true;
                    BreakName.Value = "再　開";

                    ViewModelWindowMain.Instance.VisibleList.Value = true;
                    ViewModelWindowMain.Instance.VisibleDefect.Value = false;

                    break;

                case "編集":
                    EnabledControl1.Value = true;
                    EnabledControl2.Value = true;
                    VisibleButtonStart.Value = false;
                    VisibleButtonEnd.Value = false;
                    VisibleButtonBreak.Value = false;
                    VisibleButtonCancel.Value = false;

                    ViewModelWindowMain.Instance.VisibleList.Value = true;
                    ViewModelWindowMain.Instance.VisibleDefect.Value = false;

                    break;

                default:
                    Status.Value = "準備";
                    EnabledControl1.Value = true;
                    EnabledControl2.Value = false;
                    VisibleButtonStart.Value = true;
                    VisibleButtonEnd.Value = false;
                    VisibleButtonBreak.Value = false;
                    VisibleButtonCancel.Value = false;

                    ViewModelWindowMain.Instance.VisibleList.Value = true;
                    ViewModelWindowMain.Instance.VisibleDefect.Value = false;
                    ViewModelWindowMain.Instance.VisibleArrow.Value = false;
                    break;
            }

            //ボタン設定
            ViewModelWindowMain.Instance.VisiblePower.Value = true;
            ViewModelWindowMain.Instance.VisibleInfo.Value = false;
            ViewModelWindowMain.Instance.VisibleArrow.Value = false;
        }

        //登録処理
        private void RegistData()
        {
            //コード確定
            var mark = iProcess.Mark;
            if (RegFlg.Value)
            {
                var date = STRING.ToDateDB(ManufactureDate.Value);
                var code = manufacture.GenerateCode(mark + date);
                ManufactureCODE.Value = code;
            }

            //登録処理
            manufacture.InsertLog(RegFlg.Value);
            manufacture.Resist();

            //不良データ登録
            if (DefectList != null)
            {






            }

            //初期設定
            ViewModelManufactureList.Instance.ManufactureCODE.Value = string.Empty;
            Initialize();
        }

        //必須チェック
        private async Task<bool> IsRequiredRegist()
        {
            var result = true;
            var focus = string.Empty;
            var messege1 = string.Empty;
            var messege2 = string.Empty;
            var messege3 = string.Empty;

            if (string.IsNullOrEmpty(Worker.Value))
            {
                focus = "Worker";
                messege1 = "担当者を選択してください";
                messege2 = "※担当者は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            switch(ProcessName.Value)
            {
                case "合板":
                case "プレス":
                case "仕上":
                    if (string.IsNullOrEmpty(WorkProcess.Value))
                    {
                        focus = "WorkProcess";
                        messege1 = "工程を選択してください";
                        messege2 = "※工程は必須項目です。";
                        messege3 = "確認";
                        result = false;
                    }
                    break;
            }

            if (string.IsNullOrEmpty(LotNumber.Value))
            {
                focus = "LotNumber";
                messege1 = "ロット番号を入力してください";
                messege2 = "※ロット番号は必須項目です。";
                messege3 = "確認";
                result = false;
            }

            if (!result) 
            {
                var messege = (bool)await DialogHost.Show(new ControlMessage(messege1, messege2, messege3));
                await System.Threading.Tasks.Task.Delay(100);
                if (messege) { SetGotFocus(focus); }
            }
            return result;
        }

        //削除処理
        private void DeleteDate()
        {
            //製造実績削除
            manufacture.DeleteLog();
            manufacture.Delete();

            //製造不良削除
            //defect.ManufactureCODE = ManufactureCODE;
            //defect.Delete();

            //処理完了
            ViewModelManufactureList.Instance.ManufactureCODE.Value = string.Empty;
            Initialize();
        }

        //入力制御
        private void DisplayText(object value)
        {
            switch (Focus.Value)
            {
                case "LotNumber":
                    //
                    if (LotNumber.Value.Length < LotNumberLength.Value)
                    {
                        LotNumber.Value += value.ToString();
                        ManufactureInfo.Instance.LotNumber.Select(LotNumber.Value.Length, 0);
                    }
                    break;

                case "Amount":
                    if (Amount.Value.Length < AmountLength.Value)
                    {
                        Amount.Value += value.ToString();
                        ManufactureInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    }
                    break;

                default:
                    break;
            }
        }

        //文字列消去
        private void ClearText()
        {
            switch (Focus.Value)
            {
                case "LotNumber":
                    LotNumber.Value = string.Empty;
                    ManufactureInfo.Instance.LotNumber.Select(LotNumber.Value.Length, 0);
                    break;

                case "Amount":
                    Amount.Value = string.Empty;
                    ManufactureInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    break;

                default:
                    break;
            }
        }

        //バックスペース処理
        private void BackSpaceText()
        {
            switch (Focus.Value)
            {
                case "LotNumber":
                    if (LotNumber.Value.Length > 0)
                    {
                        LotNumber.Value = LotNumber.Value[..^1];
                        ManufactureInfo.Instance.LotNumber.Select(LotNumber.Value.Length, 0);
                    }
                    break;

                case "Amount":
                    if (Amount.Value.Length > 0)
                    {
                        Amount.Value = Amount.Value[..^1];
                        ManufactureInfo.Instance.Amount.Select(Amount.Value.Length, 0);
                    }
                    break;

                default:
                    break;
            }
        }

        //確定処理
        private void SetNextFocus()
        {
            if (EnabledControl1.Value)
            {
                //作業開始前
                switch (Focus.Value)
                {
                    case "LotNumber":
                        SetGotFocus("WorkProcess");
                        break;

                    case "WorkProcess":
                        SetGotFocus("Worker");
                        break;

                    case "Worker":
                        SetGotFocus("LotNumber");
                        break;

                    default:
                        break;
                }
            } 
            else 
            {
                //作業中
                switch (Focus.Value)
                {
                    case "Amount":
                        SetGotFocus("Completed");
                        break;

                    case "Completed":
                        SetGotFocus("Sales");
                        break;

                    case "Sales":
                        SetGotFocus("Amount");
                        break;

                    default:
                        break;
                }
            }
        }

        //フォーカス処理（GotForcus）
        private void SetGotFocus(object value)
        {
            Focus.Value = value;
            switch (Focus.Value)
            {
                case "LotNumber":
                    ManufactureInfo.Instance.LotNumber.Focus();
                    VisibleTenKey.Value = true;
                    VisibleWorker.Value = false;
                    VisibleWorkProcess.Value = false;
                    ViewModelControlTenKey.Instance.InputString.Value = "-";
                    if (LotNumber != null) { ManufactureInfo.Instance.LotNumber.Select(LotNumber.Value.Length, 0); }
                    break;

                case "Amount":
                    ManufactureInfo.Instance.Amount.Focus();
                    VisibleTenKey.Value = true;
                    VisibleWorker.Value = false;
                    VisibleWorkProcess.Value = false;
                    ViewModelControlTenKey.Instance.InputString.Value = ".";
                    if (Amount != null) { ManufactureInfo.Instance.Amount.Select(Amount.Value.Length, 0); }
                    break;

                case "Worker":
                    ManufactureInfo.Instance.Worker.Focus();
                    VisibleTenKey.Value = false;
                    VisibleWorker.Value = true;
                    VisibleWorkProcess.Value = false;
                    if (Worker != null) { ManufactureInfo.Instance.Worker.Select(Worker.Value.Length, 0); }
                    break;

                case "WorkProcess":
                    ManufactureInfo.Instance.WorkProcess.Focus();
                    VisibleTenKey.Value = false;
                    VisibleWorker.Value = false;
                    VisibleWorkProcess.Value = true;
                    if (WorkProcess != null) { ManufactureInfo.Instance.WorkProcess.Select(WorkProcess.Value.Length, 0); }
                    break;

                default:
                    break;
            }
        }

        //フォーカス処理（LostFoucus）
        private void SetLostFocus()
        {
            LotNumber.Value = DisplayLotNumber(LotNumber.Value);    //ロット情報の取得
        }

        //スワイプ処理
        public void Swipe(object value)
        {
            switch (value)
            {
                case "Left":
                    KeyDown("DisplayList");
                    break;
            }
        }

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }
}
