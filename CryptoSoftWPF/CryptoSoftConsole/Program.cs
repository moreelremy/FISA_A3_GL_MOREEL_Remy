using System;
using System.Threading;

namespace CryptoConsoleApp
{
    class Program
    {
        static void Main()
        {
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "CryptoSoft_Global_Mutex", out createdNew))
            {
                if (!createdNew)
                {
                    Console.WriteLine("CryptoSoft (GUI or Console) is already running.");
                    Thread.Sleep(4000);
                    return;
                }

                var view = new ConsoleView();
                var controller = new EncryptionController(view);
                controller.Run();

                GC.KeepAlive(mutex);
            }
        }
    }
}
