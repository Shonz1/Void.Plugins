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
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Network;

namespace Menus.Services;

public class MenuService(ILogger<MenuService> logger, ICommandService commandService) : IEventListener
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

    await UpdateSlotsAsync(player, menu, cancellationToken);
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
  }

  public async ValueTask UpdateSlotsAsync(IMinecraftPlayer player, Menu menu,
    CancellationToken cancellationToken = default)
  {
    for (var i = 0; i < menu.Size + 36; i++)
    {
      if (!menu.ItemsMap.TryGetValue(i, out var item))
        await UpdateSlotAsync(player, menu, i, cancellationToken);
      else
        await UpdateSlotAsync(player, menu, i, cancellationToken);
    }


    foreach (var slot in menu.ItemsMap.Keys)
      await UpdateSlotAsync(player, menu, slot, cancellationToken);
  }

  public async ValueTask UpdateSlotAsync(IMinecraftPlayer player, Menu menu, int slot,
    CancellationToken cancellationToken = default)
  {
    var isPlayerInventory = menu.Type == "minecraft:inventory";

    ItemStack? itemStack = null;

    if (menu.ItemsMap.TryGetValue(slot, out var item))
    {
      var components = new List<IItemComponent>();

      if (item.Title is not null)
        components.Add(new CustomNameItemComponent { Value = item.Title });

      if (item.Lore is not null)
        components.Add(new LoreItemComponent { Value = item.Lore });

      if (item.Profile is not null)
        components.Add(new ProfileItemComponent { Value = item.Profile });

      itemStack = new ItemStack
      {
        Identifier = item.Identifier,
        Count = item.Count,
        Components = components
      };
    }

    logger.LogTrace(
      $"Setting slot {slot} in container {(isPlayerInventory ? 0 : 1)} to {itemStack?.Identifier ?? "air"} for {player.Profile?.Username ?? "unknown"}");
    var setSlotPacket = new SetContainerSlotClientboundPacket
    {
      ContainerId = isPlayerInventory ? 0 : 1,
      Slot = slot,
      ItemStack = itemStack
    };

    await player.SendPacketAsync(setSlotPacket, cancellationToken);
  }

  public async ValueTask ClickSlotAsync(IMinecraftPlayer player, Menu menu, int slot, ClickType clickType,
    CancellationToken cancellationToken = default)
  {
    MenuItem? item = null;

    if (slot > menu.Size)
    {
      var inventoryMenu = menus.Values.FirstOrDefault(m => m.Type == "minecraft:inventory");
      if (inventoryMenu is not null)
      {
        var inventorySlot = slot - menu.Size + inventoryMenu.Size;
        item = inventoryMenu.ItemsMap.GetValueOrDefault(inventorySlot);
      }
    }
    else
      item = menu.ItemsMap.GetValueOrDefault(slot);

    if (item is null)
      return;

    await CloseMenuAsync(player, cancellationToken);

    switch (clickType)
    {
      case ClickType.RightMouseButton when item.RightClickCommand is not null:
        await commandService.ExecuteAsync(player, item.RightClickCommand, cancellationToken);
        break;
      case ClickType.LeftMouseButton:
        await commandService.ExecuteAsync(player, item.LeftClickCommand, cancellationToken);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(clickType), clickType, null);
    }
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

      if (containerServerboundPacket.Mode > 0)
      {
        await UpdateSlotAsync(player, playerMenu, containerServerboundPacket.Slot, cancellationToken);
        return;
      }

      await ClickSlotAsync(player, playerMenu, containerServerboundPacket.Slot, (ClickType) containerServerboundPacket.Button, cancellationToken);
    }
  }

  public void RegisterMenu(Menu menu)
  {
    menus[menu.Name] = menu;
  }
}
