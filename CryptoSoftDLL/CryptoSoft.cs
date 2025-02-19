using System.Diagnostics;
using System.Text.Json;

namespace CryptoSoft
{

    public static class Crypt
    {
        public static void Encrypt(string filePath, string encryptKey)
        {
            try
            {
                var fileManager = new FileManager(filePath, encryptKey);
                int ElapsedTime = fileManager.TransformFile();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}