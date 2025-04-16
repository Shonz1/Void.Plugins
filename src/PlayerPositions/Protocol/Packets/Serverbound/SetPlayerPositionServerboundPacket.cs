using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;

namespace PlayerPositions.Protocol.Packets.Serverbound;

public class SetPlayerPositionServerboundPacket : IMinecraftServerboundPacket<SetPlayerPositionServerboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x1C, ProtocolVersion.MINECRAFT_1_21_5)
  ];

  public double X { get; set; }
  public double Y { get; set; }
  public double Z { get; set; }
  public int Flags { get; set; }

  public static SetPlayerPositionServerboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    return new SetPlayerPositionServerboundPacket
    {
      X = buffer.ReadDouble(),
      Y = buffer.ReadDouble(),
      Z = buffer.ReadDouble(),
      Flags = buffer.ReadUnsignedByte()
    };
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    buffer.WriteDouble(X);
    buffer.WriteDouble(Y);
    buffer.WriteDouble(Z);
    buffer.WriteUnsignedByte((byte) Flags);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
