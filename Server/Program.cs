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
            public IPAddress localAddr;

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
            IPAddress[] localIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            
            StateOfObject soo = new(13000, localIp[1].ToString());

            Console.WriteLine("Local IP: " + soo.localAddr);
            Console.WriteLine("\nPort : " + soo.port);
            

            listOfClients = new List<TcpClient>();
            try
            {

                // Start listening for client requests.
                soo.server.Start();
                TcpClient client;
                Console.Write("\nListening for connections... ");

                // Enter the listening loop.
                while (true)
                {
                    //accepts next incoming connection
                    client = soo.server.AcceptTcpClient();

                    //create new thread for client connection
                    ThreadPool.QueueUserWorkItem(ThreadProc, new Connection(soo, client));

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
            
        }


        private static async Task ListenForClientAsync(TcpClient client, StateOfObject soo)
        {
            Console.WriteLine("\nNew Client Connected!");
            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;
            bool firstPass = true;
            string username = "";
            byte[] msg;


            // Loop to receive all the data sent by the client.
            while ((i = stream.ReadAsync(soo.bytes, 0, soo.bytes.Length).Result) != 0)
            {

                // Translate data bytes to a ASCII string.
                if (firstPass)
                {
                    username = soo.data = System.Text.Encoding.ASCII.GetString(soo.bytes, 0, i);
                    Console.WriteLine("{0} has arrived!", soo.data);
                    msg = System.Text.Encoding.ASCII.GetBytes($"{username} has connected...");
                    await client.GetStream().WriteAsync(msg, 0, msg.Length);
                    firstPass = false;
                }
                else
                {
                    soo.data = System.Text.Encoding.ASCII.GetString(soo.bytes, 0, i);
                    msg = System.Text.Encoding.ASCII.GetBytes($"{username}: {soo.data}");
                    Console.WriteLine($"[{string.Format("{0:HH:mm:ss tt}", DateTime.Now)}]{username}: {soo.data}");
                }


                // broadcast message to all clients. Check for and remove all disconnected clients

                List<TcpClient> listOfClientsToRemove = new();

                foreach (var item in listOfClients)
                {
                    if (item.Connected)
                    {
                        if(item != client)
                        { 
                            await item.GetStream().WriteAsync(msg, 0, msg.Length);
                        }
                    }
                    else
                    {
                        //prepare disconnected clients to be removed
                        Console.WriteLine("Client is no longer connected: " + listOfClients.Count);
                        listOfClientsToRemove.Add(item);
                    }
                }

                foreach(var item in listOfClientsToRemove)
                {
                    //remove all disconnected clients
                    listOfClients.Remove(item);
                    Console.WriteLine("Client removed. Clients Remaining: " + listOfClients.Count);
                }
            }

            // Shutdown and end connection
            client.Close();
        }
    }
}
