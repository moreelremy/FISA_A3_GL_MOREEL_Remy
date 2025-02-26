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

            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = clientSocket.Receive(buffer, SocketFlags.None);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string instruction = response.Length > 4 ? response.Substring(0, 5) : "";
                switch (instruction)
                {
                    case "INPUT":
                        string[] splitResponse = response.Split();
                        string message;
                        try
                        {
                            message = InputHelper.ReadLine(Convert.ToBoolean(splitResponse[1]), Convert.ToBoolean(splitResponse[2]));
                        }
                        catch (ReturnToMenuException ex)
                        {
                            message = "ReturnToMenuException";
                        }
                        if (message == "")
                        {
                            message = "\0";
                        }
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        clientSocket.Send(data);
                        break;

                    case "CLEAR":
                        Console.Clear();
                        break;

                    default:
                        Console.WriteLine(response);
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }
}