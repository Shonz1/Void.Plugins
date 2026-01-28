using Void.Minecraft.Buffers;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public record UpdateListPriorityAction(int Priority) : IPlayerInfoAction<UpdateListPriorityAction>
{
  public int ActionId => 0x40;

  public static UpdateListPriorityAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateListPriorityAction(buffer.ReadVarInt());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteVarInt(Priority);
  }
}
