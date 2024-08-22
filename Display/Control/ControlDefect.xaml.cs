using ClassBase;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //インターフェース
    public interface IDefect
    {
        void SelectionItem(object value);
    }

    //画面クラス
    public partial class ControlDefect : UserControl
    {
        public static ControlDefect Instance
        { get; set; }
        public ControlDefect()
        {
            Instance = this;
            DataContext = ViewModelControlDefect.Instance;
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelControlDefect : Common, IDefect
    {
        //変数
        string processName;
        string defect;
        List<string> defects;

        //プロパティ
        public static ViewModelControlDefect Instance   //インスタンス
        { get; set; } = new ViewModelControlDefect();
        public IDefect Idefect                          //インターフェース
        { get; set; }
        public string ProcessName                       //工程区分
        {
            get { return processName; }
            set 
            { 
                SetProperty(ref processName, value);
                if (value == null) { return; }
                process = new ProcessCategory(value);
                if (ViewModelWindowMain.Instance.ProcessWork == "仕掛搬出")
                {
                    value = process.Next;
                    process = new ProcessCategory(value);
                }
            }
        }
        public string Defect                            //不良内容
        {
            get { return defect; }
            set { SetProperty(ref defect, value); }
        }
        public List<string> Defects                     //不良内容リスト
        {
            get { return defects; }
            set { SetProperty(ref defects, value); }
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);


        //ロード時
        private void OnLoad()
        {
            Instance = this;
            ProcessName = ViewModelWindowMain.Instance.ProcessName;

            //不良内容追加
            Defects = new List<string>();
            Defects.Add("ハガレ");
            Defects.Add("ワレ");
            Defects.Add("巣");
        }

        //選択処理
        public void SelectionItem(object value)
        {
            //呼び出し元で実行
            value = Defect.ToString();
            if (Idefect == null) { return; }

            Sound.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            Idefect.SelectionItem(value);
        }
    }
}
