using Menus.Minecraft.Components.Item;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Menus.Protocol.Transformations.Properties;

public record ProfileItemComponentProperty(ProfileItemComponent Value) : IPacketProperty<ProfileItemComponentProperty>
{
  public static ProfileItemComponentProperty Read(ref MinecraftBuffer buffer)
  {
    return new ProfileItemComponentProperty(ProfileItemComponent.Read(ref buffer));
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    Value.Write(ref buffer);
  }
}
