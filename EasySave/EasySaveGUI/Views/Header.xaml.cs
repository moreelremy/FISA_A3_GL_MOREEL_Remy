using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasySaveGUI.Views
{
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }
        private void ButtonLeaveClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonMenuClick(object sender, RoutedEventArgs e)
        {
            // Créer une nouvelle instance de MainWindow
            MainWindow mainWindow = new MainWindow();

            // Afficher MainWindow
            mainWindow.Show();

            // Fermer la fenêtre actuelle (page ou autre fenêtre)
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Vérifie si la souris est pressée pour déplacer la fenêtre
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                // Récupère la fenêtre parente et la déplace
                var window = Window.GetWindow(this);  // Cette ligne permet d'obtenir la fenêtre parent
                window.DragMove();  // Déplace la fenêtre
            }
        }
    }
}
