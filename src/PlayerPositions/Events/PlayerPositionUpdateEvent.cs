using PlayerPositions.Api;
using Void.Minecraft.Players;
using Void.Proxy.Api.Events;

namespace PlayerPositions.Events;

public record PlayerPositionUpdateEvent(IMinecraftPlayer Player, Position Position, Position? PrevPosition) : IEvent;
