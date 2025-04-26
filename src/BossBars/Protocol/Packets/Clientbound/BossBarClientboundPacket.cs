using BossBars.Minecraft.BossBar.Actions;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId.Mappings;
using Void.Minecraft.Profiles;

namespace BossBars.Protocol.Packets.Clientbound;

internal class BossBarClientboundPacket : IMinecraftClientboundPacket<BossBarClientboundPacket>
{
  public static readonly MinecraftPacketIdMapping[] Mappings = [
    new(0x0A, ProtocolVersion.MINECRAFT_1_21),
    new(0x09, ProtocolVersion.MINECRAFT_1_21_5),
  ];

  public Uuid BossBarId { get; set; }
  public required IBossBarAction Action { get; set; }

  public static BossBarClientboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    var bossBarId = buffer.ReadUuid();
    var actionId = buffer.ReadVarInt();

    IBossBarAction action = actionId switch
    {
      0x00 => AddBossBarAction.Read(ref buffer),
      0x01 => RemoveBossBarAction.Read(ref buffer),
      0x02 => UpdateHealthBossBarAction.Read(ref buffer),
      0x03 => UpdateTitleBossBarAction.Read(ref buffer),
      0x04 => UpdateStyleBossBarAction.Read(ref buffer),
      0x05 => UpdateFlagsBossBarAction.Read(ref buffer),
      _ => throw new ArgumentOutOfRangeException(nameof(actionId), $"Unknown action ID: {actionId}")
    };

    return new BossBarClientboundPacket
    {
      BossBarId = bossBarId,
      Action = action
    };
  }

  public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
  {
    buffer.WriteUuid(BossBarId);
    buffer.WriteVarInt(Action.ActionId);
    Action.Write(ref buffer);
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
