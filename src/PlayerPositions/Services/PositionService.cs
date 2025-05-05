using System.Diagnostics.CodeAnalysis;
using PlayerPositions.Api;
using PlayerPositions.Events;
using PlayerPositions.Protocol.Packets.Serverbound;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Players.Contexts;

namespace PlayerPositions.Services;

public class PositionService(IPlayerContext playerContext, IEventService eventService) : IEventListener
{
  private Position? position;

  public bool HasPosition()
  {
    return position is not null;
  }

  public bool TryGetPosition([MaybeNullWhen(false)] out Position position)
  {
    position = this.position;
    return HasPosition();
  }

  [Subscribe]
  private async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
  {
    if (@event.Message is SetPlayerPositionServerboundPacket or SetPlayerRotationServerboundPacket
        or SetPlayerPositionAndRotationServerboundPacket)
    {
      var isFirstPosition = !HasPosition();

      position = new Position();

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
        await eventService.ThrowAsync(new PlayerFirstPositionEvent(playerContext.Player, position, prevPosition), cancellationToken);

      await eventService.ThrowAsync(new PlayerPositionUpdateEvent(playerContext.Player, position, prevPosition), cancellationToken);
    }
  }
}
