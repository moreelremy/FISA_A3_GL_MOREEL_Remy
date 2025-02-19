using EasySaveGUI.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySaveGUI.Views
{
    /// <summary>
    /// Logique d'interaction pour BoxLanguage.xaml
    /// </summary>
    public partial class BoxLanguage : UserControl
    {
        public BoxLanguage()
        {
            InitializeComponent();
            LoadSavedLanguage();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLanguage = selectedItem.Tag.ToString();

                // Sauvegarde dans les settings
                Properties.Settings.Default.Language = selectedLanguage;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();

                // Change la langue immédiatement
                LanguageHelper.ChangeLanguage(selectedLanguage);
            }
        }
        private void LoadSavedLanguage()
        {
            string savedLanguage = Properties.Settings.Default.Language;

            if (!string.IsNullOrEmpty(savedLanguage))
            {
                LanguageHelper.ChangeLanguage(savedLanguage);
            }

            // Sélectionner la bonne langue dans le ComboBox
            var comboBox = FindName("LanguageComboBox") as ComboBox;
            if (comboBox != null)
            {
                foreach (ComboBoxItem item in comboBox.Items)
                {
                    if (item.Tag != null && item.Tag.ToString() == savedLanguage)
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
            }
        }
    }
}