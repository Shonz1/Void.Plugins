using Void.Minecraft.Buffers;
using Void.Minecraft.Profiles;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public record AddPlayerAction(GameProfile gameProfile) : IPlayerInfoAction<AddPlayerAction>
{
  public int ActionId => 0x01;

  public static AddPlayerAction Read(ref MinecraftBuffer buffer)
  {
    return new AddPlayerAction(new GameProfile(buffer.ReadString(), Properties: buffer.ReadPropertyArray()));
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteString(gameProfile.Username);
    buffer.WritePropertyArray(gameProfile.Properties);
  }
}
