using EasySaveGUI.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;


namespace EasySaveGUI
{
    // Inline Value Converter for SaveStrategy
    public class SaveStrategyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType().Name ?? string.Empty; // Returns "FullSave" or "DifferentialSave"
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Not needed
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShowSaves : Window
    {

        public ShowSaves()
        {
            InitializeComponent();
            DataContext = new ShowSavesViewModel(App.saveRepository);

            InitializePlaceholders();
        }

        private void InitializePlaceholders()
        {
            SetPlaceholder(InputShowSavesDeleteSave, "WPF_SaveDelete");
        }

        private void SetPlaceholder(TextBox textBox, string translationKey)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = LanguageHelper.Translate(translationKey);
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == LanguageHelper.Translate("WPF_SaveDelete"))
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = LanguageHelper.Translate("WPF_SaveDelete");
                textBox.Foreground = Brushes.Gray;
            }
        }

    }
}