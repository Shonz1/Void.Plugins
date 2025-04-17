using PlayerPositions.Api;
using Void.Common.Events;
using Void.Minecraft.Players;

namespace PlayerPositions.Events;

public record PlayerPositionUpdateEvent(IMinecraftPlayer Player, Position Position, Position? PrevPosition) : IEvent;
