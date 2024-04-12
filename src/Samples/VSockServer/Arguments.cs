using Ookii.CommandLine;
using System.ComponentModel;

namespace VSockServer;

[GeneratedParser]
[Description("A simple server using VSock sockets.")]
partial class Arguments
{
    [CommandLineArgument(IsPositional = true)]
    [Description("The port to listen on.")]
    public int Port { get; set; } = 500000;
}
