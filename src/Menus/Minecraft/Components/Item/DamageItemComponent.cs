using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Components.Item;

public class DamageItemComponent : IItemComponent<DamageItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings =
    new Dictionary<ProtocolVersion, int> { { ProtocolVersion.MINECRAFT_1_20_5, 0x03 } }
      .Concat(
        ProtocolVersion
          .Range(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.Latest)
          .Select(i => new KeyValuePair<ProtocolVersion, int>(i, MinecraftDataComponentTypeRegistry.GetId(i, "minecraft:damage")))
      )
      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

  public int Value { get; set; }

  public static int GetId(ProtocolVersion protocolVersion) => Mappings[protocolVersion];

  public static DamageItemComponent Read(ref MinecraftBuffer buffer)
  {
    return new DamageItemComponent
    {
      Value = buffer.ReadVarInt()
    };
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteVarInt(Value);
  }
}
