using Server;
using System;
using System.Threading;
using System.Threading.Tasks;

var stoppingToken = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { stoppingToken.Cancel(); e.Cancel = true; };

// Start the game server
var server = new GameServer(9999);
server.Start();

await Task.Delay(-1, stoppingToken.Token);

server.Stop();