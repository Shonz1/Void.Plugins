using Menus.Minecraft;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Profiles;

namespace Menus.Api;

public record MenuItem(Identifier Identifier, string Command, int Count = 1, Component? Title = null, List<Component>? Lore = null, GameProfile? Profile = null);
