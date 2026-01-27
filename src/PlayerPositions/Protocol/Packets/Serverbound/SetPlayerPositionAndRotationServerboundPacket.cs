using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Api.Network;

namespace PlayerPositions.Protocol.Packets.Serverbound;

public class SetPlayerPositionAndRotationServerboundPacket : IMinecraftServerboundPacket<SetPlayerPositionAndRotationServerboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x06, ProtocolVersion.MINECRAFT_1_7_2),

    .. ProtocolVersion
      .Range(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.Latest)
      .Select(i => new MinecraftPacketIdMapping(
        MinecraftPacketRegistry.GetId(i, Phase.Play, Direction.Serverbound, "minecraft:move_player_pos_rot"), i))
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
