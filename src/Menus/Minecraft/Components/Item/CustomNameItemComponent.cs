using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Components.Item;

public class CustomNameItemComponent: IItemComponent<CustomNameItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings = new()
  {
    { ProtocolVersion.MINECRAFT_1_20_5, 0x05 },
    { ProtocolVersion.MINECRAFT_1_21, 0x05 },
    { ProtocolVersion.MINECRAFT_1_21_2, 0x05 },
    { ProtocolVersion.MINECRAFT_1_21_4, 0x05 },
    { ProtocolVersion.MINECRAFT_1_21_5, 0x05 },
  };

  public static int GetId(ProtocolVersion protocolVersion) => Mappings[protocolVersion];

  public required Component Value { get; set; }

  public static CustomNameItemComponent Read(ref MinecraftBuffer buffer)
  {
    return new CustomNameItemComponent
    {
      Value = buffer.ReadComponent(ProtocolVersion.Latest)
    };
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteComponent(Value, ProtocolVersion.Latest);
  }
}
