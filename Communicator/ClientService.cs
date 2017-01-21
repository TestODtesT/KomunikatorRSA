using Communicator.Common.Entities;
using Communicator.Common.Helpers;
using Communicator.Common.RSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Communicator
{
    public class ClientService
    {
        private static TcpClient _clientSocket = new TcpClient();
        private static Dictionary<string, PublicKey> _connectedClients = new Dictionary<string, PublicKey>();
        private static RSAService _rsaService = new RSAService();
        private static string _userName = string.Empty;
        private static ChatWindow _window;
        private static NetworkStream serverStream = default(NetworkStream);

        public void Connect(string userName, ChatWindow window)
        {
            _userName = userName;
            _window = window;
            _clientSocket.Connect("127.0.0.1", 8888);
            _window.AddMessage(new Message() { UserName = ">>", Text = "Connected to Chat Server...", DateTime = DateTime.Now });
            serverStream = _clientSocket.GetStream(); 

            var message = new Message();
            message.Type = MessageType.ClientConnected;
            message.PublicKey = _rsaService.GetPublicKey();
            message.UserName = _userName;

            byte[] outStream = message.ToByteArray();
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            var thread = new Thread(GetMessage);
            thread.Start();
        }

        public void Disconnect()
        {
            var message = new Message();
            message.DateTime = DateTime.Now;
            message.Type = MessageType.ClientDisconnected;
            message.UserName = _userName;
            var msgStream = message.ToByteArray();
            serverStream.Write(msgStream, 0, msgStream.Length);
            serverStream.Flush();
        }

        public void SendMessage(string messageText)
        {
            var message = new Message();
            message.DateTime = DateTime.Now;
            message.Type = MessageType.PrivateMessage;
            message.UserName = _userName;
            foreach (var client in _connectedClients.Where(x => x.Key != _userName))
            {
                message.To = client.Key;
                message.EncryptedString = _rsaService.Encrypt(messageText, client.Value);
                var msgStream = message.ToByteArray();
                serverStream.Write(msgStream, 0, msgStream.Length);
                serverStream.Flush();
            }
            message.Text = messageText;
            _window.AddMessage(message);
        }

        private static void AddUser(string userName)
        {
            _window.AddUser(userName);
        }

        private static void GetAllClientFromMessage(Message message)
        {
            _connectedClients = message.ConnectedClients;
            foreach (var client in _connectedClients)
            {
                AddUser(client.Key);
            }
        }

        private static void GetMessage()
        {
            while (true)
            {
                byte[] receivedBytes = new byte[30000];
                NetworkStream networkStream = _clientSocket.GetStream();
                networkStream.Read(receivedBytes, 0, receivedBytes.Length);
                var message = receivedBytes.ToObject() as Message;

                if (message.Type == MessageType.PrivateMessage) 
                {
                    var decrypted = _rsaService.Decrypt(message.EncryptedString); 
                    message.Text = decrypted;
                    _window.AddMessage(message);
                }
                else
                {
                    if (message.Type == MessageType.ClientConnected) 
                    {
                        if (_connectedClients.Count == 0) 
                        {
                            GetAllClientFromMessage(message); 
                        }
                        if (!_connectedClients.Any(z => z.Key == message.UserName)) 
                        {
                            _connectedClients.Add(message.UserName, message.PublicKey);
                            AddUser(message.UserName); 
                            message.UserName = ">>"; 
                            _window.AddMessage(message);
                        }
                    }
                    else if (message.Type == MessageType.ClientDisconnected) 
                    {
                        if (_connectedClients.Count == 0)
                        {
                            GetAllClientFromMessage(message); 
                        }
                        else
                        {
                            if (_connectedClients.Any(x => x.Key == message.UserName))
                            {
                                _connectedClients.Remove(message.UserName);
                                RemoveUser(message.UserName);
                                message.UserName = ">>"; 
                                _window.AddMessage(message);
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(message.Text)) 
                    {
                        message.UserName = ">>"; 
                        _window.AddMessage(message); 
                    }
                }
            }
        }

        private static void RemoveUser(string userName)
        {
            _window.RemoveUser(userName);
        }
    }
}