using Microsoft.Extensions.Logging;
using TabLists.Protocol.Packets.Clientbound;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players.Contexts;

namespace TabLists.Services;

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
    playerContext.Player.RegisterPacket<PlayerInfoUpdateClientboundPacket>(channel, Operation.Read, PlayerInfoUpdateClientboundPacket.Mappings);
    playerContext.Player.RegisterPacket<PlayerInfoRemoveClientboundPacket>(channel, Operation.Read, PlayerInfoRemoveClientboundPacket.Mappings);

    logger.LogInformation($"Registered play packets to server for {PlayerExtensions.get_Profile(playerContext.Player)?.Username ?? "unknown"}");
  }

  private void RegisterPlayForClientPackets(INetworkChannel channel)
  {
    playerContext.Player.RegisterPacket<PlayerInfoUpdateClientboundPacket>(channel, Operation.Write, PlayerInfoUpdateClientboundPacket.Mappings);
    playerContext.Player.RegisterPacket<PlayerInfoRemoveClientboundPacket>(channel, Operation.Write, PlayerInfoRemoveClientboundPacket.Mappings);

    logger.LogInformation($"Registered play packets to client for {PlayerExtensions.get_Profile(playerContext.Player)?.Username ?? "unknown"}");
  }
}
