using Void.Minecraft.Buffers;

namespace BossBars.Minecraft.BossBar.Actions;

public record UpdateStyleBossBarAction(int Color, int Dividers) : IBossBarAction<UpdateStyleBossBarAction>
{
  public int ActionId => 0x04;

  public static UpdateStyleBossBarAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateStyleBossBarAction(buffer.ReadVarInt(), buffer.ReadVarInt());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteVarInt(Color);
    buffer.WriteVarInt(Dividers);
  }
}
