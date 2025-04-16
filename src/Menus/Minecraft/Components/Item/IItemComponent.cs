using Void.Minecraft.Buffers;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Components.Item;

public interface IItemComponent
{
  public void Write(ref MinecraftBuffer buffer);
}

public interface IItemComponent<out TItemComponent> : IItemComponent where TItemComponent : IItemComponent
{
  public static abstract int GetId(ProtocolVersion protocolVersion);
  public static abstract TItemComponent Read(ref MinecraftBuffer buffer);
}
