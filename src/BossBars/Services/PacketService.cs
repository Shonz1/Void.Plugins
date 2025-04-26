using BossBars.Protocol.Packets.Clientbound;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Players.Contexts;

namespace BossBars.Services;

internal class PacketService(ILogger<PacketService> logger, IPlayerContext playerContext) : IEventListener
{
  [Subscribe]
  private void OnPhaseChanged(PhaseChangedEvent @event)
  {
    var handler = @event.Phase switch
    {
      Phase.Play => RegisterPlayPackets,
      _ => null as Action
    };

    handler?.Invoke();
  }

  private void RegisterPlayPackets()
  {
    var player = playerContext.Player.AsMinecraftPlayer();

    player.RegisterPacket<BossBarClientboundPacket>(BossBarClientboundPacket.Mappings);

    logger.LogInformation($"Registered play packets for {player.Profile?.Username ?? "unknown"}");
  }
}
