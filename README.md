# Ookii.VmSockets [![NuGet](https://img.shields.io/nuget/v/Ookii.VmSockets)](https://www.nuget.org/packages/Ookii.VmSockets/)

Ookii.VmSockets is a library that provides support for Hyper-V sockets (hvsocket) and Linux VSock
sockets for .Net applications. Both are used to communicate between the host and a guest virtual
machine, on Windows Hyper-V and on Linux respectively. It's also possible for a Linux guest,
including the Windows Subsystem for Linux (WSL), using VSock to communicate with a Windows host
using Hyper-V sockets.

To get started, keep reading or check out the [samples](src/Samples).

## Hyper-V sockets

Hyper-V sockets are sockets using the `AF_HYPERV` address family. They provide a method for a
Windows host to communicate with a guest VM, running Windows or Linux, using regular socket APIs.
Hyper-V sockets are supported on Windows 10 and later.

Ookii.VmSockets provides the [`HvSocketEndPoint`][] class, which can be used with the .Net
[`Socket`][] class to communicate with a VM, or in a guest to communicate with the host.

To create a Hyper-V endpoint, you need the ID of a VM to connect to or listen for connections from,
or you can use a predefined ID such as [`VmId.Parent`][] and [`VmId.Wildcard`][]. You also need a
service ID, which can be any arbitrary GUID which identifies the connection.

Additionally, the [`HvSocket`][] class provides a number of helper methods to create a socket or set
socket options.

The following example creates a socket that listens for connections from any source, where
`ServiceId` is a GUID that represents your service:

```csharp
using var server = HvSocket.Create(SocketType.Stream);
server.Bind(new HvSocketEndPoint(HvSocket.Wildcard, ServiceId));
server.Listen(1);
var socket = server.Accept();
```

> [!IMPORTANT]
> When using Hyper-V sockets on the host to listen for connections from a guest, you must
> [add your service ID to the registry](https://learn.microsoft.com/virtualization/hyper-v-on-windows/user-guide/make-integration-service).
> This is not required if a guest listens for connections from the host, with the client using
> [`Socket.Connect()`][] running on the host.

The [`HvSocketEndPoint`][] class can connect to a Linux guest using VSock by using the
[`HvSocketEndPoint(Guid, int)`][] constructor, specifying a VSock port number instead of a service
ID. The below example connects to a Linux VM:

```csharp
public Socket ConnectToLinuxVm(Guid vmId, int port)
{
    var socket = HvSocket.Create(SocketType.Stream);
    server.Connect(new HvSocketEndPoint(vmId, port));
    return server;
}
```

To determine the ID of a particular VM, the easiest method is to use the [`Get-VM`][] PowerShell
cmdlet. For example:

```pwsh
Get-VM | Format-Table -Property Name,Id
```

To determine the ID of the Windows Subsystem for Linux (WSL) utility VM, as well as other HCS VMs
running on your system, you can use `hcsdiag.exe list` instead.

## VSock sockets

Linux provides for communication between a host and guest VMs using
[VSock](https://www.man7.org/linux/man-pages/man7/vsock.7.html), which uses the `AF_VSOCK` address
family.

> [!IMPORTANT]
> Due to limitations[^1] in the .Net sockets implementation on Linux, VSock support is only
> available for .Net 8.0 and later.

Ookii.VmSockets provides the [`VSockEndPoint`][] class to represent a VSock address, which consists
of a context ID (CID), which can be a predefined ID from the [`ContextId`][] class, and a port number.
The [`VSock`][] class provides helper functions and constants.

> [!IMPORTANT]
> Unlike with Hyper-V sockets, you *must* use the [`VSock.Create()`][] method to create a VSock
> socket, because on Linux, the regular [`Socket`][] class constructor does not allow the use of
> address families other than the ones predefined by .Net. The [`VSock.Create()`][] method uses
> PInvoke with the libc `socket` function to create the socket instead.

The following example listens for connections on port 50000:

```csharp
using var server = VSock.Create(SocketType.Stream);
server.Bind(new VSockEndPoint(VSock.Any, 50000));
server.Listen(1);
var socket = server.Accept();
```

Because of the limitations imposed by .Net, some properties of the [`Socket`][] class may not return
the correct values when using VSock sockets.

## Requirements

Ookii.VmSockets is a class library for use in your own applications for
[Microsoft .Net](https://dotnet.microsoft.com/). It can be used with applications supporting one of
the following:

- .Net Standard 2.0
- .Net Standard 2.1
- .Net 6.0
- .Net 8.0 and later

Linux VSock support is only available for .Net 8.0 and later.

## Building and testing

To build Ookii.VmSockets, make sure you have the following installed:

- [Microsoft .Net 8.0 SDK](https://dotnet.microsoft.com/download) or later

To build the library, tests and samples, simply use the `dotnet build` command in the `src`
directory. You can run the unit tests using `dotnet test`. The tests can be run on Windows and
Linux. Some tests will be skipped depending on the platform being used.

The tests are built and run for .Net 8.0 and .Net Framework 4.8. The .Net Framework tests are
not relevant on Linux, as VSock is not supported on .Net Framework and Hyper-V sockets are not
supported on Linux.

The class library documentation is generated using
[Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB).

## Learn more

- [What's new in Ookii.BinarySize](docs/ChangeLog.md)
- [Class library documentation](https://www.ookii.org/Link/VmSocketsDoc)
- [Samples](src/Samples)

[^1]: In .Net on Linux, it is not possible to create an instance of the [`SocketAddress`][] class
    with an address family that is not predefined by .Net. In .Net 8, this can be worked around by
    directly modifying the address family in the buffer of the [`SocketAddress`][] instance, but
    accessing the buffer in this way is only possible in .Net 8 and later. Without this ability,
    it's not possible to write a functional [`VSockEndPoint`][] class.

[`ContextId`]: https://www.ookii.org/docs/vmsockets-1.0/html/T_Ookii_VmSockets_ContextId.htm
[`Get-VM`]: https://learn.microsoft.com/powershell/module/hyper-v/get-vm?view=windowsserver2022-ps
[`HvSocket`]: https://www.ookii.org/docs/vmsockets-1.0/html/T_Ookii_VmSockets_HvSocket.htm
[`HvSocketEndPoint(Guid, int)`]: https://www.ookii.org/docs/vmsockets-1.0/html/M_Ookii_VmSockets_HvSocketEndPoint__ctor_1.htm
[`HvSocketEndPoint`]: https://www.ookii.org/docs/vmsockets-1.0/html/T_Ookii_VmSockets_HvSocketEndPoint.htm
[`Socket.Connect()`]: https://learn.microsoft.com/dotnet/api/system.net.sockets.socket.connect
[`Socket`]: https://learn.microsoft.com/dotnet/api/system.net.sockets.socket
[`SocketAddress`]: https://learn.microsoft.com/dotnet/api/system.net.socketaddress
[`VmId.Parent`]: https://www.ookii.org/docs/vmsockets-1.0/html/F_Ookii_VmSockets_VmId_Parent.htm
[`VmId.Wildcard`]: https://www.ookii.org/docs/vmsockets-1.0/html/F_Ookii_VmSockets_VmId_Wildcard.htm
[`VSock.Create()`]: https://www.ookii.org/docs/vmsockets-1.0/html/M_Ookii_VmSockets_VSock_Create.htm
[`VSock`]: https://www.ookii.org/docs/vmsockets-1.0/html/T_Ookii_VmSockets_VSock.htm
[`VSockEndPoint`]: https://www.ookii.org/docs/vmsockets-1.0/html/T_Ookii_VmSockets_VSockEndPoint.htm
