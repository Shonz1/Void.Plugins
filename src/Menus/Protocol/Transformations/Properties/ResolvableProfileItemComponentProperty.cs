using Menus.Minecraft.Components.Item;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Menus.Protocol.Transformations.Properties;

public record ResolvableProfileItemComponentProperty(ResolvableProfileItemComponent Value) : IPacketProperty<ResolvableProfileItemComponentProperty>
{
  public static ResolvableProfileItemComponentProperty Read(ref MinecraftBuffer buffer)
  {
    return new ResolvableProfileItemComponentProperty(ResolvableProfileItemComponent.Read(ref buffer));
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    Value.Write(ref buffer);
  }
}
