# Hyper-V socket server sample

This sample creates a simple server that listens for connections from the [Hyper-V socket client](../HvSocketClient/)
or [VSock client](../VSockClient/) sample. It accepts a single connection, reads strings from the
socket, and sends the strings back to the client with their character order reversed.

For simplicity, this sample uses VSock port numbers, which will work regardless of whether the
client is using Hyper-V sockets on Windows or VSock sockets on Linux.

By default, the server listens on VSock port 500000 to connections from any partitions, including
any guest VMs and, if executed in a VM, from the host. You can use a command-line argument to
specify a specific VM ID to listen to when running on the host, and to change the port number.

Use the `Get-VM` PowerShell cmdlet to get the VM ID for a particular VM, or `hcsdiag.exe list` to
get the VM ID for the Windows Subsystem for Linux (WSL) utility VM.

For more information, run `./HvSocketServer -Help`.

```text
A simple server using Hyper-V sockets.

Usage: HvSocketServer [[-VmId] <Guid>] [[-Port] <Int32>] [-Help] [-Version]

    -VmId <Guid>
        The VM ID to accept connections from. If not specified, the wildcard ID is used.

    -Port <Int32>
        The vsock port to listen on. Default value: 500000.

    -Help [<Boolean>] (-?, -h)
        Displays this help message.

    -Version [<Boolean>]
        Displays version information.
```

Use the `Get-VM` PowerShell cmdlet to get the VM ID for a particular VM.

> [!IMPORTANT]
> When running this server on the host, you must
> [add your service ID to the registry](https://learn.microsoft.com/virtualization/hyper-v-on-windows/user-guide/make-integration-service).
> Use [host_default_service_id.reg](host_default_service_id.reg) to add registry entries for the
> default service ID matching VSock port 500000. If you run the server in a guest, and the client on
> the host, this is not required.

When attempting to accept connections from the Windows Subsystem for Linux (WSL), it may be
necessary to use the WSL utility VM's ID directly, as the wildcard ID may not work. To get this ID,
you can run `hcsdiag.exe list`.

This sample uses [Ookii.CommandLine](https://www.github.com/SvenGroot/Ookii.CommandLine) to parse
command-line arguments.
