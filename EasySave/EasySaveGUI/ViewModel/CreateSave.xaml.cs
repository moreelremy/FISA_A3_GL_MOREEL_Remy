﻿using System.Configuration;
using EasySaveGUI.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CreateSave : Window
    {
        public CreateSave()
        {
            InitializeComponent();
            DataContext = LanguageHelper.Instance;

            LoadSavedLanguage();
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

        private void ButtonCreateSaveMenuClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close CreateSave window
        }

        private void ButtonCreateSaveLeaveClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}