using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Api.Network;

namespace Menus.Protocol.Packets.Clientbound;

public class OpenContainerClientboundPacket : IMinecraftClientboundPacket<OpenContainerClientboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings =
  [
    new(0x2D, ProtocolVersion.MINECRAFT_1_7_2),
    new(0x13, ProtocolVersion.MINECRAFT_1_9),
    new(0x14, ProtocolVersion.MINECRAFT_1_13),
    new(0x2E, ProtocolVersion.MINECRAFT_1_14),
    new(0x2F, ProtocolVersion.MINECRAFT_1_15),
    new(0x2E, ProtocolVersion.MINECRAFT_1_16),
    new(0x2D, ProtocolVersion.MINECRAFT_1_16_2),
    new(0x2E, ProtocolVersion.MINECRAFT_1_17),
    new(0x2B, ProtocolVersion.MINECRAFT_1_19),
    new(0x2D, ProtocolVersion.MINECRAFT_1_19_1),
    new(0x2C, ProtocolVersion.MINECRAFT_1_19_3),
    new(0x30, ProtocolVersion.MINECRAFT_1_19_4),
    new(0x31, ProtocolVersion.MINECRAFT_1_20_2),
    new(0x33, ProtocolVersion.MINECRAFT_1_20_5),

    .. ProtocolVersion
      .Range(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.Latest)
      .Select(i => new MinecraftPacketIdMapping(
        MinecraftPacketRegistry.GetId(i, Phase.Play, Direction.Clientbound, "minecraft:open_screen"), i))
  ];

  public int ContainerId { get; set; }
  public int Type { get; set; }
  public Component Title { get; set; } = Component.Default;

  public static OpenContainerClientboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    return new OpenContainerClientboundPacket();
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    buffer.WriteVarInt(ContainerId);
    buffer.WriteVarInt(Type);
    buffer.WriteComponent(Title);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
