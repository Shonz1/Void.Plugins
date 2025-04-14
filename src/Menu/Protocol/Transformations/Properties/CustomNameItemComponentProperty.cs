using Menu.Minecraft.Components.Item;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Menu.Protocol.Transformations.Properties;

public record CustomNameItemComponentProperty(CustomNameItemComponent Value) : IPacketProperty<CustomNameItemComponentProperty>
{
  public static CustomNameItemComponentProperty Read(ref MinecraftBuffer buffer)
  {
    return new CustomNameItemComponentProperty(CustomNameItemComponent.Read(ref buffer));
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    Value.Write(ref buffer);
  }
}
