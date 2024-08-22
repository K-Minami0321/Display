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
        //変数
        ContentControl framePage;
        DataTable selectTable;
        DataRowView selectedItem;
        int selectedIndex;
        double scrollIndex;
        object focus;
        string nextFocus;
        string soundFolder;
        string quantityLabel;
        string amountLabel;
        string unitLabel;
        bool visibleCoil;

        //プロパティ
        public INIFile IniFile              //iniファイル
        { get; set; } = new INIFile(CONST.SETTING_INI);
        public SoundPlay Sound              //WAVE再生処理
        { get; set; } = new SoundPlay();
        public ContentControl FramePage     //画面ページ
        {
            get { return framePage; }
            set { SetProperty(ref framePage, value); }
        }
        public DataTable SelectTable        //一覧データ
        {
            get { return selectTable; }
            set { SetProperty(ref selectTable, value); }
        }
        public DataRowView SelectedItem     //選択した行
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }
        public int SelectedIndex            //行選択
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value); }
        }
        public double ScrollIndex           //スクロール位置
        {
            get { return scrollIndex; }
            set { SetProperty(ref scrollIndex, value); }
        }
        public object Focus                 //フォーカス
        {
            get { return focus; }
            set { SetProperty(ref focus, value); }
        }
        public string NextFocus             //次のフォーカス
        {
            get { return nextFocus; }
            set { SetProperty(ref nextFocus, value); }
        }
        public string SoundFolder           //サウンドフォルダ
        {
            get { return STRING.Empty(FOLDER.ApplicationPath()) + @"Sound\"; }
        }

        //スタートページを表示
        public void StartPage(string page)
        {
            //ページ移動
            Type type = Type.GetType("Display." + page);
            ViewModelWindowMain.Instance.FramePage = (ContentControl)Activator.CreateInstance(type);
        }
    }
}
