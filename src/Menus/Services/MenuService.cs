using Menus.Api;
using Menus.Minecraft;
using Menus.Minecraft.Components.Item;
using Menus.Protocol.Packets.Clientbound;
using Menus.Protocol.Packets.Serverbound;
using Microsoft.Extensions.Logging;
using Void.Data.Api.Minecraft;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Players.Contexts;

namespace Menus.Services;

public class MenuService(ILogger<MenuService> logger, IPlayerContext playerContext, ICommandService commandService) : IEventListener
{
  private static Menu? defaultInventoryMenu;

  public Menu? InventoryMenu = defaultInventoryMenu;
  public Menu? CurrentMenu;

  public async ValueTask OpenAsync(Menu menu, CancellationToken cancellationToken)
  {
    var player = playerContext.Player;

    var isPlayerInventory = menu.Type == "minecraft:inventory";

    if (!isPlayerInventory)
    {
      CurrentMenu = menu;

      logger.LogTrace($"Opening menu {menu.Name} for {PlayerExtensions.get_Profile(playerContext.Player)?.Username ?? "unknown"}");
      var openContainerPacket = new OpenContainerClientboundPacket
      {
        ContainerId = 1,
        Type = MinecraftMenuRegistry.GetId(ProtocolVersion.Latest, menu.Type),
        Title = menu.Title
      };
      await player.SendPacketAsync(openContainerPacket, cancellationToken);
    }

    await UpdateSlotsAsync(menu, cancellationToken);
  }

  public async ValueTask CloseAsync(CancellationToken cancellationToken)
  {
    if (CurrentMenu is null)
      return;

    CurrentMenu = null;

    var closeContainerPacket = new CloseContainerClientboundPacket
    {
      ContainerId = 1
    };

    await playerContext.Player.SendPacketAsync(closeContainerPacket, cancellationToken);
  }

  public async ValueTask SetProperty(int property, int value, CancellationToken cancellationToken)
  {
    if (CurrentMenu is null)
      return;

    var setContainerPropertyPacket = new SetContainerPropertyClientboundPacket
    {
      ContainerId = 1,
      Property = property,
      Value = value
    };

    await playerContext.Player.SendPacketAsync(setContainerPropertyPacket, cancellationToken);
  }

  public async ValueTask UpdateSlotsAsync(Menu menu, CancellationToken cancellationToken)
  {
    var isInventory = menu.Type == "minecraft:inventory";

    if (!isInventory)
    {
      for (var i = 0; i < menu.Size; i++)
        await UpdateSlotAsync(menu, i, cancellationToken);
    }

    if (InventoryMenu is null)
      return;

    for (var i = 0; i < InventoryMenu.Size + 36; i++)
      await UpdateSlotAsync(InventoryMenu, i, cancellationToken);

  }

  public async ValueTask UpdateSlotAsync(Menu menu, int slot, CancellationToken cancellationToken)
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
        components.Add(new ResolvableProfileItemComponent { Kind = Kind.Partial, Profile = item.Profile });

      itemStack = new ItemStack
      {
        Identifier = item.Identifier,
        Count = item.Count,
        Components = components
      };
    }

    logger.LogTrace(
      $"Setting slot {slot} in container {(isPlayerInventory ? 0 : 1)} to {itemStack?.Identifier ?? "air"} for {PlayerExtensions.get_Profile(playerContext.Player)?.Username ?? "unknown"}");
    var setSlotPacket = new SetContainerSlotClientboundPacket
    {
      ContainerId = isPlayerInventory ? 0 : 1,
      Slot = slot,
      ItemStack = itemStack
    };

    await playerContext.Player.SendPacketAsync(setSlotPacket, cancellationToken);
  }

  public async ValueTask ClickSlotAsync(Menu menu, int slot, ClickType clickType, CancellationToken cancellationToken)
  {
    MenuItem? item = null;

    if (slot > menu.Size)
    {
      if (InventoryMenu is not null)
      {
        var inventorySlot = slot - menu.Size + InventoryMenu.Size;
        item = InventoryMenu.ItemsMap.GetValueOrDefault(inventorySlot);
      }
    }
    else
      item = menu.ItemsMap.GetValueOrDefault(slot);

    if (item is null)
      return;

    switch (clickType)
    {
      case ClickType.RightMouseButton when item.RightClickCommand is not null:
        await commandService.ExecuteAsync(playerContext.Player, item.RightClickCommand, cancellationToken);
        break;
      case ClickType.LeftMouseButton when item.LeftClickCommand is not null:
        await commandService.ExecuteAsync(playerContext.Player, item.LeftClickCommand, cancellationToken);
        break;
    }
  }

  [Subscribe]
  private async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
  {
    if (@event.Message is CloseContainerClientboundPacket or CloseContainerServerboundPacket
        or OpenContainerClientboundPacket)
    {
      CurrentMenu = null;
    }

    if (@event.Message is ClickContainerServerboundPacket containerServerboundPacket)
    {
      if (CurrentMenu is null)
        return;

      @event.Cancel();

      if (containerServerboundPacket.Mode > 0)
      {
        await UpdateSlotAsync(CurrentMenu, containerServerboundPacket.Slot, cancellationToken);
        return;
      }

      await ClickSlotAsync(CurrentMenu, containerServerboundPacket.Slot, (ClickType) containerServerboundPacket.Button, cancellationToken);
    }
  }

  public static void SetDefaultInventoryMenu(Menu menu)
  {
    defaultInventoryMenu = menu;
  }
}
