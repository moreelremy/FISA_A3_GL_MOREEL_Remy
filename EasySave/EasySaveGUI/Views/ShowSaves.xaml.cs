using EasySaveGUI.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;


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
        }
    }
}