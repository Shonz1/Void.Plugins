using Void.Data.Api.Minecraft;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;

namespace Menus.Minecraft.Components.Item;

public enum Kind
{
  Partial = 0,
  Complete = 1
}

public enum Model
{
  Wide = 0,
  Slim = 1
}

public class ResolvableProfileItemComponent : IItemComponent<ResolvableProfileItemComponent>
{
  private static readonly Dictionary<ProtocolVersion, int> Mappings = new()
  {
    { ProtocolVersion.MINECRAFT_1_21_9, 0x3D },
    { ProtocolVersion.MINECRAFT_1_21_11, 0x44 }
  };

  public required Kind Kind { get; set; }
  public required GameProfile Profile { get; set; }
  public Identifier? Body { get; set; }
  public Identifier? Cape { get; set; }
  public Identifier? Elytra { get; set; }
  public Model? Model { get; set; }

  public static int GetId(ProtocolVersion protocolVersion) => Mappings[protocolVersion];

  public static ResolvableProfileItemComponent Read(ref MinecraftBuffer buffer)
  {
    var kind = (Kind) buffer.ReadVarInt();

    var name = "";
    var uuid = default(Uuid);

    switch (kind)
    {
      case Kind.Partial:
      {
        if (buffer.ReadBoolean())
          name = buffer.ReadString();

        if (buffer.ReadBoolean())
          uuid = buffer.ReadUuid();

        break;
      }
      case Kind.Complete:
      {
        uuid = buffer.ReadUuid();
        name = buffer.ReadString();
        break;
      }
      default:
        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
    }

    var properties = buffer.ReadPropertyArray();

    return new ResolvableProfileItemComponent
    {
      Kind = kind,
      Profile = new GameProfile(name, uuid, properties),
      Body = buffer.ReadVarInt() is 0 ? null : Identifier.FromString(buffer.ReadString()),
      Cape = buffer.ReadVarInt() is 0 ? null : Identifier.FromString(buffer.ReadString()),
      Elytra = buffer.ReadVarInt() is 0 ? null : Identifier.FromString(buffer.ReadString()),
      Model = buffer.ReadVarInt() is 0 ? null : buffer.ReadVarInt() as Model?,
    };
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteVarInt((int) Kind);

    switch (Kind)
    {
      case Kind.Partial:
      {
        var hasName = !string.IsNullOrWhiteSpace(Profile.Username);
        buffer.WriteBoolean(hasName);
        if (hasName)
          buffer.WriteString(Profile.Username);

        var hasUuid = Profile.Id != default;
        buffer.WriteBoolean(hasUuid);
        if (hasUuid)
          buffer.WriteUuid(Profile.Id);

        break;
      }

      case Kind.Complete:
      {
        buffer.WriteUuid(Profile.Id);
        buffer.WriteString(Profile.Username);
        break;
      }
    }

    buffer.WritePropertyArray(Profile.Properties);

    var hasBody = Body is not null;
    buffer.WriteBoolean(hasBody);
    if (hasBody)
      buffer.WriteString(Body!);

    var hasCape = Cape is not null;
    buffer.WriteBoolean(hasCape);
    if (hasCape)
      buffer.WriteString(Cape!);

    var hasElytra = Elytra is not null;
    buffer.WriteBoolean(hasElytra);
    if (hasElytra)
      buffer.WriteString(Elytra!);

    var hasModel = Model is not null;
    buffer.WriteBoolean(hasModel);
    if (hasModel)
      buffer.WriteVarInt((int) Model!);
  }
}
