# VSock server sample

This sample creates a simple server that listens for connections from the [Hyper-V socket client](../HvSocketClient/)
or [VSock client](../VSockClient/) sample. It accepts a single connection, reads strings from the
socket, and sends the strings back to the client with their character order reversed.

By default, the server listens using the wildcard CID, and on VSock port 500000. This can be
changed by using command-line arguments (this sample uses [Ookii.CommandLine](https://www.github.com/SvenGroot/Ookii.CommandLine)
to parse command-line arguments). For more information, run `./VSockServer --help`.
