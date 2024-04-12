using Ookii.CommandLine;
using System.ComponentModel;

namespace HvSocketClient;

[GeneratedParser]
[Description("A simple client using Hyper-V sockets.")]
partial class Arguments
{
    [CommandLineArgument(IsPositional = true)]
    [Description("The VM ID to connect to. If not specified, the parent of the current VM is used.")]
    public Guid? VmId { get; set; }

    [CommandLineArgument(IsPositional = true)]
    [Description("The vsock port to connect to.")]
    public int Port { get; set; } = 500000;
}
