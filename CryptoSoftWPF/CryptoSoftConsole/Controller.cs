using CryptoSoft;

namespace CryptoConsoleApp
{
    public class EncryptionController
    {
        private readonly ConsoleView _view;

        /// <summary>
        /// Constructor of the controller
        /// </summary>
        /// <param name="view"> The view of the application </param>
        public EncryptionController(ConsoleView view)
        {
            _view = view;
        }

        /// <summary>
        /// Run the application and display the main menu
        /// </summary>
        public void Run()
        {
            bool continueRunning = true;

            while (continueRunning)
            {
                // Display the title of the application and get the file path and encryption key
                _view.DisplayTitle();
                string filePath = _view.GetFilePath();
                string encryptKey = _view.GetEncryptionKey();

                // Encrypt the file if the user confirms the operation
                if (_view.ConfirmEncryption())
                {
                    _view.ShowMessage("Encryption in progress...");
                    Crypt.Encrypt(filePath, encryptKey);
                    _view.ShowMessage("File encrypted successfully !");
                }
                else
                {
                    _view.ShowMessage("Operation canceled.");
                }

                // Ask the user if he wants to encrypt another file or exit the application
                _view.EncryptAgain();
                string continueResponse = Console.ReadLine() ?? string.Empty;

                if (continueResponse.Trim() != "1")
                {
                    continueRunning = false;
                    _view.ShowMessage("Thank you for using CryptoSoft. See you soon !");
                }
            }
        }
    }
}
