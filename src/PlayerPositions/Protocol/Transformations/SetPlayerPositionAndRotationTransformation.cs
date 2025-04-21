using Microsoft.Extensions.Logging;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace PlayerPositions.Protocol.Transformations;

public class SetPlayerPositionAndRotationTransformation(ILogger<SetPlayerPositionAndRotationTransformation> logger)
{
  public void DowngradeTo_1_7_2(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"Downgrade to 1.7.2 {fromProtocolVersion} -> {toProtocolVersion}");

    wrapper.Passthrough<DoubleProperty>(); // X
    var y = wrapper.Read<DoubleProperty>();
    wrapper.Write(y);
    wrapper.Write(y);
    wrapper.Passthrough<DoubleProperty>(); // Z
    wrapper.Passthrough<FloatProperty>(); // Yaw
    wrapper.Passthrough<FloatProperty>(); // Pitch

    var flags = wrapper.Read<ByteProperty>();
    wrapper.Write(BoolProperty.FromPrimitive((flags.AsPrimitive & 0x01) == 0)); // Mode
  }
}
