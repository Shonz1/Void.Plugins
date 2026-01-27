using Menus.Minecraft.Components.Item;
using Void.Data.Api.Minecraft;

namespace Menus.Minecraft;

public class ItemStack
{
  public required Identifier Identifier { get; set; }
  public int Count { get; set; } = 1;
  public List<IItemComponent> Components { get; set; } = new();
}
