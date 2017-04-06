using System;
using IdentityServer4.MongoDB.Options;
using IdentityServer4.MongoDB.Services;
using IdentityServer4.MongoDB.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.MongoDB.Extensions
{
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder,
            Action<StoreOptions> storeOptionsAction = null)
        {
            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            var storeOptions = new StoreOptions();
            storeOptionsAction?.Invoke(storeOptions);
            builder.Services.AddIdentityServerMongoDbRepositories(storeOptions).AddIdentityServerMongoDbMappers();

            return builder;
        }

        public static IIdentityServerBuilder AddConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            // these need to be registered as concrete classes in DI for
            // the caching decorators to work
            builder.Services.AddTransient<ClientStore>();
            builder.Services.AddTransient<ResourceStore>();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            Action<StoreOptions> storeOptionsAction = null,
            Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var storeOptions = new StoreOptions();
            storeOptionsAction?.Invoke(storeOptions);
            builder.Services.AddIdentityServerMongoDbOperationalRepositories(storeOptions);

            var tokenCleanupOptions = new TokenCleanupOptions();
            tokenCleanUpOptions?.Invoke(tokenCleanupOptions);
            builder.Services.AddSingleton(provider => new TokenCleanup(provider, provider.GetRequiredService<ILogger<TokenCleanup>>(), tokenCleanupOptions));

            return builder;
        }

        public static IApplicationBuilder UseIdentityServerTokenCleanup(this IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            var tokenCleanup = app.ApplicationServices.GetService<TokenCleanup>();
            if (tokenCleanup == null) throw new InvalidOperationException("AddOperationalStore must be called on the service collection.");

            applicationLifetime.ApplicationStarted.Register(tokenCleanup.Start);
            applicationLifetime.ApplicationStopping.Register(tokenCleanup.Stop);

            return app;
        }
    }
}