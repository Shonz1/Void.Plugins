using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;

namespace Menus.Minecraft.Components.Item;

public class ProfileItemComponent : IItemComponent<ProfileItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings =
    new Dictionary<ProtocolVersion, int> { { ProtocolVersion.MINECRAFT_1_20_5, 0x2E } }
      .Concat(
        ProtocolVersion
          .Range(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.Latest)
          .Select(i => new KeyValuePair<ProtocolVersion, int>(i, MinecraftDataComponentTypeRegistry.GetId(i, "minecraft:profile")))
      )
      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

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
