using BossBars.Api;
using BossBars.Minecraft.BossBar.Actions;
using BossBars.Protocol.Packets.Clientbound;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;

namespace BossBars.Services;

public class BossBarService : IEventListener
{
  private readonly Dictionary<IMinecraftPlayer, Dictionary<Uuid, BossBar>> bossBarHolders = new();

  public async ValueTask AddBossBar(IMinecraftPlayer player, BossBar bossBar, CancellationToken cancellationToken)
  {
    EnsurePlayerExistsAsHolder(player);

    bossBarHolders[player].Add(bossBar.Id, bossBar);

    await player.SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new AddBossBarAction(bossBar.Title, bossBar.Health, (int) bossBar.Color, (int) bossBar.Division, (int) bossBar.Flags)
    }, cancellationToken);
  }

  public async ValueTask RemoveBossBar(IMinecraftPlayer player, BossBar bossBar, CancellationToken cancellationToken)
  {
    EnsurePlayerExistsAsHolder(player);

    bossBarHolders[player].Remove(bossBar.Id);

    await player.SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new RemoveBossBarAction()
    }, cancellationToken);
  }

  public async ValueTask SetBossBarTitle(IMinecraftPlayer player, BossBar bossBar, Component value, CancellationToken cancellationToken)
  {
    EnsurePlayerExistsAsHolder(player);

    if (!bossBarHolders[player].ContainsKey(bossBar.Id))
      return;

    bossBarHolders[player][bossBar.Id] = bossBar with { Title = value };

    await player.SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new UpdateTitleBossBarAction(value)
    }, cancellationToken);
  }

  public async ValueTask SetBossBarHealth(IMinecraftPlayer player, BossBar bossBar, float value, CancellationToken cancellationToken)
  {
    EnsurePlayerExistsAsHolder(player);

    if (!bossBarHolders[player].ContainsKey(bossBar.Id))
      return;

    bossBarHolders[player][bossBar.Id] = bossBar with { Health = value };

    await player.SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new UpdateHealthBossBarAction(value)
    }, cancellationToken);
  }

  public async ValueTask SetBossBarStyle(IMinecraftPlayer player, BossBar bossBar, BossBarColor colorValue,
    CancellationToken cancellationToken)
  {
    EnsurePlayerExistsAsHolder(player);

    if (!bossBarHolders[player].ContainsKey(bossBar.Id))
      return;

    await SetBossBarStyle(player, bossBar, colorValue, bossBarHolders[player][bossBar.Id].Division, cancellationToken);
  }

  public async ValueTask SetBossBarStyle(IMinecraftPlayer player, BossBar bossBar, BossBarDivision divisionValue,
    CancellationToken cancellationToken)
  {
    EnsurePlayerExistsAsHolder(player);

    if (!bossBarHolders[player].ContainsKey(bossBar.Id))
      return;

    await SetBossBarStyle(player, bossBar, bossBarHolders[player][bossBar.Id].Color, divisionValue, cancellationToken);
  }

  public async ValueTask SetBossBarStyle(IMinecraftPlayer player, BossBar bossBar, BossBarColor colorValue, BossBarDivision divisionValue, CancellationToken cancellationToken)
  {
    EnsurePlayerExistsAsHolder(player);

    if (!bossBarHolders[player].ContainsKey(bossBar.Id))
      return;

    bossBarHolders[player][bossBar.Id] = bossBar with { Color = colorValue, Division = divisionValue};

    await player.SendPacketAsync(new BossBarClientboundPacket
    {
      BossBarId = bossBar.Id,
      Action = new UpdateStyleBossBarAction((int) colorValue, (int) divisionValue)
    }, cancellationToken);
  }

  private void EnsurePlayerExistsAsHolder(IMinecraftPlayer player)
  {
    if (!bossBarHolders.ContainsKey(player))
      bossBarHolders.Add(player, new Dictionary<Uuid, BossBar>());
  }

  [Subscribe]
  private void OnLinkStopped(LinkStoppedEvent @event)
  {
    if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
      return;

    bossBarHolders.Remove(player);
  }
}
