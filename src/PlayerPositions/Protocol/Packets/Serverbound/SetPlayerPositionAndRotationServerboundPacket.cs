using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;

namespace PlayerPositions.Protocol.Packets.Serverbound;

public class SetPlayerPositionAndRotationServerboundPacket : IMinecraftServerboundPacket<SetPlayerPositionAndRotationServerboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x06, ProtocolVersion.MINECRAFT_1_7_2),
    new(0x1D, ProtocolVersion.MINECRAFT_1_21_5)
  ];

  public double X { get; set; }
  public double Y { get; set; }
  public double Z { get; set; }
  public float Yaw { get; set; }
  public float Pitch { get; set; }
  public int Flags { get; set; }

  public static SetPlayerPositionAndRotationServerboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    return new SetPlayerPositionAndRotationServerboundPacket
    {
      X = buffer.ReadDouble(),
      Y = buffer.ReadDouble(),
      Z = buffer.ReadDouble(),
      Yaw = buffer.ReadFloat(),
      Pitch = buffer.ReadFloat(),
      Flags = buffer.ReadUnsignedByte()
    };
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    buffer.WriteDouble(X);
    buffer.WriteDouble(Y);
    buffer.WriteDouble(Z);
    buffer.WriteDouble(Yaw);
    buffer.WriteDouble(Pitch);
    buffer.WriteUnsignedByte((byte) Flags);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
