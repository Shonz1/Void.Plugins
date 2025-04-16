using System.IO.Compression;
using Menus.Minecraft.Components.Item;
using Menus.Minecraft.Registry;
using Menus.Protocol.Transformations.Properties;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Components.Text.Serializers;
using Void.Minecraft.Nbt;
using Void.Minecraft.Nbt.Tags;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Menus.Protocol.Transformations;

public class ItemStackTransformation(ILogger logger)
{
  public void Downgrade(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion fromProtocolVersion,
    ProtocolVersion toProtocolVersion)
  {
    // New versions add to top

    if (toProtocolVersion > ProtocolVersion.MINECRAFT_1_20_3)
      PassthroughComponent(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion > ProtocolVersion.MINECRAFT_1_20_3 && toProtocolVersion <= ProtocolVersion.MINECRAFT_1_20_3)
      FromComponentToNbt(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion > ProtocolVersion.MINECRAFT_1_20 && fromProtocolVersion <= ProtocolVersion.MINECRAFT_1_20_3 && toProtocolVersion > ProtocolVersion.MINECRAFT_1_20)
      PassthroughNbt(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion > ProtocolVersion.MINECRAFT_1_20 && toProtocolVersion <= ProtocolVersion.MINECRAFT_1_20)
      FromNbtToNamedNbt(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion <= ProtocolVersion.MINECRAFT_1_20 && toProtocolVersion > ProtocolVersion.MINECRAFT_1_13_2)
      PassthroughNamedNbt(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion > ProtocolVersion.MINECRAFT_1_13_2 && toProtocolVersion <= ProtocolVersion.MINECRAFT_1_13_2)
      FromJsonLoreToStringLore(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion > ProtocolVersion.MINECRAFT_1_13_1 && toProtocolVersion <= ProtocolVersion.MINECRAFT_1_13_1)
      FromPresendToNegativeId(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion <= ProtocolVersion.MINECRAFT_1_13_1 && toProtocolVersion > ProtocolVersion.MINECRAFT_1_12_2)
      PassthroughNegativeId(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion > ProtocolVersion.MINECRAFT_1_12_2 && toProtocolVersion <= ProtocolVersion.MINECRAFT_1_12_2)
      FromIdToIdWithMeta(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion <= ProtocolVersion.MINECRAFT_1_12_2 && toProtocolVersion > ProtocolVersion.MINECRAFT_1_7_6)
      PassthroughIdWithMeta(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion > ProtocolVersion.MINECRAFT_1_7_6 && toProtocolVersion <= ProtocolVersion.MINECRAFT_1_7_6)
      FromNbtToGzipedNbt(wrapper, fromProtocolVersion, toProtocolVersion);

    if (fromProtocolVersion <= ProtocolVersion.MINECRAFT_1_7_6 && toProtocolVersion >= ProtocolVersion.MINECRAFT_1_7_2)
      PassthroughGzipedNbt(wrapper, fromProtocolVersion, toProtocolVersion);
  }

  public void PassthroughGzipedNbt(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"Passthrough gziped nbt {fromProtocolVersion} -> {toProtocolVersion}");

    var itemId = wrapper.Read<ShortProperty>();
    if (itemId.AsPrimitive == -1)
      return;

