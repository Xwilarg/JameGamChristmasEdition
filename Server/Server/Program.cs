using Server;
using System;
using System.Threading;
using System.Threading.Tasks;

var port = 9999;

// Handle --port argument
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--port" && args.Length > i + 1)
    {
        port = int.Parse(args[i + 1]);
    }
}

// Create a cancellation token for stopping the server
var stoppingToken = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { stoppingToken.Cancel(); e.Cancel = true; };

// Start the game server
var server = new GameServer(port);
server.Start();

await Task.Delay(-1, stoppingToken.Token);

server.Stop();