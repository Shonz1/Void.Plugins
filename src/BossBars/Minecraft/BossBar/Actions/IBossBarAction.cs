using Void.Minecraft.Buffers;

namespace BossBars.Minecraft.BossBar.Actions;

public interface IBossBarAction
{
  public int ActionId { get; }
  public void Write(ref MinecraftBuffer buffer);
}

public interface IBossBarAction<out TBossBarAction> : IBossBarAction where TBossBarAction : IBossBarAction
{
  public static abstract TBossBarAction Read(ref MinecraftBuffer buffer);
}
