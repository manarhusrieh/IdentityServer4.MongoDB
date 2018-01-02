using System;
using Autofac;
using IdentityServer4.MongoDB.Entities;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.MongoDB.Extensions;
using IdentityServer4.MongoDB.Repositories;

namespace IdentityServer4.MongoDB.Tests
{
    public class HostContainer : IDisposable
    {
        private static readonly object SyncObj = new object();
        private static bool _isInitialized;

        public ILifetimeScope Container { get; }

        public HostContainer()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddIdentityServer()
                .AddConfigurationStore()
                .AddOperationalStore();

            var containerBuilder = new ContainerBuilder();
            //containerBuilder.Populate(services);
            Container = containerBuilder.Build();

            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (SyncObj)
                {
                    if (!_isInitialized)
                    {
                        using (var scope = Container.BeginLifetimeScope())
                        {
                            // clear all data
                            var database = scope.Resolve<IRepository<Client>>().Collection.Database;
                            database.Client.DropDatabase(database.DatabaseNamespace.DatabaseName);
                        }
                        _isInitialized = true;
                    }
                }
            }
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}