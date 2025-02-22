using System;
using System.Threading;
using System.Windows;

namespace CryptoSoftWPF
{
    public partial class App : Application
    {
        // Mutex to prevent multiple instances of the application 
        protected override void OnStartup(StartupEventArgs e)
        {
            // Mutex to prevent multiple instances of the application
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "CryptoSoftWPF_Mutex", out createdNew))
            {
                // If the mutex is already created, it means that the application is already running
                if (!createdNew)
                {
                    MessageBox.Show("CryptoSoft is already running!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Shutdown();
                    return;
                }

                // Start the application if it is not already running
                base.OnStartup(e);

                // Prevent the mutex from being garbage collected
                GC.KeepAlive(mutex);
            }
        }
    }
}
