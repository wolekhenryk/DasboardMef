using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TextWidget
{
    /// <summary>
    /// Interaction logic for TextWidgetView.xaml
    /// </summary>
    // TextWidget/TextWidgetView.xaml.cs
    public partial class TextWidgetView : UserControl
    {
        public TextWidgetView() => InitializeComponent();
        public void ShowStats(string text)
        {
            Words.Text = $"{text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length} słów";
            Chars.Text = $"{text.Length} znaków";
        }
    }
}
