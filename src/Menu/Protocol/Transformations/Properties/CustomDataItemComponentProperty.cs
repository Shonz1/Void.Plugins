using Menu.Minecraft.Components.Item;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Menu.Protocol.Transformations.Properties;

public record CustomDataItemComponentProperty(CustomDataItemComponent Value) : IPacketProperty<CustomDataItemComponentProperty>
{
  public static CustomDataItemComponentProperty Read(ref MinecraftBuffer buffer)
  {
    return new CustomDataItemComponentProperty(CustomDataItemComponent.Read(ref buffer));
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    Value.Write(ref buffer);
  }
}
