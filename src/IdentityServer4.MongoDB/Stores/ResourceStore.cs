using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.MongoDB.Repositories;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IdentityServer4.MongoDB.Stores
{
    public class ResourceStore : IResourceStore
    {
        private readonly IRepository<Entities.IdentityResource> _identityResourceRepository;
        private readonly IRepository<Entities.ApiResource> _apiResourceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ResourceStore> _logger;

        public ResourceStore(IRepository<Entities.IdentityResource> identityResourceRepository, IRepository<Entities.ApiResource> apiResourceRepository, IMapper mapper, ILogger<ResourceStore> logger)
        {
            _identityResourceRepository = identityResourceRepository;
            _apiResourceRepository = apiResourceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();
            var identities = await _identityResourceRepository.AsQueryable().Where(x => names.Contains(x.Name)).ToListAsync().ConfigureAwait(false);
            var models = _mapper.Map<IEnumerable<IdentityResource>>(identities).ToArray();

            _logger.LogDebug("Found {scopes} identity scopes in database", models.Select(x => x.Name));

            return models;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();
            var apis = await _apiResourceRepository.AsQueryable().Where(x => names.Contains(x.Name)).ToListAsync().ConfigureAwait(false);
            var models = _mapper.Map<IEnumerable<ApiResource>>(apis).ToArray();

            _logger.LogDebug("Found {scopes} API scopes in database", models.SelectMany(x => x.Scopes).Select(x => x.Name));

            return models;
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var api = await _apiResourceRepository.AsQueryable().FirstOrDefaultAsync(x => x.Name == name).ConfigureAwait(false);
            var model = _mapper.Map<ApiResource>(api);

            _logger.LogDebug(model != null ? "Found {api} API resource in database" : "Did not find {api} API resource in database", name);

            return model;
        }

        public async Task<Resources> GetAllResources()
        {
            var identities = await _identityResourceRepository.AsQueryable().ToListAsync().ConfigureAwait(false);
            var apis = await _apiResourceRepository.AsQueryable().ToListAsync().ConfigureAwait(false);
            var identityModels = _mapper.Map<IEnumerable<IdentityResource>>(identities);
            var apiModels = _mapper.Map<IEnumerable<ApiResource>>(apis);

            var result = new Resources(identityModels, apiModels);
            _logger.LogDebug("Found {scopes} as all scopes in database", result.IdentityResources.Select(x => x.Name).Union(result.ApiResources.SelectMany(x => x.Scopes).Select(x => x.Name)));
            return result;
        }
    }
}