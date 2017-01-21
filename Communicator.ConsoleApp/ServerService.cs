using Communicator.Common;
using Communicator.Common.Entities;
using Communicator.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// same client crash - TO DO

namespace Communicator.ConsoleApp
{
    public class ServerService
    {
        private TcpListener _listenerSocket;
        private TcpClient _clientSocket = default(TcpClient);
        public static List<ConnectedClient> Clients = new List<ConnectedClient>();
        public static List<Thread> Threads = new List<Thread>();

        private static string GetIp()
        {
            string hostname = Dns.GetHostName();
            IPHostEntry ipentry = Dns.GetHostEntry(hostname);
            IPAddress[] addr = ipentry.AddressList;
            return addr[addr.Length - 1].ToString();
        }


        public void Start()
        {
            Console.WriteLine($"{DateTime.Now} - Starting server...");

            try
            {
                _listenerSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
                _listenerSocket.Start();

                Console.WriteLine($"{DateTime.Now} - Server started");

                Console.WriteLine($"{DateTime.Now} - Listening on :8888");

                Listen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - [*SocketException*] - {ex.Message}");
            }
        }

        public void Stop()
        {
            _clientSocket.Close();
            _listenerSocket.Stop();
            Console.WriteLine($"{DateTime.Now} - Server stopped");
        }

        private void Listen()
        {
            while (true)
            {
                _clientSocket = _listenerSocket.AcceptTcpClient();
                var receivedBytes = new byte[30000];
                NetworkStream networkStream = _clientSocket.GetStream();
                networkStream.Read(receivedBytes, 0, receivedBytes.Length);
                var message = receivedBytes.ToObject() as Message;

                if(message.Type == MessageType.ClientConnected)
                {

                    Clients.Add(new ConnectedClient() { Socket = _clientSocket, UserName = message.UserName, PublicKey = message.PublicKey });
                    var messageToBroadcast = new Message() { DateTime = DateTime.Now, Text = $"{message.UserName} joined", Type = MessageType.ClientConnected, PublicKey = message.PublicKey, UserName = message.UserName, ConnectedClients = Clients.ToDictionary(z => z.UserName, z => z.PublicKey) };
                    Broadcast(messageToBroadcast);
                    DisplayHelper.DisplayGlobalMessage(messageToBroadcast.Text, messageToBroadcast.DateTime);

                    var client = new ClientService();
                    client.Start(_clientSocket, message.UserName);
                }

            }
        }

        public static void Broadcast(Message message)
        {
            if (string.IsNullOrEmpty(message.To)) 
            {
                foreach (var client in Clients)
                {
                    SendMessage(message, client);
                }
            }
            else 
            {
                var client = Clients.FirstOrDefault(x => x.UserName == message.To);
                if (client != null)
                {
                    SendMessage(message, client);
                }
            }
        }

        private static void SendMessage(Message message, ConnectedClient client)
        {
            var broadcastStream = client.Socket.GetStream();
            var broadcastBytes = message.ToByteArray();
            broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
            broadcastStream.Flush();
        }
    }
}
