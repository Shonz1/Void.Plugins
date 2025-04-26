using BossBars.Api;
using BossBars.Minecraft.BossBar.Actions;
using BossBars.Protocol.Packets.Clientbound;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Players.Extensions;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Players.Contexts;

namespace BossBars.Services;

public class BossBarService(IPlayerContext playerContext, BossBarsPlugin plugin) : IEventListener
{
  private readonly Dictionary<Uuid, BossBar> bossBars = new();

  public async ValueTask AddAsync(BossBar bossBar, CancellationToken cancellationToken)
  {
    bossBars.Add(bossBar.Id, bossBar);

    await playerContext.Player.AsMinecraftPlayer().SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new AddBossBarAction(bossBar.Title, bossBar.Health, (int) bossBar.Color, (int) bossBar.Division, (int) bossBar.Flags)
    }, cancellationToken);
  }

  public async ValueTask RemoveAsync(BossBar bossBar, CancellationToken cancellationToken)
  {
    bossBars.Remove(bossBar.Id);

    await playerContext.Player.AsMinecraftPlayer().SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new RemoveBossBarAction()
    }, cancellationToken);
  }

  public async ValueTask SetTitleAsync(BossBar bossBar, Component value, CancellationToken cancellationToken)
  {
    if (!bossBars.ContainsKey(bossBar.Id))
      return;

    bossBars[bossBar.Id] = bossBar with { Title = value };

    await playerContext.Player.AsMinecraftPlayer().SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new UpdateTitleBossBarAction(value)
    }, cancellationToken);
  }

  public async ValueTask SetHealthAsync(BossBar bossBar, float value, CancellationToken cancellationToken)
  {
    if (!bossBars.ContainsKey(bossBar.Id))
      return;

    bossBars[bossBar.Id] = bossBar with { Health = value };

    await playerContext.Player.AsMinecraftPlayer().SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new UpdateHealthBossBarAction(value)
    }, cancellationToken);
  }

  public async ValueTask SetStyleAsync(BossBar bossBar, BossBarColor colorValue,
    CancellationToken cancellationToken)
  {
    if (!bossBars.TryGetValue(bossBar.Id, out var bar))
      return;

    await SetStyleAsync(bossBar, colorValue, bar.Division, cancellationToken);
  }

  public async ValueTask SetStyleAsync(BossBar bossBar, BossBarDivision divisionValue,
    CancellationToken cancellationToken)
  {
    if (!bossBars.TryGetValue(bossBar.Id, out var bar))
      return;

    await SetStyleAsync(bossBar, bar.Color, divisionValue, cancellationToken);
  }

  public async ValueTask SetStyleAsync(BossBar bossBar, BossBarColor colorValue, BossBarDivision divisionValue, CancellationToken cancellationToken)
  {
    if (!bossBars.ContainsKey(bossBar.Id))
      return;

    bossBars[bossBar.Id] = bossBar with { Color = colorValue, Division = divisionValue};

    await playerContext.Player.AsMinecraftPlayer().SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new UpdateStyleBossBarAction((int) colorValue, (int) divisionValue)
    }, cancellationToken);
  }

  [Subscribe]
  private async ValueTask OnPluginUnloading(PluginUnloadingEvent @event, CancellationToken cancellationToken)
  {
    if (@event.Plugin != plugin)
      return;

    foreach (var bossBar in bossBars.Values)
      await RemoveAsync(bossBar, cancellationToken);
  }
}
