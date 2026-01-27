using BossBars.Protocol.Packets.Clientbound;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players.Contexts;

namespace BossBars.Services;

internal class PacketService(ILogger<PacketService> logger, IPlayerContext playerContext) : IEventListener
{
  [Subscribe]
  private void OnPhaseChanged(PhaseChangedEvent @event)
  {
    Action<INetworkChannel>? handler = @event.Phase switch
    {
      Phase.Play => @event.Side switch {
        Side.Server => RegisterPlayForServerPackets,
        Side.Client => RegisterPlayForClientPackets,
        _ => null
      },
      _ => null
    };

    handler?.Invoke(@event.Channel);
  }

  private void RegisterPlayForServerPackets(INetworkChannel channel)
  {
    playerContext.Player.RegisterPacket<BossBarClientboundPacket>(channel, Operation.Read, BossBarClientboundPacket.Mappings);

    logger.LogInformation($"Registered play packets to server for {playerContext.Player}");
  }

  private void RegisterPlayForClientPackets(INetworkChannel channel)
  {
    playerContext.Player.RegisterPacket<BossBarClientboundPacket>(channel, Operation.Write, BossBarClientboundPacket.Mappings);

    logger.LogInformation($"Registered play packets to client for {playerContext.Player}");
  }
}
