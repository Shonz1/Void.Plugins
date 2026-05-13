using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Components.Item;

public class CustomNameItemComponent: IItemComponent<CustomNameItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings =
    new Dictionary<ProtocolVersion, int> { { ProtocolVersion.MINECRAFT_1_20_5, 0x05 } }
      .Concat(
        ProtocolVersion
          .Range(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.Latest)
          .Select(i => new KeyValuePair<ProtocolVersion, int>(i, MinecraftDataComponentTypeRegistry.GetId(i, "minecraft:custom_name")))
      )
      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

  public static int GetId(ProtocolVersion protocolVersion) => Mappings[protocolVersion];

  public required Component Value { get; set; }

  public static CustomNameItemComponent Read(ref MinecraftBuffer buffer)
  {
    return new CustomNameItemComponent
    {
      Value = buffer.ReadComponent()
    };
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteComponent(Value);
  }
}
