using Ookii.VmSockets;
using System.Net.Sockets;
using VSockClient;

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

using var socket = VSock.Create(SocketType.Stream);
var contextId = arguments.ContextId ?? VSock.Host;
var endpoint = new VSockEndPoint(contextId, arguments.Port);
Console.WriteLine($"Connecting to {endpoint}...");
socket.Connect(endpoint);
Console.WriteLine($"Connected.");
using var stream = new NetworkStream(socket, true);
using var reader = new BinaryReader(stream);
using var writer = new BinaryWriter(stream);

while (true)
{
    Console.Write("Enter a string to send to the server (empty to exit): ");
    var value = Console.ReadLine();
    if (string.IsNullOrEmpty(value))
    {
        break;
    }

    writer.Write(value);
    var response = reader.ReadString();
    Console.WriteLine($"Server responded: {response}");
}

return 0;
