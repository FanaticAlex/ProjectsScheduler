using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using ProjectsScheduler.Core.InputData;

namespace ProjectsScheduler.Desktop.View
{
    public class VacationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "[]";

            return $"[{String.Join(",", ((IEnumerable<int>)value).ToArray())}]";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
                return null;

            var trimmed = ((string)value).Substring(1, ((string)value).Length - 2); // убираем скобки
            var items = trimmed.Split(",")
                .Where(s => int.TryParse(s, out int d))
                .Select(s => int.Parse(s))
                .ToList();

            return items;
        }
    }
}
