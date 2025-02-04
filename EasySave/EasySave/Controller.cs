using System;

class Controler
{
    static void Main(string[] args)
    {

        bool leave = false;
        while (!leave)
        {
            string response = View.ShowMenu();

            switch (response)
            {
                case "1":
                    Console.WriteLine(Language.GetString("BackupCreated"));
                    break;

                case "2":
                    Console.WriteLine(Language.GetString("BackupStarted"));
                    break;

                case "3":
                    Console.WriteLine(Language.GetString("ViewLogs"));
                    break;

                case "4":
                    // Change the language with the model
                    Language.SetLanguage(View.GetLanguageChoice());
                    break;

                case "5":
                    leave = true;
                    break;

                default:
                    Console.WriteLine(Language.GetString("InvalidChoice"));
                    break;
            }
            Console.Clear();
        }
    }
}
