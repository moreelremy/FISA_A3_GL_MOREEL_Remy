using System;
using System.Threading;
using System.Windows;

namespace CryptoSoftWPF
{
    public partial class App : Application
    {
        private static Mutex mutex; // Stocker la mutex globalement

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            mutex = new Mutex(true, "CryptoSoft_Global_Mutex", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("CryptoSoft (GUI ou Console) est déjà en cours d'exécution.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
            GC.KeepAlive(mutex);
        }
    }
}
