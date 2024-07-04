using ClassBase;
using ClassLibrary;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Controls;


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
        ContentControl _FramePage;
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
        public ContentControl FramePage     //画面ページ
        {
            get { return _FramePage; }
            set { SetProperty(ref _FramePage, value); }
        }
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

        //スタートページを表示
        public void StartPage()
        {
            //ページ移動
            Type type = Type.GetType("Display." + INI.GetString("Page", "Initial"));
            ViewModelWindowMain.Instance.FramePage = (ContentControl)Activator.CreateInstance(type);
        }
    }

}
