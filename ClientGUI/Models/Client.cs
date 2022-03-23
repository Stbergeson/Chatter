using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientGUI.Models
{
    public class Client
    {

        int port;
        byte[] data;
        TcpClient client;
        NetworkStream stream;

        public void Connect(String server)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.

                port = 13000;
                data = null;
                client = new TcpClient(server, port);


                //create new thread to listen for incoming messages
                ThreadPool.QueueUserWorkItem(ListenForMessages, client);


                // Get a client stream for reading and writing.
                stream = client.GetStream();


            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }

        void CloseStream()
        {
            // Close everything.
            stream.Close();
            client.Close();
        }

        public void SendMessage(string message)
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            data = System.Text.Encoding.ASCII.GetBytes(message);


            // Send the message to the connected TcpServer.

            stream.Write(data, 0, data.Length);
        }

        private void ListenForMessages(object obj)
        {
            try
            {
                TcpClient client = (TcpClient)obj;
                NetworkStream stream = client.GetStream();

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                string responseData;

                while (true)
                {
                    // Read the first batch of the TcpServer response bytes.
                    int bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine(responseData);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
