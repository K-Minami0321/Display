﻿using ClassBase;
using ClassLibrary;
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
        public ControlDefect()
        {
            DataContext = new ViewModelControlDefect();
            InitializeComponent();
        }
    }

    //プロパティ
    public class PropertyDefect
    {
        public static ViewModelControlDefect ViewModel      //ViewModel
        { get; set; }






    }

    //ViewModel
    public class ViewModelControlDefect : Common, IDefect
    {
        //変数
        ViewModelWindowMain CtrlWindow;
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
            get => processName;
            set 
            { 
                SetProperty(ref processName, value);
                ProcessCategory process = new ProcessCategory(value);
                if (CtrlWindow.ProcessWork == "仕掛搬出") { value = process.Next; }
            }
        }
        public string Defect                            //不良内容
        {
            get => defect;
            set => SetProperty(ref defect, value); 
        }
        public List<string> Defects                     //不良内容リスト
        {
            get => defects;
            set => SetProperty(ref defects, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand selectionChanged;
        public ICommand SelectionChanged => selectionChanged ??= new ActionCommand(SelectionItem);

        //コンストラクター
        public ViewModelControlDefect()
        {
            PropertyDefect.ViewModel = this;
        }

        //ロード時
        private void OnLoad()
        {
            Instance = this;
            ProcessName = CtrlWindow.ProcessName;

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

            var Sound = new SoundPlay();
            Sound.PlayAsync(SoundFolder + CONST.SOUND_TOUCH);
            Idefect.SelectionItem(value);
        }
    }
}
