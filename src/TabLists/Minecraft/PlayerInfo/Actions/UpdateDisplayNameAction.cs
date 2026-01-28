using Void.Minecraft.Buffers;
using Void.Minecraft.Components.Text;

namespace TabLists.Minecraft.PlayerInfo.Actions;

public record UpdateDisplayNameAction(Component? Name) : IPlayerInfoAction<UpdateDisplayNameAction>
{
  public int ActionId => 0x20;

  public static UpdateDisplayNameAction Read(ref MinecraftBuffer buffer)
  {
    return new UpdateDisplayNameAction(buffer.ReadComponent());
  }

  public void Write(ref MinecraftBuffer buffer)
  {
    if (Name is null)
    {
      buffer.WriteBoolean(false);
      return;
    }

    buffer.WriteBoolean(true);
    buffer.WriteComponent(Name);
  }
}
