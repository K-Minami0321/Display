using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //インターフェース
    public interface IDefectCategory
    {
        void SelectionItem(object value);
    }

    //画面クラス
    public partial class ControlDefectCategory : UserControl
    {
        public static ControlDefectCategory Instance
        { get; set; }
        public ControlDefectCategory()
        {
            Instance = this;
            DataContext = ViewModelControlDefectCategory.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelControlDefectCategory : Common, IDefectCategory
    {
        //変数
        ViewModelWindowMain windowMain;
        string processName;
        string defectCategory;
        List<string> defectCategorys;

        //プロパティ
        public static ViewModelControlDefectCategory Instance       //インスタンス
        { get; set; } = new ViewModelControlDefectCategory();
        public IDefectCategory IdefectCategory                      //インターフェース
        { get; set; }
        public string ProcessName                                   //工程区分
        {
            get { return processName; }
            set 
            { 
                SetProperty(ref processName, value);
                if (value == null) { return; }
                process = new ProcessCategory(value);
            }
        }
        public string DefectCategory                                //不良分類
        {
            get { return defectCategory; }
            set { SetProperty(ref defectCategory, value); }
        }
        public List<string> DefectCategorys                         //不良分類リスト
        {
            get { return defectCategorys; }
            set { SetProperty(ref defectCategorys, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);

        //ロード時
        private void OnLoad()
        {
            windowMain = ViewModelWindowMain.Instance;
            Instance = this;

            ProcessName = windowMain.ProcessName;
            DefectCategorys = process.DefectClasses;
        }

        //選択処理
        public void SelectionItem(object value)
        {
            //呼び出し元で実行
            value = DefectCategory.ToString();
            if (IdefectCategory == null) { return; }

            Sound.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            IdefectCategory.SelectionItem(value);

        }
    }
}
