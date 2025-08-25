using Void.Minecraft.Buffers;
using Void.Minecraft.Nbt;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Components.Item;

public class CustomDataItemComponent : IItemComponent<CustomDataItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings = new()
  {
    { ProtocolVersion.MINECRAFT_1_20_5, 0x00 },
    { ProtocolVersion.MINECRAFT_1_21, 0x00 },
    { ProtocolVersion.MINECRAFT_1_21_2, 0x00 },
    { ProtocolVersion.MINECRAFT_1_21_4, 0x00 },
    { ProtocolVersion.MINECRAFT_1_21_5, 0x00 },
    { ProtocolVersion.MINECRAFT_1_21_6, 0x00 },
    { ProtocolVersion.MINECRAFT_1_21_7, 0x00 }
  };

  public static int GetId(ProtocolVersion protocolVersion) => Mappings[protocolVersion];

  public required NbtTag Value { get; set; }

  public static CustomDataItemComponent Read(ref MinecraftBuffer buffer)
  {
    return new CustomDataItemComponent
    {
      Value = buffer.ReadTag()
    };
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteTag(Value);
  }
}
