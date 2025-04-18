﻿using Microsoft.Extensions.DependencyInjection;
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
      services.AddSingleton<PacketService>();
      services.AddSingleton<PositionService>();
    });
  }
}
