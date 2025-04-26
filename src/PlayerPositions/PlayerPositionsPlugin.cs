using Microsoft.Extensions.DependencyInjection;
using PlayerPositions.Protocol.Transformations;
using PlayerPositions.Services;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;

namespace PlayerPositions;

public class PlayerPositionsPlugin(IDependencyService dependencyService) : IApiPlugin
{
  public string Name => nameof(PlayerPositionsPlugin);

  [Subscribe]
  private void OnProxyStarting(ProxyStartingEvent @event)
  {
    dependencyService.Register(services =>
    {
      services.AddSingleton<SetPlayerPositionTransformation>();
      services.AddSingleton<SetPlayerRotationTransformation>();
      services.AddSingleton<SetPlayerPositionAndRotationTransformation>();

      services.AddScoped<PacketService>();
      services.AddScoped<TransformationService>();
      services.AddScoped<PositionService>();
    });
  }
}
