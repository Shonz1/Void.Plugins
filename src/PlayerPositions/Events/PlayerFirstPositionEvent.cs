using PlayerPositions.Api;
using Void.Common.Events;
using Void.Minecraft.Players;

namespace PlayerPositions.Events;

public record PlayerFirstPositionEvent(IMinecraftPlayer Player, Position Position, Position? PrevPosition) : IEvent;
