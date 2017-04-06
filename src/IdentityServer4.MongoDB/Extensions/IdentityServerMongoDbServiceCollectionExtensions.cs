using System;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.MongoDB.Options;
using IdentityServer4.MongoDB.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.MongoDB.Extensions
{
    public static class IdentityServerMongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServerMongoDbRepositories(this IServiceCollection services, StoreOptions storeOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IRepository<Client>>(provider => new MongoRepository<Client>(storeOptions));
            services.AddTransient<IRepository<ApiResource>>(provider => new MongoRepository<ApiResource>(storeOptions));
            services.AddTransient<IRepository<IdentityResource>>(provider => new MongoRepository<IdentityResource>(storeOptions));
            return services;
        }

        public static IServiceCollection AddIdentityServerMongoDbOperationalRepositories(this IServiceCollection services, StoreOptions storeOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IRepository<PersistedGrant>>(provider => new MongoRepository<PersistedGrant>(storeOptions));
            return services;
        }

        public static IServiceCollection AddIdentityServerMongoDbMappers(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var mapperConfiguration = new AutoMapper.MapperConfiguration(expression =>
            {
                expression.AddProfile<ApiResourceMapperProfile>();
                expression.AddProfile<ClientMapperProfile>();
                expression.AddProfile<IdentityResourceMapperProfile>();
                expression.AddProfile<PersistedGrantMapperProfile>();
            });
            services.AddTransient<IMapper>(provider => new AutoMapperWrapper(new AutoMapper.Mapper(mapperConfiguration, provider.GetService)));
            return services;
        }
    }
}