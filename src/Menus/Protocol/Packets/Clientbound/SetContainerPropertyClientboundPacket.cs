using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Api.Network;

namespace Menus.Protocol.Packets.Clientbound;

public class SetContainerPropertyClientboundPacket : IMinecraftClientboundPacket<SetContainerPropertyClientboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x31, ProtocolVersion.MINECRAFT_1_7_2),
    new(0x15, ProtocolVersion.MINECRAFT_1_9),
    new(0x16, ProtocolVersion.MINECRAFT_1_13),
    new(0x15, ProtocolVersion.MINECRAFT_1_14),
    new(0x16, ProtocolVersion.MINECRAFT_1_15),
    new(0x15, ProtocolVersion.MINECRAFT_1_16),
    new(0x14, ProtocolVersion.MINECRAFT_1_16_2),
    new(0x15, ProtocolVersion.MINECRAFT_1_17),
    new(0x12, ProtocolVersion.MINECRAFT_1_19),
    new(0x11, ProtocolVersion.MINECRAFT_1_19_3),
    new(0x13, ProtocolVersion.MINECRAFT_1_19_4),
    new(0x14, ProtocolVersion.MINECRAFT_1_20_2),

    .. ProtocolVersion
      .Range(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.Latest)
      .Select(i => new MinecraftPacketIdMapping(
        MinecraftPacketRegistry.GetId(i, Phase.Play, Direction.Clientbound, "minecraft:container_set_data"), i))
  ];

  public int ContainerId { get; set; }
  public int Property { get; set; }
  public int Value { get; set; }

  public static SetContainerPropertyClientboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    return new SetContainerPropertyClientboundPacket
    {
      ContainerId = buffer.ReadVarInt(),
      Property = buffer.ReadShort(),
      Value = buffer.ReadShort()
    };
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    buffer.WriteVarInt(ContainerId);
    buffer.WriteShort((short) Property);
    buffer.WriteShort((short) Value);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
