# Hyper-V socket client sample

This sample is a simple client that can connect to the [Hyper-V socket server](../HvSocketServer/)
or [VSock server](../VSockServer/) sample. It connects to the server, and prompts for strings to
send to the server. It then prints the server's response. Enter an empty string to exit the
application.

For simplicity, this sample uses VSock port numbers, which will work regardless of whether the
server is using Hyper-V sockets on Windows or VSock sockets on Linux.

By default, the client connects to the parent VM ID (which is useful only when running the client
in the guest), and on VSock port 500000. This can be changed by using command-line arguments (this
sample uses [Ookii.CommandLine](https://www.github.com/SvenGroot/Ookii.CommandLine) to parse
command-line arguments). For more information, run `./HvSocketClient -Help`.

Use the `Get-VM` PowerShell cmdlet to get the VM ID for a particular VM, or `hcsdiag.exe list` to
get the VM ID for the Windows Subsystem for Linux (WSL) utility VM.
