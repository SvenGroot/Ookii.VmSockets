# VSock client sample

This sample is a simple client that can connect to the [Hyper-V socket server](../HvSocketServer/)
or [VSock server](../VSockServer/) sample. It connects to the server, and prompts for strings to
send to the server. It then prints the server's response. Enter an empty string to exit the
application.

By default, the client connects to the host CID (which is useful only when running the client in
the guest), and on VSock port 500000. You can use a command-line argument to specify a VM ID to
connect to when running on the host, and to change the port number.

For more information, run `./VSockClient --help`.

```text
A simple client using VSock sockets.

Usage: VSockClient [[--context-id] <int32>] [[--port] <int32>] [--help] [--version]

    -c, --context-id <int32>
            The context ID to connect to. If not specified, the host of the current VM is used.

    -p, --port <int32>
            The port to connect to. Default value: 500000.

    -?, --help [<boolean>] (-h)
            Displays this help message.

        --version [<boolean>]
            Displays version information.
```

This sample uses [Ookii.CommandLine](https://www.github.com/SvenGroot/Ookii.CommandLine) to parse
command-line arguments.
