namespace CryptoConsoleApp
{
    class Program
    {
        static void Main()
        {
            var view = new ConsoleView();

            var controller = new EncryptionController(view);
            controller.Run();
        }
    }
}
