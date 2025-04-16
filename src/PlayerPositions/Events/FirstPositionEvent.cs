using PlayerPositions.Api;
using Void.Common.Events;

namespace PlayerPositions.Events;

public record FirstPositionEvent(PlayerPosition playerPosition) : IEvent;
