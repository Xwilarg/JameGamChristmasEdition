using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class GameServer
    {
        private TcpListener _listener;
        private CancellationTokenSource _cancellationToken;

        private List<Client> _clients;

        public GameServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);

            _cancellationToken = new();
            _clients = new();
        }

        public void Start()
        {
            _listener.Start();

            // Start the accept thread and the main thread
            new Thread(new ThreadStart(AcceptClients)).Start();
            new Thread(new ThreadStart(HandleClients)).Start();

            Console.WriteLine("Server started, ready to receive connections");
        }

        public void Stop()
        {
            Console.WriteLine("Stopping server");

            _cancellationToken.Cancel();
            _listener.Stop();
        }

        private void AcceptClients()
        {
            var stoppingToken = _cancellationToken.Token;

            while (!stoppingToken.IsCancellationRequested)
            {
                // Blocking accept the next client
                var connection = _listener.AcceptTcpClient();
                var client = new Client(connection.Client);

                // Add the client
                lock (_clients) _clients.Add(client);

                Console.WriteLine("New incoming connection");
            }
        }

        private void HandleClients()
        {
            var stoppingToken = _cancellationToken.Token;

            while (!stoppingToken.IsCancellationRequested)
            {
                List<Socket> toRead;
                lock (_clients) toRead = _clients.Select(x => x.Socket).ToList();

                if (toRead.Count == 0) continue;

                Socket.Select(toRead, null, null, -1);

                // Read all incoming data
                foreach (var socket in toRead)
                {
                    Client client;
                    lock (_clients) client = _clients.First(x => x.Socket == socket);

                    HandleIncomingData(socket, client);
                }

                Thread.Sleep(10);
            }
        }

        private void HandleIncomingData(Socket socket, Client client)
        {
            // Read the message length
            var buffer = new byte[2];
            socket.Receive(buffer);

            var length = BitConverter.ToUInt16(buffer);
            buffer = new byte[length];

            // Read the message
            socket.Receive(buffer);

            //
        }

        private void RemoveClient(Client client)
        {
            lock (_clients) _clients.Remove(client);

            client.Socket.Close();
        }
    }
}
