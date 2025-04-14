using Menu.Minecraft;
using Void.Minecraft.Components.Text;

namespace Menu.Api;

public record Menu(string Name, Identifier Type, int Size, Component Title, Dictionary<int, MenuItem> ItemsMap);
