using Void.Minecraft.Buffers;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public record UpdateHatAction(bool IsVisible) : IPlayerInfoAction<UpdateHatAction>
{
  public int ActionId => 0x80;

  public static UpdateHatAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateHatAction(buffer.ReadBoolean());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteBoolean(IsVisible);
  }
}
