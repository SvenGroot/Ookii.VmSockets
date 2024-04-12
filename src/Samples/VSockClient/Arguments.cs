using Ookii.CommandLine;
using System.ComponentModel;

namespace VSockClient;

[GeneratedParser]
[Description("A simple client using VSock sockets.")]
[ParseOptions(IsPosix = true)]
partial class Arguments
{
    [CommandLineArgument(IsPositional = true, IsShort = true)]
    [Description("The context ID to connect to. If not specified, the host of the current VM is used.")]
    public int? ContextId { get; set; }

    [CommandLineArgument(IsPositional = true, IsShort = true)]
    [Description("The port to connect to.")]
    public int Port { get; set; } = 500000;
}
