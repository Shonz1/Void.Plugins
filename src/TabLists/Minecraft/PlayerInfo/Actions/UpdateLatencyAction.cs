using Void.Minecraft.Buffers;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public record UpdateLatencyAction(int Ping) : IPlayerInfoAction<UpdateLatencyAction>
{
  public int ActionId => 0x10;

  public static UpdateLatencyAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateLatencyAction(buffer.ReadVarInt());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteVarInt(Ping);
  }
}
