using Menu.Protocol.Transformations;
using Menu.Services;
using Microsoft.Extensions.Logging;
using Void.Common.Plugins;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;

namespace Menu;

public class MenuPlugin(ILogger<MenuPlugin> logger, IEventService eventService) : IPlugin
{
  public string Name => nameof(MenuPlugin);

  [Subscribe]
  public void OnProxyStarting(ProxyStartingEvent @event)
  {
    var itemStackTransformation = new ItemStackTransformation(logger);
    var setContainerSlotTransformation = new SetContainerSlotTransformation(logger, itemStackTransformation);
    var openContainerTransformation = new OpenContainerTransformation(logger);
    var closeContainerTransformation = new CloseContainerTransformation(logger);
    var clickContainerTransformation = new ClickContainerTransformation(logger);

    eventService.RegisterListener<PacketService>();
    eventService.RegisterListener<TansformationService>(
      setContainerSlotTransformation,
      openContainerTransformation,
      closeContainerTransformation,
      clickContainerTransformation
    );
    eventService.RegisterListener<MenuService>();
  }
}
