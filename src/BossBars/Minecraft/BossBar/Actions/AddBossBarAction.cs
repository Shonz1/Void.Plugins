using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;

namespace BossBars.Minecraft.BossBar.Actions;

public record AddBossBarAction(Component Title, float Health, int Color, int Division, int Flags) : IBossBarAction<AddBossBarAction>
{
  public int ActionId => 0x00;

  public static AddBossBarAction Read(ref MinecraftBuffer buffer)
  {
    return new AddBossBarAction(
      buffer.ReadComponent(),
      buffer.ReadFloat(),
      buffer.ReadVarInt(),
      buffer.ReadVarInt(),
      buffer.ReadUnsignedByte()
    );
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteComponent(Title);
    buffer.WriteFloat(Health);
    buffer.WriteVarInt(Color);
    buffer.WriteVarInt(Division);
    buffer.WriteUnsignedByte((byte) Flags);
  }
}
