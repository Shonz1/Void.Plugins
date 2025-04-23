using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network;

namespace BossBars.Minecraft.BossBar.Actions;

public record UpdateTitleBossBarAction(Component Title) : IBossBarAction<UpdateTitleBossBarAction>
{
  public int ActionId => 0x03;

  public static UpdateTitleBossBarAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateTitleBossBarAction(buffer.ReadComponent(ProtocolVersion.Latest));
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    buffer.WriteComponent(Title, ProtocolVersion.Latest);
  }
}
