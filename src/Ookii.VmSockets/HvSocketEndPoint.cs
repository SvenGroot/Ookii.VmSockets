using System.Net;
using System.Net.Sockets;

namespace Ookii.VmSockets;

public class HvSocketEndPoint : EndPoint
{
    public HvSocketEndPoint(Guid vmId, Guid serviceId)
    {
        VmId = vmId;
        ServiceId = serviceId;
    }

    public HvSocketEndPoint(Guid vmId, int port)
    {
        VmId = vmId;
        ServiceId = HvSocket.CreateVSockServiceId(port);
    }

    public override AddressFamily AddressFamily => HvSocket.AddressFamily;

    public override EndPoint Create(SocketAddress socketAddress)
    {
        var vmId = socketAddress.ReadGuid(4);
        var serviceId = socketAddress.ReadGuid(20);
        return new HvSocketEndPoint(vmId, serviceId);
    }

    public Guid VmId { get; set; }

    public Guid ServiceId { get; set; }

    public override SocketAddress Serialize()
    {
        var address = new SocketAddress(AddressFamily, HvSocket.SocketAddressSize);
        address.WriteGuid(4, VmId);
        address.WriteGuid(20, ServiceId);
        return address;
    }
}
