using System;

class Controller
{
    static void Main(string[] args)
    {
        SaveRepository saveRepository = new SaveRepository();

        bool leave = false;
        while (!leave)
        {
            string response = View.ShowMenu();

            try
            {
                switch (response)
                {
                    case "1":
                        Save newSave = View.CreateBackupView();
                        Save addedSave = saveRepository.AjouterSave(newSave);
                        View.SaveAddedMessageView(addedSave);
                        Console.WriteLine(Language.GetString("PressAnyKey"));
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.WriteLine(Language.GetString("BackupStarted"));
                        break;

                    case "3":
                        Console.WriteLine(Language.GetString("ViewLogs"));
                        Logger.Log("Backup1", @"C:\Source\File.txt", @"D:\Backup\File.txt", 1024, 500);
                        break;

                    case "4":
                        Language.SetLanguage(View.GetLanguageChoice());
                        break;

                    case "5":
                        List<Save> saves = saveRepository.ObtenirToutesLesSaves();
                        if (saveRepository.EstVide())
                        {
                            View.NoBackupView();
                        }
                        else
                        {
                            View.AfficherSavesView(saves);
                        }

                        Console.WriteLine(Language.GetString("PressAnyKey"));
                        Console.ReadLine();
                        break;

                    case "6":
                        leave = true;
                        break;

                    default:
                        Console.WriteLine(Language.GetString("InvalidChoice"));
                        break;
                }
            }
            catch (ReturnToMenuException ex)
            {
                // Handle the localized message from the exception
                Console.WriteLine(ex.Message);
                continue;
            }

            Console.Clear();
        }
    }
}

