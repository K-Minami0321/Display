using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

#pragma warning disable
namespace Update
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new ViewModelMainWindow();
            InitializeComponent();
        }
    }

    public class ViewModelMainWindow : INotifyPropertyChanged
    {
        //変数
        string version;

        //プロパティ
        public string Version                         //バージョン
        {
            get => version;
            set => SetProperty(ref version, value);
        }

        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);

        //変更通知
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            var h = PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
            return true;
        }

        private async void OnLoad()
        {
            //変数
            DateTime filetimeserver;                                    //サーバー
            DateTime filetimeclient;                                    //クライアント
            var setfile = "Display.exe";                                //対象ファイル
            var setpath = FOLDER.ApplicationPath();
            var serverpath = CONST.SERVER_UPDATE + CONST.UPDATE_DISPLAY;
            Version = "（Ver. " + CONST.DISPLAY_VERSION + "）";

            //対象ファイルの比較(Display.exe)
            filetimeserver = File.GetLastWriteTime(serverpath + setfile);
            filetimeclient = File.GetLastWriteTime(setpath + setfile);
            if (filetimeserver > filetimeclient)
            {
                //ファイル存在チェック
                if (File.Exists(setpath + setfile))
                {
                    //ファイルリネーム
                    File.Delete(setpath + "Display_old.exe");
                    File.Move(setpath + setfile, setpath + "Display_old.exe");
                }
                //ファイルのコピー
                File.Copy(serverpath + setfile, setpath + setfile, true);
            }

            //2秒待つ
            await System.Threading.Tasks.Task.Delay(2000);

            //EXEの起動
            var process = new Process();
            process.StartInfo.FileName = setpath + setfile;
            process.Start();

            //終了処理
            Application.Current.Shutdown();
        }
    }
}
