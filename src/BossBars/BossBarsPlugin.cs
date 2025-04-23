using BossBars.Services;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;

namespace BossBars;

public class BossBarsPlugin(IDependencyService dependencyService) : IApiPlugin
{
  public string Name => nameof(BossBarsPlugin);

  [Subscribe]
  private void OnProxyStarting(ProxyStartingEvent @event)
  {
    dependencyService.Register(services =>
    {
      services.AddSingleton<PacketService>();
      services.AddSingleton<BossBarService>();
    });
  }
}
