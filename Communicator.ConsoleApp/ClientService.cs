using Communicator.Common;
using Communicator.Common.Entities;
using Communicator.Common.Helpers;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Communicator.ConsoleApp
{
    public class ClientService
    {
        private TcpClient _clientSocket;
        private string _userName;
        private Thread _thread;
        private bool _isConnected;

        public void Start(TcpClient clientSocket, string userName)
        {
            _clientSocket = clientSocket;
            _userName = userName;
            _isConnected = true;
            _thread = new Thread(Chat);
            _thread.Start();
        }

        private void Chat()
        {
            while (_isConnected)
            {
                try
                {
                    var receivedBytes = new byte[30000];
                    NetworkStream networkStream = _clientSocket.GetStream();
                    networkStream.Read(receivedBytes, 0, receivedBytes.Length);
                    var message = receivedBytes.ToObject() as Message;
                    if (message.Type == MessageType.PrivateMessage)
                    {
                        DisplayHelper.DisplayPrivateMessage(message.EncryptedString, message.UserName, message.DateTime, message.To);
                        ServerService.Broadcast(message);
                    }
                    else if (message.Type == MessageType.ClientDisconnected)
                    {
                        ClientDisconnect();
                    }
                    else
                    {
                        ServerService.Broadcast(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    ClientDisconnect();
                }
            }
            _thread.Abort();
        }

        private void ClientDisconnect()
        {
            var client = ServerService.Clients.FirstOrDefault(x => x.UserName == _userName);
            ServerService.Clients.Remove(client);
            var messageToBroadcast = new Message() { DateTime = DateTime.Now, Text = $"{_userName} left the chat", Type = MessageType.ClientDisconnected, UserName = _userName, ConnectedClients = ServerService.Clients.ToDictionary(z => z.UserName, z => z.PublicKey) };
            ServerService.Broadcast(messageToBroadcast);
            DisplayHelper.DisplayGlobalMessage(messageToBroadcast.Text, messageToBroadcast.DateTime);
            _isConnected = false;
        }
    }
}