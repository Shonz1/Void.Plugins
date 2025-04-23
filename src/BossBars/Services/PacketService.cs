using BossBars.Protocol.Packets.Clientbound;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;

namespace BossBars.Services;

internal class PacketService(ILogger<PacketService> logger) : IEventListener
{
  [Subscribe]
  private void OnPhaseChanged(PhaseChangedEvent @event)
  {
    var player = @event.Player;

    var handler = @event.Phase switch
    {
      Phase.Play => RegisterPlayPackets,
      _ => null as Action<IMinecraftPlayer>
    };

    handler?.Invoke(player);
  }

  private void RegisterPlayPackets(IMinecraftPlayer player)
  {
    player.RegisterPacket<BossBarClientboundPacket>(BossBarClientboundPacket.Mappings);

    logger.LogInformation($"Registered play packets for {player.Profile?.Username ?? "unknown"}");
  }
}
