using ClassBase;
using ClassLibrary;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

//-----------------------------------------------------------------
//
//  Behavior
//
//

#pragma warning disable
namespace Display
{
    #region WindowBehavior：Behavior
    public interface IWindow
    {
        void ManipulationDelta(object? sender, ManipulationDeltaEventArgs e);
    }

    public class WindowBehavior : Behavior<Window>
    {
        //プロパティ
        public static WindowBehavior Instance  //インスタンス
        { get; set; } = new WindowBehavior();
        public IWindow iWindow;

        //コンストラクター
        public WindowBehavior()
        {
            Instance = this;
        }

        //イベント
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ManipulationDelta += ManipulationDelta;
            AssociatedObject.ManipulationStarted += AssociatedObject_ManipulationStarted;
        }

        private void AssociatedObject_ManipulationStarted(object? sender, ManipulationStartedEventArgs e)
        {

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ManipulationDelta -= ManipulationDelta;
        }

        //スワイプ処理
        private void ManipulationDelta(object? sender, ManipulationDeltaEventArgs e)
        {
            if (iWindow != null) { iWindow.ManipulationDelta(sender, e); }
        }
    }
    #endregion

    #region TextBox：Behavior
    public class TextBoxBehavior : Behavior<TextBox>
    {
        //DLL
        [DllImport("user32")] private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        //依存プロパティ
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(string), typeof(TextBoxBehavior), new PropertyMetadata(null, SetMode));
        public static readonly DependencyProperty UpperProperty = DependencyProperty.Register("Upper", typeof(bool), typeof(TextBoxBehavior), new PropertyMetadata(false, SetUpper));
        public string Mode          //入力文字制限
        {
            get { return GetValue(ModeProperty).ToString(); }
            set { SetValue(ModeProperty, value); }
        }
        public bool Upper           //入力時大文字変換
        {
            get { return (bool)GetValue((UpperProperty)); }
            set { SetValue(UpperProperty, value); }
        }

        //プロパティ
        public Regex ModeReg        //入力文字制限
        { get; set; }
        public bool ModeUpper       //入力時大文字変換
        { get; set; }

        //イベント
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += Loaded;
            AssociatedObject.MouseEnter += MouseEnter;
            AssociatedObject.GotFocus += GotFocus;
            AssociatedObject.KeyDown += KeyDown;
            AssociatedObject.PreviewMouseLeftButtonDown += MouseLeftButtonDown;
            AssociatedObject.Padding = new Thickness(5, 5, 5, 5);
            AssociatedObject.BorderBrush = CONVERT.ToSolidColorBrush("#FF90A4AE");
            AssociatedObject.PreviewKeyDown += PreviewKeyDown;
            AssociatedObject.PreviewTextInput += PreviewTextInput;
            AssociatedObject.TextChanged += TextChanged;
            AssociatedObject.ManipulationBoundaryFeedback += ManipulationBoundaryFeedback;
            DataObject.AddPastingHandler(this.AssociatedObject, PastingHandler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= Loaded;
            AssociatedObject.MouseEnter -= MouseEnter;
            AssociatedObject.GotFocus -= GotFocus;
            AssociatedObject.KeyDown -= KeyDown;
            AssociatedObject.PreviewMouseLeftButtonDown -= MouseLeftButtonDown;
            AssociatedObject.PreviewKeyDown -= PreviewKeyDown;
            AssociatedObject.PreviewTextInput -= PreviewTextInput;
            AssociatedObject.TextChanged -= TextChanged;
            AssociatedObject.ManipulationBoundaryFeedback -= ManipulationBoundaryFeedback;
            DataObject.RemovePastingHandler(this, PastingHandler);
        }

        //ロード時
        private void Loaded(object sender, RoutedEventArgs e)
        {
            if (!AssociatedObject.IsEnabled) { AssociatedObject.Opacity = 1;  }
        }

        //マウスオーバー時
        private void MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is TextBox control)) { return; }
            control.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x90, 0xA4, 0xAE));
        }

        //フォーカス時
        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox control)) { return; }
            control.Select(control.Text.Length, 0);
        }

        //値変更時
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox control)) { return; }
            if (ModeUpper) { control.Text = control.Text.ToUpper(); }       //大文字変更
            control.Select(control.Text.Length, 0);
        }

        //マウスクリック時
        private void MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox control)) { return; }
            if (control.IsFocused) { return; }
            if (!(control.IsReadOnly)) { control.Focus(); }
            e.Handled = true;
        }

        //キー処理
        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // エンターキー押下でフォーカスを移動する
                case Key.Enter:
                    var element = e.OriginalSource as UIElement;
                    var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
                    element.MoveFocus(new TraversalRequest(direction));
                    break;
            }
        }

        //バウンドエフェクト（無効）
        private void ManipulationBoundaryFeedback(object? sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        // 設定された文字以外の入力を拒否
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (ModeReg == null) { return; }
            if (!ModeReg.IsMatch(e.Text)) { e.Handled = true; }
        }

        // PreviewTextInputでは半角スペースを検知できないので、PreviewKeyDownで検知
        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ModeReg == null) { return; }
            if (e.Key == Key.Space) { e.Handled = true; }
        }

        // 貼り付け時に数字以外の入力を拒否
        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (ModeReg == null) { return; }

            //貼り付け時のチェック
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) { return; }

            string text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            if (!ModeReg.IsMatch(text))
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }

        //入力モード
        private static void SetMode(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Regex reg;

            switch (e.NewValue.ToString())
            {
                case "Number":
                    reg = new Regex("^[0-9]*$");            //数字のみ
                    break;

                case "Decimal":
                    reg = new Regex("^[0-9.]*$");           //小数点含む
                    break;

                case "Hyphen":
                    reg = new Regex("^[0-9-]*$");           //数字+ハイフン
                    break;

                case "Colon":
                    reg = new Regex("^[0-9:]*$");           //数字+コロン
                    break;

                case "Alphabet":
                    reg = new Regex("^[a-zA-Z0-9#-~]*$");   //英数字・記号
                    break;

                case "AlphabetHyphen":
                    reg = new Regex("^[a-zA-Z0-9-~]*$");    //英数字・記号+ハイフン
                    break;

                default:
                    reg = null;
                    break;
            }
            TextBoxBehavior MODE = (TextBoxBehavior)sender;
            MODE.ModeReg = reg;
        }

        //入力時小文字→大文字変換
        private static void SetUpper(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBoxBehavior UPPER = (TextBoxBehavior)sender;
            UPPER.ModeUpper = (bool)e.NewValue;
        }
    }
    #endregion

    #region ComboBox：Behavior
    public class ComboBoxBehavior : Behavior<ComboBox>
    {
        //プロパティ
        public static ComboBoxBehavior Instance      //インスタンス
        { get; set; }

        //コンストラクター
        public ComboBoxBehavior()
        {
            Instance = this;
        }

        //イベント
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += KeyDown;
            AssociatedObject.Padding = new Thickness(5, 5, 20, 5);
            AssociatedObject.BorderBrush = CONVERT.ToSolidColorBrush("#FF90A4AE");

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= KeyDown;
        }

        //キー処理
        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // エンターキー押下でフォーカスを移動する
                case Key.Enter:
                    var control = e.OriginalSource as UIElement;
                    var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
                    control.MoveFocus(new TraversalRequest(direction));
                    break;

                default:
                    break;
            }
        }
    }
    #endregion

    #region Button：Behavior
    public class ButtonBehavior : Behavior<Button>
    {
        //イベント
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += GotFocus;
            AssociatedObject.MouseEnter += GotFocus;
            AssociatedObject.LostFocus += LostFocus;
            AssociatedObject.MouseLeave += LostFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= GotFocus;
            AssociatedObject.MouseEnter -= GotFocus;
            AssociatedObject.LostFocus -= LostFocus;
            AssociatedObject.MouseLeave -= LostFocus;
        }

        //フォーカス処理
        public virtual void GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button control)) { return; }
            control.Background = CONVERT.ToSolidColorBrush(CONST.BUTTON_FORCUS);
        }
        public virtual void LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button control)) { return; }
            control.Background = CONVERT.ToSolidColorBrush(CONST.BUTTON_COLOR);
        }
    }
    #endregion

    #region Button：メニューボタン
    public class ButtonMenu : ButtonBehavior
    {
        //プロパティ
        public static readonly DependencyProperty SelectProperty =
            DependencyProperty.Register("Select", typeof(string), typeof(ButtonBehavior), new PropertyMetadata(null, null));
        public string Select        //メニューボタン選択時
        {
            get { return GetValue(SelectProperty).ToString(); }
            set { SetValue(SelectProperty, value); }
        }

        //フォーカス処理
        public override void GotFocus(object sender, RoutedEventArgs e)
        {
            if (Select != "True")
            {
                if (!(sender is Button control)) { return; }
                control.Background = CONVERT.ToSolidColorBrush(CONST.BUTTON_FORCUS);
            }
        }
        public override void LostFocus(object sender, RoutedEventArgs e)
        {
            if (Select != "True")
            {
                if (!(sender is Button control)) { return; }
                control.Background = CONVERT.ToSolidColorBrush(CONST.BUTTON_COLOR);
            }
        }
    }
    #endregion

    #region RadioButtonBehavior：Behavior
    public class RadioButtonBehavior : Behavior<RadioButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += KeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= KeyDown;

        }

        //キー処理
        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // エンターキー押下でフォーカスを移動する
                case Key.Enter:
                    var control = e.OriginalSource as UIElement;
                    var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
                    control.MoveFocus(new TraversalRequest(direction));
                    break;

                default:
                    break;
            }
        }
    }
    #endregion

    #region CheckBoxBehavior：Behavior
    //インターフェース
    public interface ICheck
    {
        void ChackCange();
    }

    public class CheckBoxBehavior : Behavior<CheckBox>
    {
        //プロパティ
        public static CheckBoxBehavior Instance     //インスタンス
        { get; set; }
        public ICheck Icheck                        //インターフェース
        { get; set; }

        //コンストラクター
        public CheckBoxBehavior() { Instance = this; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += Loaded;
            AssociatedObject.Checked += Checked;
            AssociatedObject.Unchecked += Checked;
            AssociatedObject.KeyDown += KeyDown;
            AssociatedObject.GotFocus += GotFocus;
            AssociatedObject.MouseEnter += GotFocus;
            AssociatedObject.LostFocus += LostFocus;
            AssociatedObject.MouseLeave += LostFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= Loaded;
            AssociatedObject.Checked -= Checked;
            AssociatedObject.Unchecked -= Checked;
            AssociatedObject.KeyDown -= KeyDown;
            AssociatedObject.GotFocus -= GotFocus;
            AssociatedObject.MouseEnter -= GotFocus;
            AssociatedObject.LostFocus -= LostFocus;
            AssociatedObject.MouseLeave -= LostFocus;
        }

        //ロード時
        private void Loaded(object sender, RoutedEventArgs e)
        {
            StatusCheckBox(sender);
            if (!AssociatedObject.IsEnabled) { AssociatedObject.Opacity = 1; }
        }

        //キー処理
        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // エンターキー押下でフォーカスを移動する
                case Key.Enter:
                    var control = e.OriginalSource as UIElement;
                    var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
                    control.MoveFocus(new TraversalRequest(direction));
                    break;

                default:
                    break;
            }
        }

        //チェック状態
        public void Checked(object sender, RoutedEventArgs e)
        {
            StatusCheckBox(sender);

            //インタフェース処理
            if (Icheck != null) { Icheck.ChackCange(); }
        }
        //フォーカス処理
        public virtual void GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox control)) { return; }
            if (control.IsChecked == true)
            {
                control.Foreground = CONVERT.ToSolidColorBrush(CONST.CHACK_CHAR_SELECT);
                control.Background = CONVERT.ToSolidColorBrush(CONST.CHACK_BACK_SELECT);
            }
            else
            {
                control.Foreground = CONVERT.ToSolidColorBrush(CONST.CHACK_CHAR);
                control.Background = CONVERT.ToSolidColorBrush(CONST.COLOR_FORCUS);
            }
        }

        public virtual void LostFocus(object sender, RoutedEventArgs e)
        {
            StatusCheckBox(sender);
        }

        //チェックボックスの状態
        private void StatusCheckBox(object sender)
        {
            if (!(sender is CheckBox control)) { return; }
            if (control.IsChecked == true)
            {
                control.Foreground = CONVERT.ToSolidColorBrush(CONST.CHACK_CHAR_SELECT);
                control.Background = CONVERT.ToSolidColorBrush(CONST.CHACK_BACK_SELECT);
            }
            else
            {
                control.Foreground = CONVERT.ToSolidColorBrush(CONST.CHACK_CHAR);
                control.Background = CONVERT.ToSolidColorBrush(CONST.CHACK_BACK);
            }
        }
    }
    #endregion

    #region ToggleButton：Behavior

    //インターフェース
    public interface IToggle
    {
        void ChackCange();
    }

    public class ToggleBehavior : Behavior<ToggleButton>
    {
        //プロパティ
        public static ToggleBehavior Instance       //インスタンス
        { get; set; }
        public IToggle Itoggle                      //インターフェース
        { get; set; }

        //コンストラクター
        public ToggleBehavior() { Instance = this; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += Loaded;
            AssociatedObject.Click += Click;
            AssociatedObject.GotFocus += GotFocus;
            AssociatedObject.MouseEnter += GotFocus;
            AssociatedObject.LostFocus += LostFocus;
            AssociatedObject.MouseLeave += LostFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= Loaded;
            AssociatedObject.Click -= Click;
            AssociatedObject.GotFocus -= GotFocus;
            AssociatedObject.MouseEnter -= GotFocus;
            AssociatedObject.LostFocus -= LostFocus;
            AssociatedObject.MouseLeave -= LostFocus;
        }

        //ロード時
        private void Loaded(object sender, RoutedEventArgs e)
        {
            StatusToggleButton(sender);
        }

        //チェック状態
        private void Click(object sender, RoutedEventArgs e)
        {
            StatusToggleButton(sender);

            //インタフェース処理
            if (Itoggle != null) { Itoggle.ChackCange(); }
        }

        //フォーカス処理
        public virtual void GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is ToggleButton control)) { return; }

            control.Foreground = CONVERT.ToSolidColorBrush(CONST.CHACK_CHAR);
            control.Background = CONVERT.ToSolidColorBrush(CONST.COLOR_FORCUS);

        }

        public virtual void LostFocus(object sender, RoutedEventArgs e)
        {
            StatusToggleButton(sender);
        }

        //チェックボックスの状態
        private void StatusToggleButton(object sender)
        {
            if (!(sender is ToggleButton control)) { return; }
            if (control.IsChecked == true)
            {
                control.Foreground = CONVERT.ToSolidColorBrush(CONST.CHACK_CHAR_SELECT);
                control.Background = CONVERT.ToSolidColorBrush(CONST.CHACK_BACK_SELECT);
            }
            else
            {
                control.Foreground = CONVERT.ToSolidColorBrush(CONST.CHACK_CHAR);
                control.Background = CONVERT.ToSolidColorBrush(CONST.CHACK_BACK);
            }
        }
    }
    #endregion

    #region DatePicker：Behavior
    //インターフェース
    public interface IDate
    {
        void DateCange();
    }

    public class DatePickerBehavior : Behavior<DatePicker>
    {
        //依存プロパティ
        public static readonly DependencyProperty IsMonthYearProperty =
            DependencyProperty.Register("IsMonthYear", typeof(bool), typeof(DatePickerBehavior), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty CustomDateFormatProperty =
            DependencyProperty.Register("CustomDateFormat", typeof(string), typeof(DatePickerBehavior), new FrameworkPropertyMetadata(null));

        //変数
        System.Globalization.JapaneseCalendar jpCalendar;
        System.Globalization.CultureInfo cultureInfo;
        DatePicker datepicker;
        Button calendarButton;
        DatePickerTextBox textbox;
        Popup popup;
        IDictionary<DependencyProperty, bool> isHandlerSuspended;

        //プロパティ
        public static DatePickerBehavior Instance   //インスタンス
        { get; set; }
        public IDate Idate                          //インターフェース
        { get; set; }
        public bool IsMonthYear                     //年月
        {
            get { return (bool)GetValue(IsMonthYearProperty); }
            set { SetValue(IsMonthYearProperty, value); }
        }
        public string CustomDateFormat              //日付形式
        {
            get { return (string)GetValue(CustomDateFormatProperty); }
            set { SetValue(CustomDateFormatProperty, value); }
        }
        public bool IsCustomizeFormat               //日付形式が設定されているかどうか
        { get; set; }

        //コンストラクター
        public DatePickerBehavior()
        {
            Instance = this;
        }

        //ロード時
        protected override void OnAttached()
        {
            base.OnAttached();
            jpCalendar = new System.Globalization.JapaneseCalendar();
            cultureInfo = new System.Globalization.CultureInfo("ja-JP");
            cultureInfo.DateTimeFormat.Calendar = jpCalendar;

            AssociatedObject.Loaded += OnLoad;
            AssociatedObject.SelectedDateChanged += SelectedDateChanged;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        //終了時
        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoad;
            AssociatedObject.SelectedDateChanged -= SelectedDateChanged;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }

        //ロード時
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            datepicker = sender as DatePicker;
            if (datepicker.Template == null) { return; }

            //テキストボックス定義
            textbox = datepicker.Template.FindName("PART_TextBox", datepicker) as DatePickerTextBox;
            if (textbox != null)
            {
                //IME=OFF
                InputMethod.SetPreferredImeState(textbox, InputMethodState.Off);

                //イベントの削除
                textbox.RemoveHandler(TextBox.MouseLeftButtonDownEvent, new RoutedEventHandler(GotFocus));
                textbox.RemoveHandler(TextBox.GotFocusEvent, new RoutedEventHandler(GotFocus));
                textbox.RemoveHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextKeyDown));
                textbox.RemoveHandler(TextBox.LostFocusEvent, new RoutedEventHandler(TextLostFocus));
                textbox.RemoveHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(TextChanged));

                //イベントの追加
                textbox.AddHandler(TextBox.MouseLeftButtonDownEvent, new RoutedEventHandler(GotFocus), true);
                textbox.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(GotFocus), true);
                textbox.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextKeyDown), true);
                textbox.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(TextLostFocus), true);
                textbox.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(TextChanged));
            }

            //ボタン定義
            calendarButton = datepicker.Template.FindName("PART_Button", datepicker) as Button;
            if (calendarButton != null)
            {
                datepicker.CalendarOpened += DatePickerOnCalendarOpened;
                datepicker.CalendarClosed += DatePickerOnCalendarClosed;
            }

            //カスタムフォーマット
            IsCustomizeFormat = !string.IsNullOrEmpty(CustomDateFormat);
            SetDate();
        }

        //KeyDown処理
        private void TextKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // エンターキー押下でフォーカスを移動する
                case Key.Enter:
                    //フォーカス移動
                    var control = e.OriginalSource as UIElement;
                    var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
                    control.MoveFocus(new TraversalRequest(direction));
                    break;

                default:
                    break;
            }
        }

        // Key（DOWN）でIsDropDownOpenをします
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key;

            if (key == Key.System) { key = e.SystemKey; }
            if (key == Key.Down)
            {
                if (AssociatedObject.IsDropDownOpen == false)
                {
                    AssociatedObject.IsDropDownOpen = true;
                    e.Handled = true;
                    return;
                }
            }
        }

        //IsMonth=trueのときのカレンダーが開いたときの動作
        private void DatePickerOnCalendarOpened(object sender, RoutedEventArgs e)
        {
            //年月のみ選択
            if (IsMonthYear)
            {
                var calendar = GetDatePickerCalendar(sender);
                calendar.DisplayMode = CalendarMode.Year;
                calendar.DisplayModeChanged += CalendarOnDisplayModeChanged;
            }
            SetDate();
        }

        //IsMonth=trueのときのカレンダーが閉じたときの動作
        private void DatePickerOnCalendarClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            if (IsMonthYear)
            {
                var calendar = GetDatePickerCalendar(sender);
                AssociatedObject.SelectedDate = calendar.SelectedDate;
                calendar.DisplayModeChanged -= CalendarOnDisplayModeChanged;
            }
        }

        //カレンダー取得処理
        private Calendar GetDatePickerCalendar(object sender)
        {
            DatePicker datePicker = sender as DatePicker;
            popup = (Popup)datePicker.Template.FindName("PART_Popup", datePicker);
            return ((Calendar)popup.Child);
        }

        private void CalendarOnDisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var calendar = (Calendar)sender;
            if (calendar.DisplayMode != CalendarMode.Month) { return; }

            calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);
            AssociatedObject.IsDropDownOpen = false;

            //年月カレンダーのときのちらつき防止
            if (IsMonthYear) { calendar.DisplayMode = CalendarMode.Year; }
        }

        private DateTime? GetSelectedCalendarDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue) { return null; }
            return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, 1);
        }

        /// 編集用の書式を使って、文字列に変換します。
        private string ToEditingDateFormat(DateTime date, DatePicker datePicker)
        {
            if (datePicker.SelectedDateFormat == DatePickerFormat.Short)
            {
                //ShortDateString
                return date.ToShortDateString();
            }
            else
            {
                //LongDateString
                return date.ToLongDateString();
            }
        }

        //フォーカス取得
        private void GotFocus(object sender, RoutedEventArgs e)
        {
            //SetDate();
            if (!(sender is DatePickerTextBox textBox)) { return; }
            textBox.SelectAll();
            textBox.SelectionBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x9F, 0xAE, 0xB7));
        }

        //ロストフォーカス処理
        private void TextLostFocus(object sender, RoutedEventArgs e)
        {
            SetDate();
        }

        //日付表示形式
        private void SetDate()
        {
            if (IsCustomizeFormat)
            {
                if (textbox != null && datepicker.SelectedDate != null)
                {
                    //カスタム書式で表示
                    if (CustomDateFormat.StartsWith("g", StringComparison.CurrentCulture))
                    {
                        textbox.Text = datepicker.SelectedDate.Value.ToString(CustomDateFormat, cultureInfo);
                    }
                    else
                    {
                        textbox.Text = datepicker.SelectedDate.Value.ToString(CustomDateFormat, System.Globalization.CultureInfo.CurrentCulture);
                    }
                }
            }
        }

        //日付変更用メソッド
        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            DatePicker datePicker = sender as DatePicker;

            if (datePicker.SelectedDate == null)
            {
                //PART_TextBoxの値がない場合は、カレンダーの初期値を本日にします
                datePicker.DisplayDate = DateTime.Now;
                return;
            }

            if (datePicker.Template == null)
                return;

            //テンプレート内のテキストボックスを検索します。
            var dateTextBox = datePicker.Template.FindName("PART_TextBox", datePicker) as DatePickerTextBox;

            if (dateTextBox != null)
            {
                if (string.IsNullOrWhiteSpace(dateTextBox.Text)) { return; }

                //編集中の書式
                if (datePicker.IsFocused || dateTextBox.IsFocused)
                {
                    dateTextBox.Text = this.ToEditingDateFormat(datePicker.SelectedDate.Value, datePicker);
                }
                else
                {
                    if (IsCustomizeFormat)
                    {
                        if (CustomDateFormat.StartsWith("g"))
                        {
                            dateTextBox.Text = datePicker.SelectedDate.Value.ToString(CustomDateFormat, cultureInfo);
                        }
                        else
                        {
                            dateTextBox.Text = datePicker.SelectedDate.Value.ToString(CustomDateFormat, System.Globalization.CultureInfo.CurrentCulture);
                        }
                    }
                    else
                    {
                        dateTextBox.Text = datePicker.SelectedDate.Value.ToString("yyyy/MM/dd");
                    }
                }
            }

            //日付変更インタフェース処理
            if (Idate != null) { Idate.DateCange(); }
        }

        //TextBoxの変更処理
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetValueNoCallback(DatePicker.TextProperty, textbox.Text);
        }

        private void SetValueNoCallback(DependencyProperty property, object value)
        {
            SetIsHandlerSuspended(property, true);
            try
            {
                SetCurrentValue(property, value);
            }
            finally
            {
                SetIsHandlerSuspended(property, false);
            }
        }

        private void SetIsHandlerSuspended(DependencyProperty property, bool value)
        {
            if (value)
            {
                if (isHandlerSuspended == null)
                {
                    isHandlerSuspended = new Dictionary<DependencyProperty, bool>(2);
                }

                isHandlerSuspended[property] = true;
            }
            else
            {
                if (isHandlerSuspended != null)
                {
                    isHandlerSuspended.Remove(property);
                }
            }
        }
    }

    //土曜・日曜に色をつける
    [ValueConversion(typeof(DateTime), typeof(Brush))]
    public class DateTimeToDayOfWeekBrushConverter : IValueConverter
    {
        //プロパティ
        public static Brush SundayBrush     // 日曜日用のブラシリソース
        {
            get { return Brushes.Red; }
        }
        public static Brush SaturdayBrush   // 土曜日用のブラシリソース
        {
            get { return Brushes.Blue; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is DateTime)) { return DependencyProperty.UnsetValue; }
            var dayOfWeek = ((DateTime)value).DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return SundayBrush;
                case DayOfWeek.Saturday:
                    return SaturdayBrush;
                default:
                    return DependencyProperty.UnsetValue;
            }
        }
    }
    #endregion

    #region DataGrid：Behavior
    //インターフェース
    public interface ISelect
    {
        int SelectedIndex                           //行選択
        { get; set; }
        double ScrollIndex                          //スクロール位置
        { get; set; }
        void SelectList();                          //DataGrid選択処理
    }

    public class DataGridBehavior : Behavior<DataGrid>
    {
        public static DataGridBehavior Instance     //インスタンス
        { get; set; }
        public ISelect Iselect                      //インターフェース
        { get; set; }
        public Decorator Child;
        public ScrollViewer Scroll;

        //コンストラクター
        public DataGridBehavior() { Instance = this; }

        protected override void OnAttached()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown += KeyDown;
            AssociatedObject.MouseLeftButtonUp += MouseDoubleClick;
            AssociatedObject.CurrentCellChanged += CurrentCellChanged;
            AssociatedObject.GotFocus += GotFocus;           
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= KeyDown;
            AssociatedObject.MouseLeftButtonUp -= MouseDoubleClick;
            AssociatedObject.CurrentCellChanged -= CurrentCellChanged;
            AssociatedObject.GotFocus -= GotFocus;
        }

        //選択処理
        private void CurrentCellChanged(object? sender, EventArgs e)
        {
            if (Iselect == null) { return; }

            //選択行をセット
            Iselect.SelectedIndex = AssociatedObject.SelectedIndex;

            //スクロール値をセット
            var chack = Scroll;
            SetScrollViewer();
            if (chack != null) { Iselect.ScrollIndex = Scroll.VerticalOffset; }
        }

        //キーが押された時
        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    //選択処理
                    e.Handled = true;
                    AssociatedObject.CommitEdit();
                    Iselect.SelectList();
                    break;

                case Key.Left:
                    e.Handled = true;
                    break;

                case Key.Right:
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        //マウスクリック
        private void MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Iselect == null) { return; }
            Iselect.SelectList();
        }

        //フォーカス時
        private void GotFocus(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectedIndex = 0;
        }

        //SetScrollViewerを設定
        public void SetScrollViewer()
        {
            Child = VisualTreeHelper.GetChild(AssociatedObject, 0) as Decorator;
            Scroll = Child.Child as ScrollViewer;
        }
    }
    #endregion

    #region ListBoxBehavior：Behavior
    public class ListBoxBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnDetaching();
            AssociatedObject.ManipulationBoundaryFeedback += ManipulationBoundaryFeedback;

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ManipulationBoundaryFeedback -= ManipulationBoundaryFeedback;
        }

        //バウンドエフェクト（無効）
        private void ManipulationBoundaryFeedback(object? sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
    #endregion

}
