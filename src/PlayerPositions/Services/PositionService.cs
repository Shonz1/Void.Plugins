using System.Diagnostics.CodeAnalysis;
using PlayerPositions.Api;
using PlayerPositions.Events;
using PlayerPositions.Protocol.Packets.Serverbound;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;

namespace PlayerPositions.Services;

public class PositionService(IEventService eventService) : IEventListener
{
  private readonly Dictionary<IMinecraftPlayer, Position> positionHolders = new();

  public bool HasPosition(IMinecraftPlayer player)
  {
    return positionHolders.ContainsKey(player);
  }

  public bool TryGetPosition(IMinecraftPlayer player, [MaybeNullWhen(false)] out Position position)
  {
    return positionHolders.TryGetValue(player, out position);
  }

  [Subscribe]
  private void OnLinkStopped(LinkStoppedEvent @event)
  {
    if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
      return;

    positionHolders.Remove(player);
  }

  [Subscribe]
  private async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
  {
    if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
      return;

    if (@event.Message is SetPlayerPositionServerboundPacket or SetPlayerRotationServerboundPacket
        or SetPlayerPositionAndRotationServerboundPacket)
    {
      var isFirstPosition = !HasPosition(player);

      if (isFirstPosition)
        positionHolders.Add(player, new Position());

      if (!TryGetPosition(player, out var position))
        return;

      var prevPosition = new Position
      {
        X = position.X,
        Y = position.Y,
        Z = position.Z,
        Yaw = position.Yaw,
        Pitch = position.Pitch,
        Flags = position.Flags
      };

      switch (@event.Message)
      {
        case SetPlayerPositionServerboundPacket positionPacket:
          position.X = positionPacket.X;
          position.Y = positionPacket.Y;
          position.Z = positionPacket.Z;
          position.Flags = positionPacket.Flags;
          break;
        case SetPlayerRotationServerboundPacket rotationPacket:
          position.Yaw = rotationPacket.Yaw;
          position.Pitch = rotationPacket.Pitch;
          position.Flags = rotationPacket.Flags;
          break;
        case SetPlayerPositionAndRotationServerboundPacket positionAndRotationPacket:
          position.X = positionAndRotationPacket.X;
          position.Y = positionAndRotationPacket.Y;
          position.Z = positionAndRotationPacket.Z;
          position.Yaw = positionAndRotationPacket.Yaw;
          position.Pitch = positionAndRotationPacket.Pitch;
          position.Flags = positionAndRotationPacket.Flags;
          position.Flags = positionAndRotationPacket.Flags;
          break;
      }

      if (isFirstPosition)
        await eventService.ThrowAsync(new PlayerFirstPositionEvent(player, position, prevPosition), cancellationToken);

      await eventService.ThrowAsync(new PlayerPositionUpdateEvent(player, position, prevPosition), cancellationToken);
    }
  }
}
