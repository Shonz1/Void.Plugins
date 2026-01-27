using Microsoft.Extensions.Logging;
using PlayerPositions.Protocol.Packets.Serverbound;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players.Contexts;

namespace PlayerPositions.Services;

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
    playerContext.Player.RegisterPacket<SetPlayerPositionServerboundPacket>(channel, Operation.Write, SetPlayerPositionServerboundPacket.Mappings);
    playerContext.Player.RegisterPacket<SetPlayerRotationServerboundPacket>(channel, Operation.Write, SetPlayerRotationServerboundPacket.Mappings);
    playerContext.Player.RegisterPacket<SetPlayerPositionAndRotationServerboundPacket>(channel, Operation.Write, SetPlayerPositionAndRotationServerboundPacket.Mappings);

    logger.LogInformation($"Registered play packets to server for {playerContext.Player}");
  }

  private void RegisterPlayForClientPackets(INetworkChannel channel)
  {
    playerContext.Player.RegisterPacket<SetPlayerPositionServerboundPacket>(channel, Operation.Read, SetPlayerPositionServerboundPacket.Mappings);
    playerContext.Player.RegisterPacket<SetPlayerRotationServerboundPacket>(channel, Operation.Read, SetPlayerRotationServerboundPacket.Mappings);
    playerContext.Player.RegisterPacket<SetPlayerPositionAndRotationServerboundPacket>(channel, Operation.Read, SetPlayerPositionAndRotationServerboundPacket.Mappings);

    logger.LogInformation($"Registered play packets to server for {playerContext.Player}");
  }
}
