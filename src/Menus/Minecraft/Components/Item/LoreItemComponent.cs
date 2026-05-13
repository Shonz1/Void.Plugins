using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Components.Item;

public class LoreItemComponent : IItemComponent<LoreItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings =
    new Dictionary<ProtocolVersion, int> { { ProtocolVersion.MINECRAFT_1_20_5, 0x07 } }
      .Concat(
        ProtocolVersion
          .Range(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.Latest)
          .Select(i => new KeyValuePair<ProtocolVersion, int>(i, MinecraftDataComponentTypeRegistry.GetId(i, "minecraft:lore")))
      )
      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

  public required List<Component> Value { get; set; }

  public static int GetId(ProtocolVersion protocolVersion) => Mappings[protocolVersion];

  public static LoreItemComponent Read(ref MinecraftBuffer buffer)
  {
    var size = buffer.ReadVarInt();
    var list = new List<Component>(size);

    for (var i = 0; i < size; i++)
      list.Add(buffer.ReadComponent());

    return new LoreItemComponent
    {
      Value = list
    };
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteVarInt(Value.Count);

    foreach (var component in Value)
      buffer.WriteComponent(component);
  }
}
