using Void.Minecraft.Buffers;

namespace BossBars.Minecraft.BossBar.Actions;

public record RemoveBossBarAction : IBossBarAction<RemoveBossBarAction>
{
  public int ActionId => 0x01;

  public static RemoveBossBarAction Read(ref MinecraftBuffer buffer)
  {
    return new RemoveBossBarAction();
  }

  public void Write(ref MinecraftBuffer buffer)
  {
  }
}
