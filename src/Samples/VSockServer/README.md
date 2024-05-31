# VSock server sample

This sample creates a simple server that listens for connections from the [Hyper-V socket client](../HvSocketClient/)
or [VSock client](../VSockClient/) sample. It accepts a single connection, reads strings from the
socket, and sends the strings back to the client with their character order reversed.

By default, the server listens on VSock port 500000 to connections from any partitions, including
any guest VMs and, if executed in a VM, from the host. You can use a command-line argument to
specify a specific CID to listen to when running on the host, and to change the port number.

For more information, run `./VSockServer --help`:

```text
A simple server using VSock sockets.

Usage: VSockServer [[--context-id] <int32>] [[--port] <int32>] [--help] [--version]

    -c, --context-id <int32>
            The context ID to listen to. If not specified, the wildcard ID is used.

    -p, --port <int32>
            The port to listen on. Default value: 500000.

    -?, --help [<boolean>] (-h)
            Displays this help message.

        --version [<boolean>]
            Displays version information.
```text

This sample uses [Ookii.CommandLine](https://www.github.com/SvenGroot/Ookii.CommandLine) to parse
command-line arguments.
