using Microsoft.Extensions.Logging;
using PlayerPositions.Protocol.Packets.Serverbound;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Players.Contexts;

namespace PlayerPositions.Services;

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
    playerContext.Player.RegisterPacket<SetPlayerPositionServerboundPacket>(SetPlayerPositionServerboundPacket.Mappings);
    playerContext.Player.RegisterPacket<SetPlayerRotationServerboundPacket>(SetPlayerRotationServerboundPacket.Mappings);
    playerContext.Player.RegisterPacket<SetPlayerPositionAndRotationServerboundPacket>(SetPlayerPositionAndRotationServerboundPacket.Mappings);

    logger.LogInformation($"Registered play packets for {PlayerExtensions.get_Profile(playerContext.Player)?.Username ?? "unknown"}");
  }
}
