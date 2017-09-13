using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.MongoDB.Entities;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.MongoDB.Extensions;
using IdentityServer4.MongoDB.Repositories;

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

            // clear all data
            ClearData();
        }

        private void ClearData()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var database = scope.Resolve<IRepository<Client>>().Collection.Database;
                database.Client.DropDatabase(database.DatabaseNamespace.DatabaseName);
            }
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}