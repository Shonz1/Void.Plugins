using Void.Minecraft.Buffers;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Components.Item;

public class DamageItemComponent : IItemComponent<DamageItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings = new()
  {
    { ProtocolVersion.MINECRAFT_1_20_5, 0x03 },
    { ProtocolVersion.MINECRAFT_1_21, 0x03 },
    { ProtocolVersion.MINECRAFT_1_21_2, 0x03 },
    { ProtocolVersion.MINECRAFT_1_21_4, 0x03 },
    { ProtocolVersion.MINECRAFT_1_21_5, 0x03 },
    { ProtocolVersion.MINECRAFT_1_21_6, 0x03 },
    { ProtocolVersion.MINECRAFT_1_21_7, 0x03 }
  };

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
