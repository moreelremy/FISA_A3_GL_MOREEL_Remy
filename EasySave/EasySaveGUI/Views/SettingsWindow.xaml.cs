using System.IO;
using System.Text.Json;
using System.Windows;
using EasySaveGUI.ViewModel;

namespace EasySaveGUI
{
    public partial class SettingsWindow : Window
    {
      
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = new SettingsWindowViewModel();
        }

       
    }
}
