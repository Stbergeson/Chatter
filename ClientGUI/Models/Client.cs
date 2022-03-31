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
        TcpClient client;
        NetworkStream stream;
        Action<string> _writeToHistory;

        public void SetAction(Action<string> passedFunction)
        {
            _writeToHistory = passedFunction;
        }

        public Action<string> WriteToHistory
        {
            get
            {
                return _writeToHistory;
            }
            set
            {
                _writeToHistory = value;
            }
        }

        public void Connect(String server)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.

                port = 13000;
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

        public void CloseStream()
        {
            // Close everything.
            stream.Close();
            client.Close();
        }

        public void SendMessage(string message)
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] data = new Byte[256];
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
                byte[] data = new Byte[256];

                // String to store the response ASCII representation.
                string responseData;

                while (true)
                {
                    // Read the first batch of the TcpServer response bytes.
                    int bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    if(WriteToHistory != null)
                        WriteToHistory(responseData);
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
