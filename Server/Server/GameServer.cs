using JameGam.Common;
using Server.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    /// <summary>
    /// The game server for processing all connections
    /// </summary>
    internal class GameServer
    {
        public const int ProtocolVersion = 1;

        private TcpListener _listener;
        private CancellationTokenSource _cancellationToken;

        private List<Client> _clients;
        private int _nextId;

        /// <summary>
        /// Creates a new game server
        /// </summary>
        /// <param name="port">The port to listen on</param>
        public GameServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);

            _cancellationToken = new();
            _clients = new();
        }

        /// <summary>
        /// Starts the game server
        /// </summary>
        public void Start()
        {
            _listener.Start();

            // Start the accept thread and the main thread
            new Thread(new ThreadStart(AcceptClients)).Start();
            new Thread(new ThreadStart(HandleClients)).Start();

            Console.WriteLine("Server started, ready to receive connections");
        }

        /// <summary>
        /// Stops the game server
        /// </summary>
        public void Stop()
        {
            Console.WriteLine("Stopping server");

            _cancellationToken.Cancel();
            _listener.Stop();
        }

        /// <summary>
        /// Starts accepting clients, this call is blocking
        /// </summary>
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

        /// <summary>
        /// Starts handling clients, this call is blocking
        /// </summary>
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

                    HandleIncomingData(client);
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Handles incoming data from a client
        /// </summary>
        /// <param name="client">The client to receive data from</param>
        private void HandleIncomingData(Client client)
        {
            var reader = new BinaryReader(client.Stream);

            // Read the message type
            var messageType = (MessageType)reader.ReadUInt16();

            Console.WriteLine($"> [{client.Id}] {messageType}");

            if (client.State == ClientState.Connecting)
            {
                HandleConnecting(client, messageType, reader);
            }
            else
            {
                HandleConnected(client, messageType, reader);
            }
        }

        /// <summary>
        /// Handles incoming data from a client during the connecting state
        /// </summary>
        /// <param name="client">The client to receive data from</param>
        /// <param name="message">The message type</param>
        /// <param name="reader">The message reader</param>
        private void HandleConnecting(Client client, MessageType message, BinaryReader reader)
        {
            if (message == MessageType.Handshake)
            {
                // Read the handshake
                var version = reader.ReadUInt16();
                var name = reader.ReadString();

                if (version != ProtocolVersion)
                {
                    Console.WriteLine($"Dropping {name} for version mismatch. {version} != {ProtocolVersion}");
                    RemoveClient(client);

                    return;
                }

                // Set the client as connected
                client.Name = name;
                client.Id = _nextId++;
                client.State = ClientState.Connected;

                Console.WriteLine($"{name} joined");

                // Send connected to everyone
                Broadcast(new ConnectedMessage(client.Id, name), client);
            }

            // Else do nothing, we ignore other messages before handshake
        }

        /// <summary>
        /// Handles incoming data from a connected client
        /// </summary>
        /// <param name="client">The client to receive data from</param>
        /// <param name="message">The message type</param>
        /// <param name="reader">The message reader</param>
        private void HandleConnected(Client client, MessageType message, BinaryReader reader)
        {
            //
        }

        /// <summary>
        /// Broadcasts a message to all clients
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="exclude">The client to exclude sending the message to</param>
        private void Broadcast(IMessage message, Client? exclude = null)
        {
            // Write the message to a buffer
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            message.Write(writer);
            var data = stream.ToArray();

            // Send the message to all clients
            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    if (client.State == ClientState.Connecting) continue;
                    if (exclude != null && exclude == client) continue;

                    client.SendMessage(message.Type, data);
                }
            }
        }

        /// <summary>
        /// Removes a client from the server
        /// </summary>
        /// <param name="client">The client to remove</param>
        private void RemoveClient(Client client)
        {
            lock (_clients) _clients.Remove(client);

            client.Socket.Close();

            // Send disconected to everyone
            Broadcast(new DisconnectedMessage(client.Id));
        }
    }
}
