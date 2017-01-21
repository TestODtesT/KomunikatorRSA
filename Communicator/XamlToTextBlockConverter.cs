using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Communicator
{
    public class XamlToTextBlockConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string xaml = value as string;
            if (xaml == null)
            {
                return Binding.DoNothing;
            }

            var textBlockFormat = @"<TextBlock xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" TextWrapping=""Wrap"" IsHitTestVisible=""True"">{0}</TextBlock>";
            string fullXaml = string.Format(textBlockFormat, xaml);
            var tb = (TextBlock)XamlReader.Parse(fullXaml);

            var richXaml = @"<RichTextBox xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""  IsReadOnly=""True"" IsDocumentEnabled=""True"" Margin=""-6 0 0 0"" BorderThickness=""0"" ><FlowDocument></FlowDocument></RichTextBox>";
            var rich = (RichTextBox)XamlReader.Parse(richXaml);
            var paragraph = new Paragraph();
            foreach(var inline in tb.Inlines.ToList())
            {
                if(inline.GetType() == typeof(Hyperlink))
                {
                    var hyper = inline as Hyperlink;
                    hyper.Click += ChatWindow.Hyperlink_Click;
                    paragraph.Inlines.Add(hyper);
                }
                else if (inline.GetType() == typeof(Run))
                {
                    var run = inline as Run;
                    paragraph.Inlines.Add(run);
                }
            }
            rich.Document.Blocks.Add(paragraph);
            return rich;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
