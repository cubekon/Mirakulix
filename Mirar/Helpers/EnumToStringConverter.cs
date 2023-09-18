using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Mirar.Helpers;
public class EnumToStringConverter : IValueConverter
{
    public EnumToStringConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // if enum is of type collection
        if (value is ICollection)
        {
            if(value is null) return new List<string>();

            List<string> results = new List<string>();

            foreach(var va in (ICollection)value)
            {
                if (!Enum.IsDefined(va.GetType(), va))
                {
                    throw new ArgumentException("Exception: EnumToStringConverter -> value is not in the enum");
                }

                var enumValue = Enum.GetName(va.GetType(), va);

                results.Add(enumValue ?? "- Error -");
            }

            return results;
        }

        // if single enum
        else
        {
            if (!Enum.IsDefined(value.GetType(), value))
            {
                throw new ArgumentException("Exception: EnumToStringConverter -> value is not in the enum");
            }

            var enumValue = Enum.GetName(value.GetType(), value);

            return enumValue ?? "- Error -";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if(value is string enumString)
        {
            return Enum.Parse(typeof(ElementTheme), enumString);
        }

        throw new ArgumentException("Exception: EnumToStringConverter -> value is not a string");
    }
}
