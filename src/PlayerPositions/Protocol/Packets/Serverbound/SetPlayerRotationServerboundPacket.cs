using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;

namespace PlayerPositions.Protocol.Packets.Serverbound;

public class SetPlayerRotationServerboundPacket : IMinecraftServerboundPacket<SetPlayerRotationServerboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x05, ProtocolVersion.MINECRAFT_1_7_2),
    new(0x1E, ProtocolVersion.MINECRAFT_1_21_5)
  ];

  public float Yaw { get; set; }
  public float Pitch { get; set; }
  public int Flags { get; set; }

  public static SetPlayerRotationServerboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    return new SetPlayerRotationServerboundPacket
    {
      Yaw = buffer.ReadFloat(),
      Pitch = buffer.ReadFloat(),
      Flags = buffer.ReadUnsignedByte()
    };
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    buffer.WriteDouble(Yaw);
    buffer.WriteDouble(Pitch);
    buffer.WriteUnsignedByte((byte) Flags);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
