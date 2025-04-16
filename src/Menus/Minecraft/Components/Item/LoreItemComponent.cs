using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Components.Item;

public class LoreItemComponent : IItemComponent<LoreItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings = new()
  {
    { ProtocolVersion.MINECRAFT_1_20_5, 0x07 },
    { ProtocolVersion.MINECRAFT_1_21, 0x07 },
    { ProtocolVersion.MINECRAFT_1_21_2, 0x08 },
    { ProtocolVersion.MINECRAFT_1_21_4, 0x08 },
    { ProtocolVersion.MINECRAFT_1_21_5, 0x08 }
  };

  public required List<Component> Value { get; set; }

  public static int GetId(ProtocolVersion protocolVersion) => Mappings[protocolVersion];

  public static LoreItemComponent Read(ref MinecraftBuffer buffer)
  {
    var size = buffer.ReadVarInt();
    var list = new List<Component>(size);

    for (var i = 0; i < size; i++)
      list.Add(buffer.ReadComponent(ProtocolVersion.Latest));

    return new LoreItemComponent
    {
      Value = list
    };
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteVarInt(Value.Count);

    foreach (var component in Value)
      buffer.WriteComponent(component, ProtocolVersion.Latest);
  }
}
