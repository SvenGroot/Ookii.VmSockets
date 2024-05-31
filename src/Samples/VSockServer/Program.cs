using Ookii.CommandLine;
using Ookii.VmSockets;
using System.Net.Sockets;
using VSockServer;

var arguments = Arguments.Parse();
if (arguments == null)
{
    return 1;
}

if (!OperatingSystem.IsLinux())
{
    Console.WriteLine("VSock sockets are only supported on Linux.");
    return 1;
}

var contextId = arguments.ContextId ?? ContextId.Any;
var endpoint = new VSockEndPoint(contextId, arguments.Port);
using var listener = VSock.Create(SocketType.Stream);
listener.Bind(endpoint);
listener.Listen(1);
Console.WriteLine("Listening for connections...");
using var socket = listener.Accept();
Console.WriteLine($"Connected to {socket.RemoteEndPoint}");
Console.WriteLine(VSock.GetTrusted(socket));
using var stream = new NetworkStream(socket, true);
using var reader = new BinaryReader(stream);
using var writer = new BinaryWriter(stream);

try
{
    while (true)
    {
        var value = reader.ReadString();
        Console.WriteLine($"Received: {value}");
        writer.Write(new string(value.Reverse().ToArray()));
    }
}
catch (EndOfStreamException)
{
    Console.WriteLine("Client disconnected.");
}

return 0;
