using Microsoft.Extensions.DependencyInjection;
using PlayerPositions.Services;
using Void.Common.Plugins;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins;

namespace PlayerPositions;

public class PlayerPositionsPlugin(IDependencyService dependencyService) : IApiPlugin
{
  public string Name => nameof(PlayerPositionsPlugin);

  [Subscribe]
  private void OnProxyStarting(ProxyStartingEvent @event)
  {
    dependencyService.Register(services =>
    {
      services.AddSingleton<PacketService>();
      services.AddSingleton<PositionService>();
    });

    dependencyService.CreateInstance<PacketService>();
    dependencyService.CreateInstance<PositionService>();
  }
}
