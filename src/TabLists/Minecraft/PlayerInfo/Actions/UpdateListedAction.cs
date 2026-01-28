using Void.Minecraft.Buffers;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public record UpdateListedAction(bool IsListed) : IPlayerInfoAction<UpdateListedAction>
{
  public int ActionId => 0x08;

  public static UpdateListedAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateListedAction(buffer.ReadBoolean());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteBoolean(IsListed);
  }
}
