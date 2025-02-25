using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    public static void Main(string[] args)
    {
        try
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 5000));
            if (clientSocket != null)
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int bytesRead = clientSocket.Receive(buffer);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (response == "INPUT")
                    {
                        Console.WriteLine(response);
                    }
                    else
                    {
                        Console.WriteLine(response);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }

    private static void ListenToServer(Socket client)
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                Console.Write("Message : ");
                string message = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(message);
                client.Send(data);

                int bytesRead = client.Receive(buffer);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Serveur: " + response);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Connexion perdue : " + e.Message);
        }
    }
    private static void Disconnect(Socket socket)
    {
        try
        {
            socket.Close();
            Console.WriteLine("Déconnecté du serveur.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Impossible de fermer la connexion : " + e.Message);
        }
    }
}