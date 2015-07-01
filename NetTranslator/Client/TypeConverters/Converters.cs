using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Client.TranslatorServiceReference;
using Client.VM;

namespace Client.TypeConverters
{
    public class enumToBoolConverter : IValueConverter
        {
            #region IValueConverter Members
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                string parameterString = parameter as string;
                if (parameterString == null)
                    return DependencyProperty.UnsetValue;

                if (Enum.IsDefined(value.GetType(), value) == false)
                    return DependencyProperty.UnsetValue;

                object parameterValue = Enum.Parse(value.GetType(), parameterString);

                return parameterValue.Equals(value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                string parameterString = parameter as string;
                if (parameterString == null)
                    return DependencyProperty.UnsetValue;

                return Enum.Parse(targetType, parameterString);
            }
            #endregion
        }

    public static class Converters
    {
        public static RussianWord[] ConvertToRus(ICollection<string> items)
        {
            RussianWord[] RetItems = new RussianWord[items.Count];
            int k = 0;
            foreach (string item in items)
            {
                RetItems[k] = new RussianWord() { Content = item };
                k++;
            }
            return RetItems;
        }

        public static EnglishWord[] ConvertToEng(ICollection<string> items)
        {
            EnglishWord[] RetItems = new EnglishWord[items.Count];
            int k = 0;
            foreach (string item in items)
            {
                RetItems[k] = new EnglishWord() { Content = item };
                k++;
            }
            return RetItems;
        }

        public static ObservableCollection<string> Convert(ICollection<Word> items)
        {
            ObservableCollection<string> RetItems = new ObservableCollection<string>();
            foreach (Word item in items)
                RetItems.Add(item.Content);
            return RetItems;
        }
    }
}