    var count = wrapper.Read<ByteProperty>();
    var itemMeta = wrapper.Read<ShortProperty>();

    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive, itemMeta.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    var updatedItemMeta = MinecraftItemRegistry.GetMeta(toProtocolVersion, itemIdentifier);

    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemId));
    wrapper.Write(count);
    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemMeta));

    wrapper.Passthrough<ShortProperty>(); // Nbt size
    wrapper.Passthrough<BinaryProperty>(); // Gziped nbt
  }

  public void FromNbtToGzipedNbt(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"From nbt to gziped nbt {fromProtocolVersion} -> {toProtocolVersion}");

    var itemId = wrapper.Read<ShortProperty>();
    if (itemId.AsPrimitive == -1)
      return;

    var count = wrapper.Read<ByteProperty>();
    var itemMeta = wrapper.Read<ShortProperty>();

    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive, itemMeta.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    var updatedItemMeta = MinecraftItemRegistry.GetMeta(toProtocolVersion, itemIdentifier);

    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemId));
    wrapper.Write(count);
    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemMeta));

    var nbt = wrapper.Read<NamedNbtProperty>();
    var nbtStream = nbt.AsNbtTag.AsStream();
    var nbtBuffer = nbtStream.ToArray();
    var gzipStream = new MemoryStream();

    using (var dstream = new GZipStream(gzipStream, CompressionLevel.Fastest))
      dstream.Write(nbtBuffer, 0, nbtBuffer.Length);

    var binary = BinaryProperty.FromStream(gzipStream);
    wrapper.Write(ShortProperty.FromPrimitive((short) binary.Value.Length));
    wrapper.Write(binary);
  }

  public void PassthroughIdWithMeta(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"Passthrough id with meta {fromProtocolVersion} -> {toProtocolVersion}");

    var itemId = wrapper.Read<ShortProperty>();
    if (itemId.AsPrimitive == -1)
      return;

    var count = wrapper.Read<ByteProperty>();
    var itemMeta = wrapper.Read<ShortProperty>();

    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive, itemMeta.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    var updatedItemMeta = MinecraftItemRegistry.GetMeta(toProtocolVersion, itemIdentifier);

    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemId));
    wrapper.Write(count);
    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemMeta));

    wrapper.Passthrough<NamedNbtProperty>(); // Nbt
  }

  public void FromIdToIdWithMeta(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"From id to id with meta {fromProtocolVersion} -> {toProtocolVersion}");

    var itemId = wrapper.Read<ShortProperty>();
    if (itemId.AsPrimitive == -1)
      return;

    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    var itemMeta = MinecraftItemRegistry.GetMeta(toProtocolVersion, itemIdentifier);
    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemId));

    wrapper.Passthrough<ByteProperty>(); // Count
    wrapper.Write(ShortProperty.FromPrimitive((short) itemMeta)); // Damage

    var nbt = wrapper.Read<NamedNbtProperty>().AsNbtTag;
    if (nbt is NbtCompound compound)
    {
      var display = compound["display"] as NbtCompound;
      if (display != null)
      {
        var name = display["Name"] as NbtString;
        if (name != null)
        {
          var component = ComponentJsonSerializer.Deserialize(name.Value);
          display["Name"] = new NbtString(ComponentLegacySerializer.Serialize(component, '§'));
        }
      }
    }

    wrapper.Write(NamedNbtProperty.FromNbtTag(nbt));
  }

  public void PassthroughNegativeId(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"Passthrough negative id {fromProtocolVersion} -> {toProtocolVersion}");

    var itemId = wrapper.Read<ShortProperty>();
    if (itemId.AsPrimitive == -1)
      return;

    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemId));

    wrapper.Passthrough<ByteProperty>(); // Count
    wrapper.Passthrough<NamedNbtProperty>(); // Nbt
  }

  public void FromPresendToNegativeId(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"From present to negative id {fromProtocolVersion} -> {toProtocolVersion}");

    var isPresent = wrapper.Read<BoolProperty>();
    if (!isPresent.AsPrimitive)
    {
      wrapper.Write(ShortProperty.FromPrimitive(-1));
      return;
    }

    var itemId = wrapper.Read<VarIntProperty>();
    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    wrapper.Write(ShortProperty.FromPrimitive((short) updatedItemId));

    wrapper.Passthrough<ByteProperty>(); // Count
    wrapper.Passthrough<NamedNbtProperty>(); // Nbt
  }

  public void FromJsonLoreToStringLore(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion fromProtocolVersion,
    ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"From json lore to string lore {fromProtocolVersion} -> {toProtocolVersion}");

    var isPresent = wrapper.Passthrough<BoolProperty>();
    if (!isPresent.AsPrimitive)
      return;

    var itemId = wrapper.Read<VarIntProperty>();
    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    wrapper.Write(VarIntProperty.FromPrimitive(updatedItemId));

    wrapper.Passthrough<ByteProperty>(); // Count

    var nbt = wrapper.Read<NamedNbtProperty>().AsNbtTag;
    if (nbt is NbtCompound compound)
    {
      var display = compound["display"] as NbtCompound;
      if (display != null)
      {
        var lore = display["Lore"] as NbtList;
        if (lore != null)
        {
          var loreList = new List<NbtString>();

          foreach (var loreItem in lore.Data)
          {
            var loreEntry = loreItem as NbtString;
            if (loreEntry == null)
              continue;

            var component = ComponentJsonSerializer.Deserialize(loreEntry.Value);
            loreList.Add(new NbtString(ComponentLegacySerializer.Serialize(component, '§')));

            display["Lore"] = new NbtList(loreList, NbtTagType.String);
          }
        }
      }
    }

    wrapper.Write(NamedNbtProperty.FromNbtTag(nbt));
  }

  public void PassthroughNamedNbt(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"Passthrough named nbt {fromProtocolVersion} -> {toProtocolVersion}");

    var isPresent = wrapper.Passthrough<BoolProperty>();
    if (!isPresent.AsPrimitive)
      return;

    var itemId = wrapper.Read<VarIntProperty>();
    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    wrapper.Write(VarIntProperty.FromPrimitive(updatedItemId));

    wrapper.Passthrough<ByteProperty>(); // Count
    wrapper.Passthrough<NamedNbtProperty>(); // Nbt;
  }

  public void FromNbtToNamedNbt(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion fromProtocolVersion,
    ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"From nbt to named nbt {fromProtocolVersion} -> {toProtocolVersion}");

    var isPresent = wrapper.Passthrough<BoolProperty>();
    if (!isPresent.AsPrimitive)
      return;

    var itemId = wrapper.Read<VarIntProperty>();
    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    wrapper.Write(VarIntProperty.FromPrimitive(updatedItemId));

    wrapper.Passthrough<ByteProperty>(); // Count

    var nbt = wrapper.Read<NbtProperty>().AsNbtTag;
    wrapper.Write(NamedNbtProperty.FromNbtTag(nbt));
  }

  public void PassthroughNbt(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion fromProtocolVersion,
    ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"Passthrough nbt {fromProtocolVersion} -> {toProtocolVersion}");

    var isPresent = wrapper.Passthrough<BoolProperty>();
    if (!isPresent.AsPrimitive)
      return;

    var itemId = wrapper.Read<VarIntProperty>();
    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    wrapper.Write(VarIntProperty.FromPrimitive(updatedItemId));

    wrapper.Passthrough<ByteProperty>(); // Count
    wrapper.Passthrough<NbtProperty>(); // Nbt
  }

  public void FromComponentToNbt(IMinecraftBinaryPacketWrapper wrapper, ProtocolVersion fromProtocolVersion,
    ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"From component to nbt {fromProtocolVersion} -> {toProtocolVersion}");

    var count = wrapper.Read<VarIntProperty>();

    if (count.AsPrimitive == 0)
    {
      wrapper.Write(BoolProperty.FromPrimitive(false));
      return;
    }

    var itemId = wrapper.Read<VarIntProperty>();
    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);

    var componentToAddCount = wrapper.Read<VarIntProperty>();
    wrapper.Read<VarIntProperty>();

    var componentsToAdd = new List<IItemComponent>(componentToAddCount.AsPrimitive);
    for (var i = 0; i < componentToAddCount.AsPrimitive; i++)
    {
      var componentId = wrapper.Read<VarIntProperty>();

      if (CustomDataItemComponent.GetId(fromProtocolVersion) == componentId.AsPrimitive)
        componentsToAdd.Add(wrapper.Read<CustomDataItemComponentProperty>().Value);
      else if (CustomNameItemComponent.GetId(fromProtocolVersion) == componentId.AsPrimitive)
        componentsToAdd.Add(wrapper.Read<CustomNameItemComponentProperty>().Value);
      else if (LoreItemComponent.GetId(fromProtocolVersion) == componentId.AsPrimitive)
        componentsToAdd.Add(wrapper.Read<LoreItemComponentProperty>().Value);
      else if (DamageItemComponent.GetId(fromProtocolVersion) == componentId.AsPrimitive)
        componentsToAdd.Add(wrapper.Read<DamageItemComponentProperty>().Value);
      else if (ProfileItemComponent.GetId(fromProtocolVersion) == componentId.AsPrimitive)
        componentsToAdd.Add(wrapper.Read<ProfileItemComponentProperty>().Value);
      else
        logger.LogError($"Unknown item component with id {componentId.AsPrimitive} in protocol {fromProtocolVersion}");
    }

    wrapper.Write(BoolProperty.FromPrimitive(true));
    wrapper.Write(VarIntProperty.FromPrimitive(updatedItemId));
    wrapper.Write(ByteProperty.FromPrimitive((byte)count.AsPrimitive));

    var compoundTag = new NbtCompound();
    var displayTag = new NbtCompound();

    foreach (var component in componentsToAdd)
    {
      switch (component)
      {
        case CustomDataItemComponent customDataItemComponent:
          if (customDataItemComponent.Value is not NbtCompound customDataTag)
            break;

          foreach (var (key, value) in customDataTag.Values)
            compoundTag[key] = value;

          break;

        case CustomNameItemComponent customNameItemComponent:
          displayTag["Name"] = new NbtString(customNameItemComponent.Value
            .SerializeJson().ToJsonString());
          break;

        case LoreItemComponent loreItemComponent:
          displayTag["Lore"] =
            new NbtList(
              loreItemComponent.Value.Select(v =>
                new NbtString(v.SerializeJson().ToJsonString())), NbtTagType.String);
          break;

        case DamageItemComponent damageItemComponent:
          compoundTag["Damage"] = new NbtShort((short)damageItemComponent.Value);
          break;

        case ProfileItemComponent profileItemComponent:
          var gameProfile = profileItemComponent.Value;
          var profileTag = new NbtCompound();
          profileTag["Id"] = new NbtString(gameProfile.Id.ToString());
          profileTag["Name"] = new NbtString(gameProfile.Username);

          var propertyTags = new NbtCompound[gameProfile.Properties.Length];
          for (var i = 0; i < gameProfile.Properties.Length; i++)
          {
            var property = gameProfile.Properties[i];
            var propertyTag = new NbtCompound();
            propertyTag["Value"] = new NbtString(property.Value);
            propertyTags[i] = propertyTag;
          }

          var texturesTag = new NbtList(propertyTags, NbtTagType.Compound);
          var propertiesTag = new NbtCompound();
          propertiesTag["textures"] = texturesTag;

          profileTag["Properties"] = propertiesTag;
          compoundTag["SkullOwner"] = profileTag;
          break;
      }
    }

    if (displayTag.Values.Count > 0)
      compoundTag["display"] = displayTag;

    wrapper.Write(NbtProperty.FromNbtTag(compoundTag));
  }

  public void PassthroughComponent(IMinecraftBinaryPacketWrapper wrapper,
    ProtocolVersion fromProtocolVersion, ProtocolVersion toProtocolVersion)
  {
    logger.LogTrace($"Passthrough component {fromProtocolVersion} -> {toProtocolVersion}");

    var count = wrapper.Passthrough<VarIntProperty>();
    if (count.AsPrimitive == 0)
      return;

    var itemId = wrapper.Read<VarIntProperty>();
    var itemIdentifier = MinecraftItemRegistry.GetIdentifier(fromProtocolVersion, itemId.AsPrimitive);
    var updatedItemId = MinecraftItemRegistry.GetId(toProtocolVersion, itemIdentifier);
    wrapper.Write(VarIntProperty.FromPrimitive(updatedItemId));

    var componentToAddCount = wrapper.Passthrough<VarIntProperty>(); // Component to add count
    wrapper.Passthrough<VarIntProperty>(); // Component to remove count

    for (var i = 0; i < componentToAddCount.AsPrimitive; i++)
    {
      var componentId = wrapper.Read<VarIntProperty>();

      var customDataComponentId = CustomDataItemComponent.GetId(fromProtocolVersion);
      if (customDataComponentId == componentId.AsPrimitive)
      {
        wrapper.Write(VarIntProperty.FromPrimitive(CustomDataItemComponent.GetId(toProtocolVersion)));
        wrapper.Passthrough<CustomDataItemComponentProperty>();
        continue;
      }

      var customNameComponentId = CustomNameItemComponent.GetId(fromProtocolVersion);
      if (customNameComponentId == componentId.AsPrimitive)
      {
        wrapper.Write(VarIntProperty.FromPrimitive(CustomNameItemComponent.GetId(toProtocolVersion)));
        wrapper.Passthrough<CustomNameItemComponentProperty>();
        continue;
      }

      var loreComponentId = LoreItemComponent.GetId(fromProtocolVersion);
      if (loreComponentId == componentId.AsPrimitive)
      {
        wrapper.Write(VarIntProperty.FromPrimitive(LoreItemComponent.GetId(toProtocolVersion)));
        wrapper.Passthrough<LoreItemComponentProperty>();
        continue;
      }

      var damageItemComponentId = DamageItemComponent.GetId(fromProtocolVersion);
      if (damageItemComponentId == componentId.AsPrimitive)
      {
        wrapper.Write(VarIntProperty.FromPrimitive(DamageItemComponent.GetId(toProtocolVersion)));
        wrapper.Passthrough<DamageItemComponentProperty>();
        continue;
      }

      var profileItemComponentId = ProfileItemComponent.GetId(fromProtocolVersion);
      if (profileItemComponentId == componentId.AsPrimitive)
      {
        wrapper.Write(VarIntProperty.FromPrimitive(ProfileItemComponent.GetId(toProtocolVersion)));
        wrapper.Passthrough<ProfileItemComponentProperty>();
        continue;
      }

      logger.LogError($"Unknown item component with id {componentId.AsPrimitive} in protocol {fromProtocolVersion}");
    }
  }
}
