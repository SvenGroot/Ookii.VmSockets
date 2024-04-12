using HvSocketClient;
using Ookii.VmSockets;
using System.Net.Sockets;

var arguments = Arguments.Parse();
if (arguments == null)
{
    return 1;
}

if (!OperatingSystem.IsWindowsVersionAtLeast(10))
{
    Console.WriteLine("Hyper-V sockets are only supported on Windows 10 and later.");
    return 1;
}

using var socket = HvSocket.Create(SocketType.Stream);
var vmId = arguments.VmId ?? HvSocket.Parent;
var endpoint = new HvSocketEndPoint(vmId, arguments.Port);
Console.WriteLine($"Connecting to {endpoint}...");
socket.Connect(endpoint);
Console.WriteLine($"Connected with local endpoint {socket.LocalEndPoint}.");
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
