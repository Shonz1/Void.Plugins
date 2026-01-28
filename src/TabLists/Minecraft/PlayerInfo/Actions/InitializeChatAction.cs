using Void.Minecraft.Buffers;
using Void.Minecraft.Profiles;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public record InitializeChatAction(Uuid? ChatSessionId, IdentifiedKey? IdentifiedKey) : IPlayerInfoAction<InitializeChatAction>
{
  public int ActionId => 0x02;

  public static InitializeChatAction Read(ref MinecraftBuffer buffer)
  {
    if (!buffer.ReadBoolean())
      return new InitializeChatAction(null, null);

    var chatSessionId = buffer.ReadUuid();
    var publicKeyExpiryTime = buffer.ReadLong();
    var encodedPublicKey = buffer.Read(buffer.ReadVarInt());
    var publicKeySignature = buffer.Read(buffer.ReadVarInt());

    return new InitializeChatAction(
      chatSessionId,
      new IdentifiedKey(IdentifiedKeyRevision.LinkedV2Revision, publicKeyExpiryTime, encodedPublicKey.ToArray(), publicKeySignature.ToArray())
    );
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteBoolean(ChatSessionId.HasValue && IdentifiedKey is not null);

    if (!ChatSessionId.HasValue)
      return;

    if (IdentifiedKey is null)
      return;

    buffer.WriteUuid(ChatSessionId.Value);
    buffer.WriteLong(IdentifiedKey.ExpiresAt);
    buffer.Write(IdentifiedKey.PublicKey);
    buffer.Write(IdentifiedKey.Signature);
  }
}
