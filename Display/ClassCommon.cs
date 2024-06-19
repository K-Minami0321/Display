using ClassBase;
using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;

#pragma warning disable
namespace Display
{
    //共通クラス
    public class Common : Shared, INotifyPropertyChanged
    {
        //インスタンス
        public IniFile INI = new IniFile(CONST.SETTING_INI);
        public Sound SOUND = new Sound();

        //プロパティ変数
        DataTable _SelectTable;
        DataRowView _SelectedItem;
        int _SelectedIndex;
        double _ScrollIndex;
        object _Focus;
        string _NextFocus;
        string _SoundFolder;
        string _QuantityLabel;
        string _AmountLabel;
        string _UnitLabel;
        bool _VisibleCoil;


        //プロパティ
        public DataTable SelectTable        //一覧データ
        {
            get { return _SelectTable; }
            set { SetProperty(ref _SelectTable, value); }
        }
        public DataRowView SelectedItem     //選択した行
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }
        public int SelectedIndex            //行選択
        {
            get { return _SelectedIndex; }
            set { SetProperty(ref _SelectedIndex, value); }
        }
        public double ScrollIndex           //スクロール位置
        {
            get { return _ScrollIndex; }
            set { SetProperty(ref _ScrollIndex, value); }
        }
        public object Focus                 //フォーカス
        {
            get { return _Focus; }
            set { SetProperty(ref _Focus, value); }
        }
        public string NextFocus             //次のフォーカス
        {
            get { return _NextFocus; }
            set { SetProperty(ref _NextFocus, value); }
        }
        public string SoundFolder           //サウンドフォルダ
        {
            get { return _SoundFolder; }
            set { SetProperty(ref _SoundFolder, value); }
        }




        public string QuantityLabel         //ラベル（重量・枚数）
        {
            get { return _QuantityLabel; }
            set { SetProperty(ref _QuantityLabel, value); }
        }
        public string AmountLabel           //ラベル（シート数・コイル数・数量）
        {
            get { return _AmountLabel; }
            set { SetProperty(ref _AmountLabel, value); }
        }
        public string UnitLabel             //ラベル（重量・数量）
        {
            get { return _UnitLabel; }
            set { SetProperty(ref _UnitLabel, value); }
        }


        //スタートページを表示
        public void StartPage()
        {
            //初期化
            //ViewModelManufactureInfo.Instance.Initialize();
            //ViewModelInProcessInfo.Instance.Initialize();
            //ViewModelTransportInfo.Instance.Initialize();
            //ViewModelDefectInfo.Instance.Initialize();
            DiaplayPage();
        }

        //ページ移動
        public void DiaplayPage()
        {
            //ページ移動
            Type type = Type.GetType("Display." + INI.GetString("Page", "Initial"));
            ViewModelWindowMain.Instance.FramePage = WindowMain.Instance.FramePage;
            ViewModelWindowMain.Instance.FramePage.Navigate(Activator.CreateInstance(type));
        }

        //ロット番号の取得・表示
        public string DisplayLotNumber(string value)
        {
            //データ取得
            var productname = ProductName;
            management.LotNumber = management.CorrectionLotNumber(value);
            management.Select(management.LotNumber);
            //if (SQL.DataCount == 0) { return string.Empty; }

            //データ表示
            ProductName = management.ProductName;
            QuantityLabel = (ShapeName == "コイル") ? "重 量" : "枚 数";
            iShape = Shape.SetShape(ShapeName);

            if (iProcess != null)
            {
                ProcessName = iProcess.Name;
                //VisibleCoil = (ProcessName == "合板" && ShapeName == "コイル") ? true : false;
                UnitLabel = (ProcessName == "合板") ? "重 量" : "数 量";
            }
            AmountLabel = (!string.IsNullOrEmpty(ShapeName)) ? iShape.Unit : "枚 数";

            if (!string.IsNullOrEmpty(ProductName) && ProductName != productname) { SOUND.PlayAsync(SoundFolder + CONST.SOUND_LOT); }
            return management.LotNumber;
        }
    }

}
