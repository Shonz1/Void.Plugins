using Void.Minecraft.Buffers;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public interface IPlayerInfoAction
{
  public int ActionId { get; }
  public void Write(ref MinecraftBuffer buffer);
}

public interface IPlayerInfoAction<out TPlayerInfoAction> : IPlayerInfoAction where TPlayerInfoAction : IPlayerInfoAction
{
  public static abstract TPlayerInfoAction Read(ref MinecraftBuffer buffer);
}
