using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Proxy.Api.Network;

namespace PlayerPositions.Protocol.Packets.Serverbound;

public class SetPlayerPositionServerboundPacket : IMinecraftServerboundPacket<SetPlayerPositionServerboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x04, ProtocolVersion.MINECRAFT_1_7_2),

    .. ProtocolVersion
      .Range(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.Latest)
      .Select(i => new MinecraftPacketIdMapping(
        MinecraftPacketRegistry.GetId(i, Phase.Play, Direction.Serverbound, "minecraft:move_player_pos"), i))
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
