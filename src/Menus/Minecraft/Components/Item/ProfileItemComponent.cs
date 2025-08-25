using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;

namespace Menus.Minecraft.Components.Item;

public class ProfileItemComponent : IItemComponent<ProfileItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings = new()
  {
    { ProtocolVersion.MINECRAFT_1_20_5, 0x2E },
    { ProtocolVersion.MINECRAFT_1_21, 0x2F },
    { ProtocolVersion.MINECRAFT_1_21_2, 0x39 },
    { ProtocolVersion.MINECRAFT_1_21_4, 0x39 },
    { ProtocolVersion.MINECRAFT_1_21_5, 0x3D },
    { ProtocolVersion.MINECRAFT_1_21_6, 0x3D },
    { ProtocolVersion.MINECRAFT_1_21_7, 0x3D }
  };

  public required GameProfile Value { get; set; }

  public static int GetId(ProtocolVersion protocolVersion) => Mappings[protocolVersion];

  public static ProfileItemComponent Read(ref MinecraftBuffer buffer)
  {
    var name = "";
    if (buffer.ReadBoolean())
      name = buffer.ReadString();

    var uuid = default(Uuid);
    if (buffer.ReadBoolean())
      uuid = buffer.ReadUuid();

    var properties = buffer.ReadPropertyArray();

    return new ProfileItemComponent
    {
      Value = new GameProfile(name, uuid, properties)
    };
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    var hasName = !string.IsNullOrWhiteSpace(Value.Username);
    buffer.WriteBoolean(hasName);
    if (hasName)
      buffer.WriteString(Value.Username);

    var hasUuid = Value.Id != default;
    buffer.WriteBoolean(hasUuid);
    if (hasUuid)
      buffer.WriteUuid(Value.Id);

    buffer.WritePropertyArray(Value.Properties);
  }
}
