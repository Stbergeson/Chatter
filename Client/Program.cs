using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "127.0.0.1";
            string message = "Message";

            Connect(server, message);
        }

        static void Connect(String server, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);


                //create new thread to listen for incoming messages
                ThreadPool.QueueUserWorkItem(ListenForMessages, client);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = null;

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                while(message != "bye")
                {

                    message = Console.ReadLine();
                    data = System.Text.Encoding.ASCII.GetBytes(message);


                    // Send the message to the connected TcpServer.

                    stream.Write(data, 0, data.Length);

                    Console.WriteLine("Sent: {0}", message);
                }


                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        private static void ListenForMessages(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            // Buffer to store the response bytes.
            byte[] data = new Byte[256];

            // String to store the response ASCII representation.
            string responseData;

            while (true)
            {
                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
            }
        }

    }
}
