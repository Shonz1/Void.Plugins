using Menu.Minecraft;
using Menu.Minecraft.Components.Item;
using Menu.Minecraft.Registry;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;

namespace Menu.Extensions;

public static class MinecraftBufferExtensions
{
  public static void WriteItemStack(this ref MinecraftBuffer buffer, ItemStack? itemStack)
  {
    if (itemStack == null || itemStack.Count == 0)
    {
      buffer.WriteVarInt(0);
      return;
    }

    var itemId = MinecraftItemRegistry.GetId(ProtocolVersion.Latest, itemStack.Identifier);

    buffer.WriteVarInt(itemStack.Count);
    buffer.WriteVarInt(itemId);

    buffer.WriteVarInt(itemStack.Components.Count); // componentsToAddCount
    buffer.WriteVarInt(0); // componentsToRemoveCount

    foreach (var component in itemStack.Components)
    {
      var decodeMethod = component.GetType().GetMethod(nameof(IItemComponent<IItemComponent>.GetId));
      if (decodeMethod is null)
        throw new Exception("Unable to find GetId method.");

      var id = decodeMethod.Invoke(null, [ProtocolVersion.Latest]);
      if (id is null)
        throw new Exception("Unable to find GetId method.");

      buffer.WriteVarInt((int) id);
      component.Write(ref buffer);
    }
  }
}
