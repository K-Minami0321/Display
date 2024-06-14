using ClassBase;
using ClassLibrary;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Data;
using System.Reactive.Disposables;
using System.Reactive.Linq;

#pragma warning disable
namespace Display
{
    //共通クラス
    public class Common : Shared, INotifyPropertyChanged, IDisposable
    {
        //インスタンス
        public IniFile INI = new IniFile(CONST.SETTING_INI);
        public Sound SOUND = new Sound();

        //変数
        CompositeDisposable Disposable                          //解放処理イベント
        { get; } = new CompositeDisposable();

        //プロパティ
        public ReactivePropertySlim<DataTable> SelectTable      //一覧データ
        { get; set; } = new ReactivePropertySlim<DataTable>();
        public ReactivePropertySlim<DataRowView> SelectedItem   //選択した行
        { get; set; } = new ReactivePropertySlim<DataRowView>();
        public ReactivePropertySlim<int> SelectedIndex          //行選択
        { get; set; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<string> SoundFolder         //サウンドフォルダ
        { get; set; } = new ReactivePropertySlim<string>(FOLDER.ApplicationPath() + @"Sound\");
        public ReactivePropertySlim<object> Focus               //フォーカス
        { get; set; } = new ReactivePropertySlim<object>();
        public ReactivePropertySlim<string> NextFocus           //次のフォーカス
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<double> ScrollIndex         //スクロール位置
        { get; set; } = new ReactivePropertySlim<double>();
        public ReactivePropertySlim<string> HeaderAmount        //ヘッダー（重量・数量）
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> VisibleShirringUnit   //ヘッダー表示・非表示（枚数・コイル数）
        { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<string> ProductCODE         //社番
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ProductName         //品番
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ShapeName           //形状
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ShirringUnit        //コイル数
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> QuantityLabel       //ラベル（重量・枚数）
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> AmountLabel         //ラベル（シート数・コイル数・数量）
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> UnitLabel           //ラベル（重量・数量）
        { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<bool> VisibleCoil           //表示・非表示（コイル項目）
        { get; set; } = new ReactivePropertySlim<bool>();

        //プロパティ定義
        private void SetProperty()
        {
            //プロパティ設定
            ShapeName = management.ToReactivePropertySlimAsSynchronized(x => x.ShapeName).AddTo(Disposable);
            ShirringUnit = management.ToReactivePropertySlimAsSynchronized(x => x.ShirringUnit).AddTo(Disposable);

            //プロパティ定義
            ShapeName.Subscribe(x =>
            {
                iShape = Shape.SetShape(x);
            }).AddTo(Disposable);
        }

        //スタートページを表示
        public void StartPage()
        {
            //初期化
            ViewModelManufactureInfo.Instance.Initialize();
            ViewModelInProcessInfo.Instance.Initialize();
            ViewModelTransportInfo.Instance.Initialize();
            ViewModelDefectInfo.Instance.Initialize();
            DiaplayPage();
        }

        //ページ移動
        public void DiaplayPage()
        {
            //ページ移動
            Type type = Type.GetType("Display." + INI.GetString("Page", "Initial"));
            ViewModelWindowMain.Instance.FramePage.Value = WindowMain.Instance.FramePage;
            ViewModelWindowMain.Instance.FramePage.Value.Navigate(Activator.CreateInstance(type));
        }

        //ロット番号の取得・表示
        public string DisplayLotNumber(string value)
        {
            SetProperty();

            //データ取得
            var productname = ProductName.Value;
            management.LotNumber = management.CorrectionLotNumber(value);
            management.Select(management.LotNumber);
            //if (SQL.DataCount == 0) { return string.Empty; }

            //データ表示
            ProductName.Value = management.ProductName;
            QuantityLabel.Value = (ShapeName.Value == "コイル") ? "重 量" : "枚 数";
            iShape = Shape.SetShape(ShapeName.Value);

            if (iProcess != null)
            {
                ProcessName = iProcess.Name;
                VisibleCoil.Value = (ProcessName == "合板" && ShapeName.Value == "コイル") ? true : false;
                UnitLabel.Value = (ProcessName == "合板") ? "重 量" : "数 量";
            }
            AmountLabel.Value = (!string.IsNullOrEmpty(ShapeName.Value)) ? iShape.Unit : "枚 数";
            
            if (!string.IsNullOrEmpty(ProductName.Value) && ProductName.Value != productname) { SOUND.PlayAsync(SoundFolder.Value + CONST.SOUND_LOT); }
            return management.LotNumber;
        }

        //解放処理
        public void Dispose() => Disposable.Dispose();
    }

}
