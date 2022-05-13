using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraphProject.Converter
{
    public static class TextToSizeConverter
    {
        public static Size ConvertSize(string text, double fontSize)
        {
            FormattedText formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                fontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
