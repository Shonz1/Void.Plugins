using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Network;

namespace Menus.Minecraft.Registry;

public class MinecraftMenu
{
  [JsonPropertyName("protocol_id")]
  public int ProtocolId { get; init; }

  [JsonPropertyName("legacy_id")]
  public string? LegacyId { get; init; }

  [JsonPropertyName("slot_count")]
  public int SlotCount { get; init; }
}

public class MinecraftMenuRegistry
{
  private static readonly Identifier inventory = "minecraft:inventory";
  private static readonly string container = "minecraft:container";

  public static int GetId(ProtocolVersion protocolVersion, Identifier identifier)
  {
    var assembly = typeof(MenusPlugin).Assembly;
    var versionName = protocolVersion.GetVersionIntroducedIn();

    using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources._{versionName.Replace(".", "._")}.registries.json");
    if (stream == null)
      return -1;

    var registry = JsonSerializer.Deserialize<MinecraftRegistry>(stream);
    if (registry == null)
      return -1;

    var identifierString = identifier.ToString();

    if (registry.MinecraftMenuRegistry.Entries.ContainsKey(identifierString))
      return registry.MinecraftMenuRegistry.Entries[identifierString].ProtocolId;

    return -1;
  }

  public static string GetLegacyId(ProtocolVersion protocolVersion, Identifier identifier)
  {
    var assembly = typeof(MenusPlugin).Assembly;
    var versionName = protocolVersion.GetVersionIntroducedIn();

    using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources._{versionName.Replace(".", "._")}.registries.json");
    if (stream == null)
      return container;

    var registry = JsonSerializer.Deserialize<MinecraftRegistry>(stream);
    if (registry == null)
      return container;

    var identifierString = identifier.ToString();

    if (registry.MinecraftMenuRegistry.Entries.ContainsKey(identifierString))
      return registry.MinecraftMenuRegistry.Entries[identifierString].LegacyId ?? container;

    return container;
  }

  public static int GetSlotCount(ProtocolVersion protocolVersion, Identifier identifier)
  {
    var assembly = typeof(MenusPlugin).Assembly;
    var versionName = protocolVersion.GetVersionIntroducedIn();

    using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources._{versionName.Replace(".", "._")}.registries.json");
    if (stream == null)
      return 0;

    var registry = JsonSerializer.Deserialize<MinecraftRegistry>(stream);
    if (registry == null)
      return 0;

    var identifierString = identifier.ToString();

    if (registry.MinecraftMenuRegistry.Entries.ContainsKey(identifierString))
      return registry.MinecraftMenuRegistry.Entries[identifierString].SlotCount;

    return 0;
  }

  public static Identifier GetIdentifier(ProtocolVersion protocolVersion, int id, int slotCount = 0)
  {
    var assembly = typeof(MenusPlugin).Assembly;
    var versionName = protocolVersion.GetVersionIntroducedIn();

    using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources._{versionName.Replace(".", "._")}.registries.json");
    if (stream == null)
      return inventory;

    var registry = JsonSerializer.Deserialize<MinecraftRegistry>(stream);
    if (registry == null)
      return inventory;

    foreach (var entry in registry.MinecraftMenuRegistry.Entries)
    {
      if (entry.Value.ProtocolId == id && entry.Value.SlotCount == slotCount)
        return entry.Key;
    }

    return inventory;
  }

  public static Identifier GetIdentifier(ProtocolVersion protocolVersion, string id, int slotCount)
  {
    var assembly = typeof(MenusPlugin).Assembly;
    var versionName = protocolVersion.GetVersionIntroducedIn();

    using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources._{versionName.Replace(".", "._")}.registries.json");
    if (stream == null)
      return inventory;

    var registry = JsonSerializer.Deserialize<MinecraftRegistry>(stream);
    if (registry == null)
      return inventory;

    foreach (var entry in registry.MinecraftMenuRegistry.Entries)
    {
      if (entry.Value.LegacyId == id && entry.Value.SlotCount == slotCount)
        return entry.Key;
    }

    return inventory;
  }

  [JsonPropertyName("entries")]
  public required Dictionary<string, MinecraftMenu> Entries { get; init; }
}
