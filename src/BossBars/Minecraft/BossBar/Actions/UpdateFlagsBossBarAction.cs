using Void.Minecraft.Buffers;

namespace BossBars.Minecraft.BossBar.Actions;

public record UpdateFlagsBossBarAction(int Flags) : IBossBarAction<UpdateFlagsBossBarAction>
{
  public int ActionId => 0x05;

  public static UpdateFlagsBossBarAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateFlagsBossBarAction(buffer.ReadUnsignedByte());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteUnsignedByte((byte)Flags);
  }
}
