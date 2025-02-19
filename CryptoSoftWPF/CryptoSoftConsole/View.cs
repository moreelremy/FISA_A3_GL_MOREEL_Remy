namespace CryptoConsoleApp
{
    public class ConsoleView
    {
        public void DisplayTitle()
        {
            Console.Clear();
            Console.WriteLine("╔═════════════════════════════════════╗");
            Console.WriteLine("║            CRYPTO SOFT              ║ ");
            Console.WriteLine("╚═════════════════════════════════════╝ \n");
        }

        public string GetFilePath()
        {
            Console.Write("Enter the path of the file to encrypt : ");
            return Console.ReadLine() ?? string.Empty;
        }

        public string GetEncryptionKey()
        {
            Console.Write("Enter the encryption key : ");
            return Console.ReadLine() ?? string.Empty;
        }

        public bool ConfirmEncryption()
        {
            Console.WriteLine("Are you sure you want to encrypt this file ?");
            Console.WriteLine("[1] Yes");
            Console.WriteLine("[2] No");
            Console.Write("Votre choix : ");

            string response = Console.ReadLine() ?? string.Empty;

            return response.Trim() == "1";
        }

        public void EncryptAgain()
        {
            Console.WriteLine("\nWould you like to encrypt another file ?");
            Console.WriteLine("[1] Yes");
            Console.WriteLine("[2] No");
            Console.Write("Votre choix : ");
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

    }
}
