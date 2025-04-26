using PlayerPositions.Api;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Players;

namespace PlayerPositions.Events;

public record PlayerFirstPositionEvent(IPlayer Player, Position Position, Position? PrevPosition) : IScopedEvent;
