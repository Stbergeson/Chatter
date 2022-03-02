using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private static List<TcpClient> listOfClients;
        private class StateOfObject
        {
            public byte[] bytes;
            public string data;
            public TcpListener server;
            public int port;
            IPAddress localAddr;

            public StateOfObject(int _port, string ip = "127.0.0.1")
            {
                port = _port;
                localAddr = IPAddress.Parse(ip);
                server = new TcpListener(localAddr, port);

                bytes = new byte[256];
                data = null;
            }
        }

        private class Connection
        {
            public StateOfObject soo { get; set; }
            public TcpClient client { get; set; }

            public Connection(StateOfObject _soo, TcpClient _client)
            {
                soo = _soo;
                client = _client;
            }
        }

        public static async Task Main(string[] args)
        {
            StateOfObject soo = new(13000);
            listOfClients = new List<TcpClient>();
            try
            {

                // Start listening for client requests.
                soo.server.Start();
                TcpClient client;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    client = soo.server.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(ThreadProc, new Connection(soo, client));
                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    //TcpClient client = soo.server.AcceptTcpClient();
                    //await ListenForClientAsync(await soo.server.AcceptTcpClientAsync(), soo);

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                soo.server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        private static async void ThreadProc(object obj)
        {
            Connection connection = (Connection)obj;
            listOfClients.Add(connection.client);
            try
            {
                await ListenForClientAsync(connection.client, connection.soo);
            }
            catch
            {
                Console.WriteLine("Error with connection.");
            }
            
            //ListenForClient(connection.client, connection.soo);
        }

        private static void ListenForClient(TcpClient client, StateOfObject soo)
        {
            Console.WriteLine("Connected!");



            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();
            

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(soo.bytes, 0, soo.bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                soo.data = System.Text.Encoding.ASCII.GetString(soo.bytes, 0, i);
                Console.WriteLine("Received: {0}", soo.data);

                // Process the data sent by the client.
                soo.data = soo.data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(soo.data);

                stream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", soo.data);

                Console.WriteLine("count: " + listOfClients.Count);
                // Send back a response.
                /*foreach (var item in listOfStreams)
                {
                    item.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", soo.data);
                }*/
                
            }

            // Shutdown and end connection
            client.Close();
        }

        private static async Task ListenForClientAsync(TcpClient client, StateOfObject soo)
        {
            Console.WriteLine("Connected!");



            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.ReadAsync(soo.bytes, 0, soo.bytes.Length).Result) != 0)
            {
                // Translate data bytes to a ASCII string.
                soo.data = System.Text.Encoding.ASCII.GetString(soo.bytes, 0, i);
                Console.WriteLine("Received: {0}", soo.data);

                // Process the data sent by the client.
                soo.data = soo.data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(soo.data);

                // Send back a response.
                /*await stream.WriteAsync(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", soo.data);
                Console.WriteLine("count: " + listOfStreams.Count);*/

                foreach (var item in listOfClients)
                {
                    await item.GetStream().WriteAsync(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", soo.data);
                }
            }

            // Shutdown and end connection
            client.Close();
        }
    }
}
