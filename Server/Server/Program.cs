using System.Net;
using System.Net.Sockets;

List<Socket> _clients = new();

void MainLoop()
{
    while (Thread.CurrentThread.IsAlive)
    {
        _clients.RemoveAll(x => !x.Connected);
        Thread.Sleep(10);
    }
}

void MsgInput()
{
    while (Thread.CurrentThread.IsAlive)
    {
        List<Socket> toRead = new(_clients);
        Socket.Select(toRead, null, null, -1);
        foreach (var r in toRead)
        {
            c.C
        }
        Thread.Sleep(10);
    }
}

Console.WriteLine("What port do you want to listen on (default 9999)");
string? portStr = Console.ReadLine();
Console.WriteLine();
int port;

if (string.IsNullOrEmpty(portStr))
{
    port = 9999;
}
else if (!int.TryParse(portStr, out port))
{
    port = 9999;
    Console.WriteLine("Failed to parse input, falling back on default port");
}

new Thread(new ThreadStart(MainLoop)).Start();
new Thread(new ThreadStart(MsgInput)).Start();

Console.WriteLine($"Starting to listen on {port}...");
try
{
    var listener = new TcpListener(IPAddress.Any, port);

    while (true)
    {
        _clients.Add(listener.AcceptTcpClient().Client);
    }
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"An error occured: {e}");
}
