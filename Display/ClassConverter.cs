using ClassBase;
using ClassLibrary;
using System;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

//-----------------------------------------------------------------
//
//  表示変換クラス
//
//

#pragma warning disable
namespace Display
{
    #region IValueConverter
    //表示・非表示
    public class CollapsedConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => CONVERT.ToCollapsed((bool)value);
    }

    public class DisplayConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToStringBool().ToCollapsed();
    }

    //値があれば表示
    public class CollapsedValueConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => string.IsNullOrEmpty((string?)value) || (string?)value == "0" ? CONVERT.ToCollapsed((false)) : CONVERT.ToCollapsed((true));
    }

    //アスタリスク変換
    public class AsteriskConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => Equals(value, true) ? "*" : string.Empty;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => Equals(value, "*") ? true : false;
    }

    //完了変換
    public class CompletConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => Equals(value, true) ? "E" : string.Empty;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => Equals(value, "E") ? true : false;
    }

    //文字間に空白を入れる
    public class InsertConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => (value.ToString().Length > 2) ? value.ToString() : STRING.Insert(value.ToString(), 1);
    }

    //通貨形式
    public class CurrencyConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToCurrency;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) { return string.Empty; }
            value = value.ToString() == "0" ? string.Empty : value;
            return value.ToCurrency();
        }
    }

    //日付形式
    public class DateTimeConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToStringDate();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToStringDate();
    }

    public class MonthDayConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToStringDate("M月d日");
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToStringDate("M月d日");
    }

    //コイル数表示
    public class CoilConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToString() == "コイル" ? CONVERT.ToCollapsed(true) : CONVERT.ToCollapsed(false);
    }

    //コイル丸囲み文字変換
    public class CircleEnclosingConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToTrim().ToCircleEnclosing();
    }

    //完了表記
    public class CompletedConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !string.IsNullOrEmpty(value.ToString()) ? "完" : string.Empty;
    }

    //反転
    public class BoolConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool)value;
    }

    //工程区分の色
    public class ProcessColorConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) { return null; }
            var ret = STRING.ToTrim(value);
            switch (ret)
            {
                case "設定":
                    return "#FF90A4AE";

                case "完了":
                    return "#00FFFFFF";

                default:
                    ProcessCategory process = new ProcessCategory(ret);
                    return process.Color;
            }
        }
    }

    //完了の色
    public class CompletedColorConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => STRING.ToTrim(value) == "完了" ? "#FFFF0000" : "#FFFFFFFF";
    }

    //チェックがあれば赤
    public class CheckColorConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToString() == "E" ? "#FFFF0000" : "#00FAFAFA";
    }

    //チェックがあれば表示
    public class CheckConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value.ToString() == "E" ? CONVERT.ToCollapsed(true) : CONVERT.ToCollapsed(false);
    }

    //土曜・日曜に色をつける
    public class DateTimeToDayOfWeekBrushConverter : IValueConverter
    {
        //プロパティ
        public static Brush WeekdayBrush    //平日用のブラシリソース
        { get => Brushes.Gray; }
        public static Brush SundayBrush     // 日曜日用のブラシリソース
        { get => Brushes.Red; }
        public static Brush SaturdayBrush   // 土曜日用のブラシリソース
        { get => Brushes.Blue; }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value.ToDate() is DateTime)) { return WeekdayBrush; }
            var dayOfWeek = (value.ToDate()).DayOfWeek;

            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return SundayBrush;

                case DayOfWeek.Saturday:
                    return SaturdayBrush;

                default:
                    return WeekdayBrush;
            }
        }
    }
    #endregion
}
