using CryptoSoft;

namespace CryptoConsoleApp
{
    public class EncryptionController
    {
        private readonly ConsoleView _view;

        public EncryptionController(ConsoleView view)
        {
            _view = view;
        }

        public void Run()
        {
            bool continueRunning = true;

            while (continueRunning)
            {
                _view.DisplayTitle();
                string filePath = _view.GetFilePath();
                string encryptKey = _view.GetEncryptionKey();

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
