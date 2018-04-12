using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.MongoDB.Options;
using IdentityServer4.MongoDB.Services;
using IdentityServer4.MongoDB.Stores;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer4.MongoDB.Extensions
{
    public static class IdentityServerMongoDbBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder,
            Action<StoreOptions> storeOptionsAction = null)
        {
            builder.AddClientStore<ClientStore>();
            builder.AddResourceStore<ResourceStore>();
            builder.AddCorsPolicyService<CorsPolicyService>();

            var storeOptions = new StoreOptions();
            storeOptionsAction?.Invoke(storeOptions);
            builder.Services.AddIdentityServerMongoDbRepositories(storeOptions).AddIdentityServerMongoDbMappers();

            return builder;
        }

        public static IIdentityServerBuilder AddConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();
            builder.AddCorsPolicyCache<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            Action<StoreOptions> storeOptionsAction = null,
            Action<TokenCleanupOptions> tokenCleanUpOptionsAction = null)
        {
            builder.Services.AddSingleton<TokenCleanup>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var storeOptions = new StoreOptions();
            storeOptionsAction?.Invoke(storeOptions);
            builder.Services.AddIdentityServerMongoDbOperationalRepositories(storeOptions);

            var tokenCleanupOptions = new TokenCleanupOptions();
            tokenCleanUpOptionsAction?.Invoke(tokenCleanupOptions);
            builder.Services.AddSingleton(tokenCleanupOptions);

            return builder;
        }

        private class TokenCleanupHost : IHostedService
        {
            private readonly TokenCleanup _tokenCleanup;
            private readonly TokenCleanupOptions _options;

            public TokenCleanupHost(TokenCleanup tokenCleanup, TokenCleanupOptions options)
            {
                _tokenCleanup = tokenCleanup;
                _options = options;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                if (_options.EnableTokenCleanup)
                {
                    _tokenCleanup.Start(cancellationToken);
                }

                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                if (_options.EnableTokenCleanup)
                {
                    _tokenCleanup.Stop();
                }

                return Task.CompletedTask;
            }
        }
    }
}