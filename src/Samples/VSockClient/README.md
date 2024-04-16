# VSock client sample

This sample is a simple client that can connect to the [Hyper-V socket server](../HvSocketServer/)
or [VSock server](../VSockServer/) sample. It connects to the server, and prompts for strings to
send to the server. It then prints the server's response. Enter an empty string to exit the
application.

By default, the client connects to the host CID (which is useful only when running the client
in the guest), and on VSock port 500000. This can be changed by using command-line arguments (this
sample uses [Ookii.CommandLine](https://www.github.com/SvenGroot/Ookii.CommandLine) to parse
command-line arguments). For more information, run `./VSockClient -Help`.
