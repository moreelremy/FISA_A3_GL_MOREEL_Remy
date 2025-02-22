using System;
using System.Threading;

namespace CryptoConsoleApp
{
    class Program
    {
        static void Main()
        {
            // Mutex to prevent multiple instances of the application
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "CryptoSoftCONSOLE_Mutex", out createdNew))
            {
                // If the mutex is already created, it means that the application is already running
                if (!createdNew)
                {
                    Console.WriteLine("CryptoSoft is already running. Please close the current instance before starting a new one.");
                    Thread.Sleep(4000);
                    return; 
                }

                // Start the application if it is not already running
                var view = new ConsoleView();
                var controller = new EncryptionController(view);
                controller.Run();

                // Prevent the mutex from being garbage collected
                GC.KeepAlive(mutex);
            }
        }
    }
}
