using System;
using System.Threading.Tasks;
using Autofac;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.MongoDB.Repositories;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Stores
{
    public class ClientStoreTests : IClassFixture<HostContainer>
    {
        private readonly HostContainer _hostContainer;

        public ClientStoreTests(HostContainer hostContainer)
        {
            _hostContainer = hostContainer;
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExists_ExpectClientRetured()
        {
            // Arrange
            var testClient = new Models.Client
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = Guid.NewGuid().ToString()
            };
            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.Client>>();
            await repository.AddAsync(mapper.Map<Entities.Client>(testClient)).ConfigureAwait(false);

            // Assert
            var clientStore = _hostContainer.Container.Resolve<IClientStore>();
            var client = await clientStore.FindClientByIdAsync(testClient.ClientId).ConfigureAwait(false);

            // Assert
            Assert.NotNull(client);
        }
    }
}