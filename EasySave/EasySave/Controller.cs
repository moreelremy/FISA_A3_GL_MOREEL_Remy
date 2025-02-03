using System;

class Program
{
    static void Main(string[] args)
    {
        bool leave = false;
        while (!leave)
        {
            string response = View.AffichageMenu();

            switch (response)
            {
                case "1":
                    break;

                case "2":
                    break;

                case "3":
                    break;

                case "4":
                    leave = true;
                    break;

            }
        }
    }
}