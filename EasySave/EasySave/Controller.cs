using System;

class Program
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
                    Console.WriteLine("fait");
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