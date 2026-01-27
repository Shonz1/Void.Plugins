using Void.Data.Api.Minecraft;
using Void.Minecraft.Components.Text;

namespace Menus.Api;

public record Menu(string Name, Identifier Type, int Size, Component Title, Dictionary<int, MenuItem> ItemsMap);
