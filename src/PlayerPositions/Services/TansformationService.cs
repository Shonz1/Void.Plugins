using Microsoft.Extensions.Logging;
using PlayerPositions.Protocol.Packets.Serverbound;
using PlayerPositions.Protocol.Transformations;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Players.Contexts;

namespace PlayerPositions.Services;

internal delegate void Transformer(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion);

internal class TransformationService(
  ILogger<TransformationService> logger,
  IPlayerContext playerContext,
  SetPlayerPositionTransformation setPlayerPositionTransformation,
  SetPlayerRotationTransformation setPlayerRotationTransformation,
  SetPlayerPositionAndRotationTransformation setPlayerPositionAndRotationTransformation
) : IEventListener
{
  public List<MinecraftPacketTransformationMapping> RepeatForRange(ProtocolVersion startProtocolVersion, ProtocolVersion endProtocolVersion, Transformer transformer)
  {
    var result = new List<MinecraftPacketTransformationMapping>();
    var range = ProtocolVersion.Range(startProtocolVersion, endProtocolVersion).ToArray();

    for (var i = 0; i < range.Length - 1; i++)
    {
      var from = range[i];
      var to = range[i + 1];

      if (from == to)
        continue;

      result.Insert(0, new MinecraftPacketTransformationMapping(from, to, wrapper => transformer(wrapper, from, to)));
    }

    return result;
  }

  [Subscribe]
  private void OnPhaseChanged(PhaseChangedEvent @event)
  {
    var handler = @event.Phase switch
    {
      Phase.Play => RegisterPlayTransformations,
      _ => null as Action
    };

    handler?.Invoke();
  }

  private void RegisterPlayTransformations()
  {
    playerContext.Player.RegisterTransformations<SetPlayerPositionServerboundPacket>([
      ..RepeatForRange(ProtocolVersion.Latest, ProtocolVersion.MINECRAFT_1_21_4, setPlayerPositionTransformation.DowngradeTo_1_7_2)
    ]);

    playerContext.Player.RegisterTransformations<SetPlayerRotationServerboundPacket>([
      ..RepeatForRange(ProtocolVersion.Latest, ProtocolVersion.MINECRAFT_1_21_4, setPlayerRotationTransformation.DowngradeTo_1_7_2)
    ]);

    playerContext.Player.RegisterTransformations<SetPlayerPositionAndRotationServerboundPacket>([
      ..RepeatForRange(ProtocolVersion.Latest, ProtocolVersion.MINECRAFT_1_21_4, setPlayerPositionAndRotationTransformation.DowngradeTo_1_7_2)
    ]);

    logger.LogInformation($"Registered play transformations for {PlayerExtensions.get_Profile(playerContext.Player)?.Username ?? "unknown"}");
  }
}
