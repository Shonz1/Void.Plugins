using Menus.Minecraft.Components.Item;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Menus.Protocol.Transformations.Properties;

public record LoreItemComponentProperty(LoreItemComponent Value) : IPacketProperty<LoreItemComponentProperty>
{
  public static LoreItemComponentProperty Read(ref MinecraftBuffer buffer)
  {
    return new LoreItemComponentProperty(LoreItemComponent.Read(ref buffer));
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    Value.Write(ref buffer);
  }
}
