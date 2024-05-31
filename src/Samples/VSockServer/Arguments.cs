using Ookii.CommandLine;
using System.ComponentModel;

namespace VSockServer;

[GeneratedParser]
[Description("A simple server using VSock sockets.")]
[ParseOptions(IsPosix = true)]
partial class Arguments
{
    [CommandLineArgument(IsPositional = true, IsShort = true)]
    [Description("The context ID to listen to. If not specified, the wildcard ID is used.")]
    public int? ContextId { get; set; }

    [CommandLineArgument(IsPositional = true, IsShort = true)]
    [Description("The port to listen on.")]
    public int Port { get; set; } = 500000;
}
