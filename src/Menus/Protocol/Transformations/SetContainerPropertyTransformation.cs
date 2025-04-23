using Microsoft.Extensions.Logging;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;
using Void.Proxy.Api.Network;

namespace Menus.Protocol.Transformations;

public class SetContainerPropertyTransformation(ILogger<SetContainerPropertyTransformation> logger)
{
  public void DowngradeTo_1_21(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    if (wrapper.Origin != Side.Proxy)
      return;

    logger.LogTrace($"Downgrade to 1.21 {fromProtocolVersion} -> {toProtocolVersion}");

    var containerId = wrapper.Read<VarIntProperty>();
    wrapper.Write(ByteProperty.FromPrimitive((byte)containerId.AsPrimitive));

    wrapper.Passthrough<ShortProperty>(); // Property id
    wrapper.Passthrough<ShortProperty>(); // Value
  }
}
