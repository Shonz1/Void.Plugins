using Microsoft.Extensions.Logging;
using PlayerPositions.Protocol.Packets.Serverbound;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;

namespace PlayerPositions.Services;

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
    player.RegisterPacket<SetPlayerPositionServerboundPacket>(SetPlayerPositionServerboundPacket.Mappings);
    player.RegisterPacket<SetPlayerRotationServerboundPacket>(SetPlayerRotationServerboundPacket.Mappings);
    player.RegisterPacket<SetPlayerPositionAndRotationServerboundPacket>(SetPlayerPositionAndRotationServerboundPacket.Mappings);

    logger.LogInformation($"Registered play packets for {player.Profile?.Username ?? "unknown"}");
  }
}
