using HvSocketServer;
using Ookii.CommandLine;
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

var vmId = arguments.VmId ?? VmId.Wildcard;
var endpoint = new HvSocketEndPoint(vmId, arguments.Port);
using (var wrapped = LineWrappingTextWriter.ForConsoleError())
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    wrapped.WriteLine($"WARNING: If you are running this server on the host to accept connections from a guest, you must add the appropriate registry entries for service ID {{{endpoint.ServiceId}}}. This is not necessary if the server is running in a guest to accept connections from the host.");
    wrapped.WriteLine("See https://learn.microsoft.com/virtualization/hyper-v-on-windows/user-guide/make-integration-service");
    Console.ResetColor();
}

using var listener = HvSocket.Create(SocketType.Stream);
listener.Bind(endpoint);
listener.Listen(1);
Console.WriteLine("Listening for connections...");
using var socket = listener.Accept();
Console.WriteLine($"Connected to {socket.RemoteEndPoint}");
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
