using Menus.Protocol.Transformations;
using Menus.Services;
using Microsoft.Extensions.DependencyInjection;
using Void.Common.Plugins;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins;

namespace Menus;

public class MenusPlugin(IDependencyService dependencyService) : IPlugin
{
  public string Name => nameof(MenusPlugin);

  [Subscribe]
  private void OnProxyStarting(ProxyStartingEvent @event)
  {
    dependencyService.Register(services =>
    {
      services.AddSingleton<ItemStackTransformation>();
      services.AddSingleton<SetContainerSlotTransformation>();
      services.AddSingleton<OpenContainerTransformation>();
      services.AddSingleton<CloseContainerTransformation>();
      services.AddSingleton<ClickContainerTransformation>();

      services.AddSingleton<PacketService>();
      services.AddSingleton<TransformationService>();
      services.AddSingleton<MenuService>();
    });

    dependencyService.CreateInstance<PacketService>();
    dependencyService.CreateInstance<TransformationService>();
    dependencyService.CreateInstance<MenuService>();
  }
}
