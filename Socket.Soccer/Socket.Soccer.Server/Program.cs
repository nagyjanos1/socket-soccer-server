using Microsoft.Extensions.Logging;
using WebSocketSharp.Server;

var wssv = new WebSocketServer ("wss://localhost:4649");
#if DEBUG
wssv.Log.Level = (global::WebSocketSharp.LogLevel)LogLevel.Trace;
#endif

wssv.AddWebSocketService<Echo>("/Echo");

wssv.Start();
if (wssv.IsListening)
{
    Console.WriteLine("A WebSocket Server listening on port: {0} service paths:", wssv.Port);

    foreach (var path in wssv.WebSocketServices.Paths)
    {
        Console.WriteLine("  {0}", path);
    }
}

Console.WriteLine("\nPress Enter key to stop server...");
Console.ReadLine();

wssv.Stop();