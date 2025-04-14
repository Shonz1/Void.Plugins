using Menu.Minecraft.Components.Item;

namespace Menu.Minecraft;

public class ItemStack
{
  public required Identifier Identifier { get; set; }
  public int Count { get; set; } = 1;
  public List<IItemComponent> Components { get; set; } = new();
}
