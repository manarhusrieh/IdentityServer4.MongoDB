using System;
using System.Threading.Tasks;
using Autofac;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.MongoDB.Repositories;
using IdentityServer4.Stores;
using MongoDB.Driver.Linq;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Stores
{
    public class PersistedGrantStoreTests : IClassFixture<HostContainer>
    {
        private readonly HostContainer _hostContainer;

        public PersistedGrantStoreTests(HostContainer hostContainer)
        {
            _hostContainer = hostContainer;
        }

        private static Models.PersistedGrant CreateTestObject()
        {
            return new Models.PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = "authorization_code",
                ClientId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                CreationTime = new DateTime(2016, 08, 01),
                Expiration = new DateTime(2016, 08, 31),
                Data = Guid.NewGuid().ToString()
            };
        }

        [Fact]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
        {
            // Arrange
            var persistedGrant = CreateTestObject();

            // Act
            var persistedGrantStore = _hostContainer.Container.Resolve<IPersistedGrantStore>();
            await persistedGrantStore.StoreAsync(persistedGrant).ConfigureAwait(false);

            // Assert
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.PersistedGrant>>();
            var foundGrant = await repository.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key).ConfigureAwait(false);
            Assert.NotNull(foundGrant);
        }

        [Fact]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectUpdateSuccess()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.PersistedGrant>>();
            await repository.AddAsync(mapper.Map<Entities.PersistedGrant>(persistedGrant)).ConfigureAwait(false);

            var newDate = DateTime.Now;
            persistedGrant.Expiration = newDate;

            // Act
            var persistedGrantStore = _hostContainer.Container.Resolve<IPersistedGrantStore>();
            await persistedGrantStore.StoreAsync(persistedGrant).ConfigureAwait(false);

            // Assert
            var foundGrant = await repository.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key).ConfigureAwait(false);
            Assert.NotNull(foundGrant);
            Assert.Equal(newDate, persistedGrant.Expiration);
        }

        [Fact]
        public async Task GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.PersistedGrant>>();
            await repository.AddAsync(mapper.Map<Entities.PersistedGrant>(persistedGrant)).ConfigureAwait(false);

            // Act
            var persistedGrantStore = _hostContainer.Container.Resolve<IPersistedGrantStore>();
            var foundPersistedGrant = await persistedGrantStore.GetAsync(persistedGrant.Key).ConfigureAwait(false);

            // Assert
            Assert.NotNull(foundPersistedGrant);
        }

        [Fact]
        public async Task GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.PersistedGrant>>();
            await repository.AddAsync(mapper.Map<Entities.PersistedGrant>(persistedGrant)).ConfigureAwait(false);

            // Act
            var persistedGrantStore = _hostContainer.Container.Resolve<IPersistedGrantStore>();
            var foundPersistedGrants = await persistedGrantStore.GetAllAsync(persistedGrant.SubjectId);

            // Assert
            Assert.NotNull(foundPersistedGrants);
            Assert.NotEmpty(foundPersistedGrants);
        }

        [Fact]
        public async Task RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.PersistedGrant>>();
            await repository.AddAsync(mapper.Map<Entities.PersistedGrant>(persistedGrant)).ConfigureAwait(false);

            // Act
            var persistedGrantStore = _hostContainer.Container.Resolve<IPersistedGrantStore>();
            await persistedGrantStore.RemoveAsync(persistedGrant.Key).ConfigureAwait(false);

            // Assert
            var foundGrant = await repository.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key).ConfigureAwait(false);
            Assert.Null(foundGrant);
        }

        [Fact]
        public async Task RemoveAllAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.PersistedGrant>>();
            await repository.AddAsync(mapper.Map<Entities.PersistedGrant>(persistedGrant)).ConfigureAwait(false);

            // Act
            var persistedGrantStore = _hostContainer.Container.Resolve<IPersistedGrantStore>();
            await persistedGrantStore.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId).ConfigureAwait(false);

            // Assert
            var foundGrant = await repository.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key).ConfigureAwait(false);
            Assert.Null(foundGrant);
        }

        [Fact]
        public async Task RemoveAllAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            // Arrange
            var persistedGrant = CreateTestObject();
            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.PersistedGrant>>();
            await repository.AddAsync(mapper.Map<Entities.PersistedGrant>(persistedGrant)).ConfigureAwait(false);

            // Act
            var persistedGrantStore = _hostContainer.Container.Resolve<IPersistedGrantStore>();
            await persistedGrantStore.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type).ConfigureAwait(false);

            // Assert
            var foundGrant = await repository.AsQueryable().FirstOrDefaultAsync(x => x.Key == persistedGrant.Key).ConfigureAwait(false);
            Assert.Null(foundGrant);
        }
    }
}