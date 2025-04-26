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
    var player = playerContext.Player.AsMinecraftPlayer();

    player.RegisterPacket<SetPlayerPositionServerboundPacket>(SetPlayerPositionServerboundPacket.Mappings);
    player.RegisterPacket<SetPlayerRotationServerboundPacket>(SetPlayerRotationServerboundPacket.Mappings);
    player.RegisterPacket<SetPlayerPositionAndRotationServerboundPacket>(SetPlayerPositionAndRotationServerboundPacket.Mappings);

    logger.LogInformation($"Registered play packets for {player.Profile?.Username ?? "unknown"}");
  }
}
