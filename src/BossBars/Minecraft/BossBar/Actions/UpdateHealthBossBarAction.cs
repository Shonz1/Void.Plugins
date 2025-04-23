using Void.Minecraft.Buffers;

namespace BossBars.Minecraft.BossBar.Actions;

public record UpdateHealthBossBarAction(float Health) : IBossBarAction<UpdateHealthBossBarAction>
{
  public int ActionId => 0x02;

  public static UpdateHealthBossBarAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateHealthBossBarAction(buffer.ReadFloat());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteFloat(Health);
  }
}
