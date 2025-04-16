using Menus.Minecraft.Components.Item;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Menus.Protocol.Transformations.Properties;

public record DamageItemComponentProperty(DamageItemComponent Value) : IPacketProperty<DamageItemComponentProperty>
{
  public static DamageItemComponentProperty Read(ref MinecraftBuffer buffer)
  {
    return new DamageItemComponentProperty(DamageItemComponent.Read(ref buffer));
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    Value.Write(ref buffer);
  }
}
