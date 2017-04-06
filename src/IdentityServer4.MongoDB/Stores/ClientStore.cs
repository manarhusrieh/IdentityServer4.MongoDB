using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.MongoDB.Repositories;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;

namespace IdentityServer4.MongoDB.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly IRepository<Entities.Client> _clientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientStore> _logger;

        public ClientStore(IRepository<Entities.Client> clientRepository, IMapper mapper, ILogger<ClientStore> logger)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _clientRepository.AsQueryable().FirstOrDefaultAsync(x => x.ClientId == clientId).ConfigureAwait(false);
            var model = _mapper.Map<Client>(client);

            _logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, model != null);

            return model;
        }
    }
}