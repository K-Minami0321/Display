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
        //プロパティ変数
        string _ProcessName;
        string _Defect;
        List<string> _Defects;

        //プロパティ
        public static ViewModelControlDefect Instance   //インスタンス
        { get; set; } = new ViewModelControlDefect();
        public IDefect Idefect                          //インターフェース
        { get; set; }
        public string ProcessName                       //工程区分
        {
            get { return _ProcessName; }
            set 
            { 
                SetProperty(ref _ProcessName, value);
                if (value == null) { return; }
                iProcess = ProcessCategory.SetProcess(value);
                if (ViewModelWindowMain.Instance.ProcessWork == "仕掛搬出")
                {
                    value = iProcess.Next;
                    iProcess = ProcessCategory.SetProcess(value);
                }
            }
        }
        public string Defect                            //不良内容
        {
            get { return _Defect; }
            set { SetProperty(ref _Defect, value); }
        }
        public List<string> Defects                     //不良内容リスト
        {
            get { return _Defects; }
            set { SetProperty(ref _Defects, value); }
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

            SOUND.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            Idefect.SelectionItem(value);
        }
    }
}
