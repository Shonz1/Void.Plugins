using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;

namespace Menu.Protocol.Packets.Serverbound;

public class SetHeldItemServerboundPacket : IMinecraftServerboundPacket<SetHeldItemServerboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x09, ProtocolVersion.MINECRAFT_1_7_2),
    new(0x17, ProtocolVersion.MINECRAFT_1_9),
    new(0x1A, ProtocolVersion.MINECRAFT_1_12),
    new(0x21, ProtocolVersion.MINECRAFT_1_13),
    new(0x23, ProtocolVersion.MINECRAFT_1_14),
    new(0x24, ProtocolVersion.MINECRAFT_1_16),
    new(0x25, ProtocolVersion.MINECRAFT_1_16_2),
    new(0x27, ProtocolVersion.MINECRAFT_1_19),
    new(0x28, ProtocolVersion.MINECRAFT_1_19_1),
    new(0x2B, ProtocolVersion.MINECRAFT_1_20_2),
    new(0x2C, ProtocolVersion.MINECRAFT_1_20_3),
    new(0x2F, ProtocolVersion.MINECRAFT_1_20_5),
    new(0x31, ProtocolVersion.MINECRAFT_1_21_2),
    new(0x33, ProtocolVersion.MINECRAFT_1_21_4)
  ];

  public int Slot { get; set; }

  public static SetHeldItemServerboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    return new SetHeldItemServerboundPacket
    {
      Slot = buffer.ReadUnsignedShort()
    };
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    buffer.WriteUnsignedShort((ushort) Slot);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
