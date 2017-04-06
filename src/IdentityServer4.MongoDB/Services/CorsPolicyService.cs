using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.MongoDB.Repositories;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IdentityServer4.MongoDB.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IRepository<Entities.Client> _clientRepository;
        private readonly ILogger<CorsPolicyService> _logger;

        public CorsPolicyService(IRepository<Entities.Client> clientRepository, ILogger<CorsPolicyService> logger)
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var origins = await _clientRepository.AsQueryable().SelectMany(x => x.AllowedCorsOrigins).Select(x => x.Origin).ToListAsync().ConfigureAwait(false);
            var distinctOrigins = origins.Where(x => x != null).Distinct();
            var isAllowed = distinctOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);

            _logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

            return isAllowed;
        }
    }
}