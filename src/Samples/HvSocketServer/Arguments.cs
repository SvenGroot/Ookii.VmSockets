using Ookii.CommandLine;
using System.ComponentModel;

namespace HvSocketServer;

[GeneratedParser]
[Description("A simple server using Hyper-V sockets.")]
partial class Arguments
{
    [CommandLineArgument(IsPositional = true)]
    [Description("The VM ID to accept connections from. If not specified, the wildcard ID is used.")]
    public Guid? VmId { get; set; }

    [CommandLineArgument(IsPositional = true)]
    [Description("The vsock port to listen on.")]
    public int Port { get; set; } = 500000;
}
