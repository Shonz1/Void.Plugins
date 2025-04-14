using System.Text.Json.Serialization;

namespace Menu.Minecraft.Registry;

public class MinecraftRegistry
{
  [JsonPropertyName("minecraft:item")]
  public required MinecraftItemRegistry MinecraftItemRegistry { get; init; }

  [JsonPropertyName("minecraft:menu")]
  public required MinecraftMenuRegistry MinecraftMenuRegistry { get; init; }
}
