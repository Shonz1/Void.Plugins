using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;

namespace Menu.Protocol.Packets.Serverbound;

public class CloseContainerServerboundPacket : IMinecraftServerboundPacket<CloseContainerServerboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings =
  [
    new(0x0D, ProtocolVersion.MINECRAFT_1_7_2),
    new(0x08, ProtocolVersion.MINECRAFT_1_9),
    new(0x09, ProtocolVersion.MINECRAFT_1_12),
    new(0x08, ProtocolVersion.MINECRAFT_1_12_1),
    new(0x09, ProtocolVersion.MINECRAFT_1_13),
    new(0x0A, ProtocolVersion.MINECRAFT_1_14),
    new(0x09, ProtocolVersion.MINECRAFT_1_17),
    new(0x0B, ProtocolVersion.MINECRAFT_1_19),
    new(0x0C, ProtocolVersion.MINECRAFT_1_19_1),
    new(0x0B, ProtocolVersion.MINECRAFT_1_19_3),
    new(0x0C, ProtocolVersion.MINECRAFT_1_19_4),
    new(0x0E, ProtocolVersion.MINECRAFT_1_20_2),
    new(0x0F, ProtocolVersion.MINECRAFT_1_20_5),
    new(0x11, ProtocolVersion.MINECRAFT_1_21_2)
  ];

  public static CloseContainerServerboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    return new CloseContainerServerboundPacket();
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
