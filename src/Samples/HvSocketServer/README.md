# Hyper-V socket server sample

This sample creates a simple server that listens for connections from the [Hyper-V socket client](../HvSocketClient/)
or [VSock client](../VSockClient/) sample. It accepts a single connection, reads strings from the
socket, and sends the strings back to the client with their character order reversed.

For simplicity, this sample uses VSock port numbers, which will work regardless of whether the
client is using Hyper-V sockets on Windows or VSock sockets on Linux.

By default, the server listens using the wildcard VM ID, and on VSock port 500000. This can be
changed by using command-line arguments (this sample uses [Ookii.CommandLine](https://www.github.com/SvenGroot/Ookii.CommandLine)
to parse command-line arguments). For more information, run `./HvSocketServer -Help`.

Use the `Get-VM` PowerShell cmdlet to get the VM ID for a particular VM.

> [!IMPORTANT]
> When running this service on the host on Windows, you must
> [add your service ID to the registry](https://learn.microsoft.com/virtualization/hyper-v-on-windows/user-guide/make-integration-service).
> Use [host_default_service_id.reg](host_default_service_id.reg) to add registry entries for the
> default service ID using VSock port 500000. If you run the server in a guest, and the client on
> the host, this is not required.

When attempting to accept connections from the Windows Subsystem for Linux (WSL), it may be
necessary to use the WSL utility VM's ID directly, as the wildcard ID may not work. To get this ID,
you can run `hcsdiag.exe list`.
