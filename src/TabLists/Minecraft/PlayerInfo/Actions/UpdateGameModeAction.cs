using Void.Minecraft.Buffers;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public record UpdateGameModeAction(int GameMode) : IPlayerInfoAction<UpdateGameModeAction>
{
  public int ActionId => 0x04;

  public static UpdateGameModeAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateGameModeAction(buffer.ReadVarInt());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteVarInt(GameMode);
  }
}
