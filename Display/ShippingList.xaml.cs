using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Controls;
using System.Windows.Input;

#pragma warning disable
namespace Display
{
    //画面クラス
    public partial class ShippingList : UserControl
    {
        public ShippingList()
        {
            DataContext = new ViewModelShippingList();
            InitializeComponent();
        }
    }

    //ViewModel
    public class ViewModelShippingList : Common, IWindowBase, ISelect
    {
        //変数




        //プロパティ






        //イベント
        ActionCommand commandLoad;
        public ICommand CommandLoad => commandLoad ??= new ActionCommand(OnLoad);
        ActionCommand commandButton;
        public ICommand CommandButton => commandButton ??= new ActionCommand(KeyDown);

        //コンストラクター
        public ViewModelShippingList()
        {




        }

        //ロード時
        private void OnLoad()
        {
            DisplayCapution();
            DiaplayList();
        }

        //キャプション・ボタン表示
        private void DisplayCapution()
        {
            //WindowMain
            WindowProperty = new PropertyWindow()
            {
                IwindowBase = this,
                VisiblePlan = true,
                VisibleDefect = false,
                VisibleArrow = false,
                VisibleList = true,
                VisibleInfo = true,
                VisiblePrinter = false,
                VisibleQRcode = false,
                ProcessWork = "出荷一覧"
            };
        }

        //一覧表示
        private void DiaplayList(string where = "")
        {




        }

        //選択処理
        public void SelectList()
        {




        }


        //スワイプ処理
        public void Swipe(object value) { return; }

        //キーイベント
        public void KeyDown(object value)
        {
            switch (value)
            {





            }
        }
    }
}
