namespace BossBars.Api;

[Flags]
public enum BossBarFlag
{
  DarkenSky = 1 << 0,
  PlayMusic = 1 << 1,
  CreateFog = 1 << 2,
}
