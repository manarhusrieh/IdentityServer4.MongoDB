using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.MongoDB.Extensions;

namespace IdentityServer4.MongoDB.Tests
{
    public class HostContainer : IDisposable
    {
        public ILifetimeScope Container { get; }

        public HostContainer()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddIdentityServer()
                .AddConfigurationStore()
                .AddOperationalStore();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            Container = containerBuilder.Build();
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}