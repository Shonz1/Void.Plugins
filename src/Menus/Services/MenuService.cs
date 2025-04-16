using Menus.Api;
using Menus.Minecraft;
using Menus.Minecraft.Components.Item;
using Menus.Minecraft.Registry;
using Menus.Protocol.Packets.Clientbound;
using Menus.Protocol.Packets.Serverbound;
using Microsoft.Extensions.Logging;
using Void.Common.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Network;

namespace Menus.Services;

public class MenuService(ILogger<MenuService> logger) : IEventListener
{
  private readonly Dictionary<string, Menu> menus = new();
  private readonly Dictionary<IMinecraftPlayer, Menu> menuHolders = new();

  public Menu? FindMenu(string name)
  {
    return menus.GetValueOrDefault(name);
  }

  public Menu? FindPlayerMenu(IMinecraftPlayer player)
  {
    return menuHolders.GetValueOrDefault(player);
  }

  public async ValueTask OpenMenuAsync(IMinecraftPlayer player, Menu menu, CancellationToken cancellationToken = default)
  {
    var isPlayerInventory = menu.Type == "minecraft:inventory";

    if (!isPlayerInventory)
    {
      menuHolders[player] = menu;

      logger.LogTrace($"Opening menu {menu.Name} for {player.Profile?.Username ?? "unknown"}");
      var openContainerPacket = new OpenContainerClientboundPacket
      {
        ContainerId = 1,
        Type = MinecraftMenuRegistry.GetId(ProtocolVersion.Latest, menu.Type),
        Title = menu.Title
      };
      await player.SendPacketAsync(openContainerPacket, cancellationToken);
    }

    foreach (var (slot, item) in menu.ItemsMap)
    {
      var components = new List<IItemComponent>();

      if (item.Title is not null)
        components.Add(new CustomNameItemComponent { Value = item.Title });

      if (item.Lore is not null)
        components.Add(new LoreItemComponent { Value = item.Lore });

      if (item.Profile is not null)
        components.Add(new ProfileItemComponent { Value = item.Profile });

      var itemStack = new ItemStack
      {
        Identifier = item.Identifier,
        Count = item.Count,
        Components = components
      };

      logger.LogTrace($"Setting slot {slot} in container {(isPlayerInventory ? 0 : 1)} to {item.Identifier} for {player.Profile?.Username ?? "unknown"}");
      var setSlotPacket = new SetContainerSlotClientboundPacket
      {
        ContainerId = isPlayerInventory ? 0 : 1,
        Slot = slot,
        ItemStack = itemStack
      };

      await player.SendPacketAsync(setSlotPacket, cancellationToken);
    }
  }

  public async ValueTask CloseMenuAsync(IMinecraftPlayer player, CancellationToken cancellationToken = default)
  {
    var playerMenu = FindPlayerMenu(player);
    if (playerMenu is null)
      return;

    menuHolders.Remove(player);

    var closeContainerPacket = new CloseContainerClientboundPacket
    {
      ContainerId = 1
    };

    await player.SendPacketAsync(closeContainerPacket, cancellationToken);

    var inventoryMenu = FindMenu("Inventory");
    if (inventoryMenu is not null)
      await OpenMenuAsync(player, inventoryMenu, cancellationToken);
  }

  [Subscribe]
  private void OnLinkStopped(LinkStoppedEvent @event)
  {
    if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
      return;

    menuHolders.Remove(player);
  }

  [Subscribe]
  private async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
  {
    if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
      return;

    if (@event.Message is CloseContainerClientboundPacket or CloseContainerServerboundPacket
        or OpenContainerClientboundPacket)
    {
      menuHolders.Remove(player);
    }

    if (@event.Message is ClickContainerServerboundPacket containerServerboundPacket)
    {
      var playerMenu = FindPlayerMenu(player);
      if (playerMenu is null)
        return;

      @event.Cancel();

      MenuItem? item = null;

      if (containerServerboundPacket.Slot > playerMenu.Size)
      {
        var inventoryMenu = FindMenu("Inventory");
        if (inventoryMenu is not null)
        {
          var inventorySlot = containerServerboundPacket.Slot - playerMenu.Size + inventoryMenu.Size;
          item = inventoryMenu.ItemsMap.GetValueOrDefault(inventorySlot);
        }
      }
      else
        item = playerMenu.ItemsMap.GetValueOrDefault(containerServerboundPacket.Slot);

      if (item is null)
        return;

      await player.SendChatMessageAsync(item.Command, cancellationToken);
      await CloseMenuAsync(player, cancellationToken);
    }
  }

  public void RegisterMenu(Menu menu)
  {
    menus[menu.Name] = menu;
  }
}
