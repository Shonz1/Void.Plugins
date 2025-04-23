using Void.Minecraft.Components.Text;
using Void.Minecraft.Profiles;

namespace BossBars.Api;

public record BossBar(
  Uuid Id,
  Component Title,
  float Health,
  BossBarColor Color = BossBarColor.Pink,
  BossBarDivision Division = BossBarDivision.NoDivision,
  BossBarFlag Flags = 0);
