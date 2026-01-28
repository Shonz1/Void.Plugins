using TabLists.Minecraft.PlayerInfo.Actions;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Minecraft.Profiles;

namespace TabLists.Protocol.Packets.Clientbound;

public class PlayerInfoUpdateClientboundPacket : IMinecraftClientboundPacket<PlayerInfoUpdateClientboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x38, ProtocolVersion.MINECRAFT_1_7_2),
    new(0x2D, ProtocolVersion.MINECRAFT_1_9),
    new(0x2E, ProtocolVersion.MINECRAFT_1_12_1),
    new(0x30, ProtocolVersion.MINECRAFT_1_13),
    new(0x33, ProtocolVersion.MINECRAFT_1_14),
    new(0x34, ProtocolVersion.MINECRAFT_1_15),
    new(0x33, ProtocolVersion.MINECRAFT_1_16),
    new(0x32, ProtocolVersion.MINECRAFT_1_16_2),
    new(0x36, ProtocolVersion.MINECRAFT_1_17),
    new(0x34, ProtocolVersion.MINECRAFT_1_19),
    new(0x37, ProtocolVersion.MINECRAFT_1_19_1), // Remove method in this packet
    new(0x36, ProtocolVersion.MINECRAFT_1_19_3),
    new(0x3A, ProtocolVersion.MINECRAFT_1_19_4),
    new(0x3C, ProtocolVersion.MINECRAFT_1_20_2),
    new(0x3E, ProtocolVersion.MINECRAFT_1_20_5),
    new(0x40, ProtocolVersion.MINECRAFT_1_21_4),
    new(0x3F, ProtocolVersion.MINECRAFT_1_21_5)
  ];

  public Dictionary<Uuid, List<IPlayerInfoAction>> PlayerInfoActions { get; set; } = new ();

  public static PlayerInfoUpdateClientboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    return new PlayerInfoUpdateClientboundPacket();
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    var actionBitset = 0;

    foreach (var (_, actions) in PlayerInfoActions)
      foreach (var action in actions)
        actionBitset |= action.ActionId;

    buffer.WriteUnsignedByte((byte) actionBitset);

    buffer.WriteVarInt(PlayerInfoActions.Count);

    foreach (var (uuid, actions) in PlayerInfoActions)
    {
      buffer.WriteUuid(uuid);

      actions.Sort((a, b) => a.ActionId > b.ActionId ? 1 : -1);

      foreach (var action in actions)
        action.Write(ref buffer);
    }
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
