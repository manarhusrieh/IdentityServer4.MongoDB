using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using IdentityModel;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.MongoDB.Repositories;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Stores
{
    public class ResourceStoreTests : IClassFixture<HostContainer>
    {
        private readonly HostContainer _hostContainer;

        public ResourceStoreTests(HostContainer hostContainer)
        {
            _hostContainer = hostContainer;
        }

        private static Models.IdentityResource CreateIdentityTestResource()
        {
            return new Models.IdentityResource
            {
                Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = true,
                UserClaims =
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name
                }
            };
        }

        private static Models.ApiResource CreateApiTestResource()
        {
            return new Models.ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                ApiSecrets = new List<Models.Secret> {new Models.Secret(Models.HashExtensions.Sha256("secret"))},
                Scopes =
                    new List<Models.Scope>
                    {
                        new Models.Scope
                        {
                            Name = Guid.NewGuid().ToString(),
                            UserClaims = {Guid.NewGuid().ToString()}
                        }
                    },
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }

        [Fact]
        public async Task FindResourcesAsync_WhenResourcesExist_ExpectResourcesReturned()
        {
            // Arrange
            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();

            var mapper = _hostContainer.Container.Resolve<IMapper>();

            var identityResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.IdentityResource>>();
            await identityResourceRepository.AddAsync(mapper.Map<Entities.IdentityResource>(testIdentityResource)).ConfigureAwait(false);

            var apiResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.ApiResource>>();
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(testApiResource)).ConfigureAwait(false);

            // Act
            var resourceStore = _hostContainer.Container.Resolve<IResourceStore>();
            var resources = await resourceStore.FindResourcesByScopeAsync(new List<string>
            {
                testIdentityResource.Name,
                testApiResource.Scopes.First().Name
            }).ConfigureAwait(false);

            // Assert
            Assert.NotNull(resources);
            Assert.NotNull(resources.IdentityResources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotNull(resources.ApiResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.NotNull(resources.IdentityResources.FirstOrDefault(x => x.Name == testIdentityResource.Name));
            Assert.NotNull(resources.ApiResources.FirstOrDefault(x => x.Name == testApiResource.Name));
        }

        [Fact]
        public async Task FindResourcesAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            // Arrange
            var testIdentityResource = CreateIdentityTestResource();
            var testIdentityResource2 = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();
            var testApiResource2 = CreateApiTestResource();

            var mapper = _hostContainer.Container.Resolve<IMapper>();

            var identityResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.IdentityResource>>();
            await identityResourceRepository.AddAsync(mapper.Map<Entities.IdentityResource>(testIdentityResource)).ConfigureAwait(false);
            await identityResourceRepository.AddAsync(mapper.Map<Entities.IdentityResource>(testIdentityResource2)).ConfigureAwait(false);

            var apiResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.ApiResource>>();
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(testApiResource)).ConfigureAwait(false);
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(testApiResource2)).ConfigureAwait(false);

            // Act
            var resourceStore = _hostContainer.Container.Resolve<IResourceStore>();
            var resources = await resourceStore.FindResourcesByScopeAsync(new List<string>
            {
                testIdentityResource.Name,
                testApiResource.Scopes.First().Name
            }).ConfigureAwait(false);

            // Assert
            Assert.NotNull(resources);
            Assert.NotNull(resources.IdentityResources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotNull(resources.ApiResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.Equal(1, resources.IdentityResources.Count);
            Assert.Equal(1, resources.ApiResources.Count);
        }

        [Fact]
        public async Task GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden()
        {
            // Arrange
            var visibleIdentityResource = CreateIdentityTestResource();
            var visibleApiResource = CreateApiTestResource();
            var hiddenIdentityResource = new Models.IdentityResource {Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false};
            var hiddenApiResource = new Models.ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                Scopes = new List<Models.Scope> {new Models.Scope {Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false}}
            };

            var mapper = _hostContainer.Container.Resolve<IMapper>();

            var identityResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.IdentityResource>>();
            await identityResourceRepository.AddAsync(mapper.Map<Entities.IdentityResource>(visibleIdentityResource)).ConfigureAwait(false);
            await identityResourceRepository.AddAsync(mapper.Map<Entities.IdentityResource>(hiddenIdentityResource)).ConfigureAwait(false);

            var apiResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.ApiResource>>();
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(visibleApiResource)).ConfigureAwait(false);
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(hiddenApiResource)).ConfigureAwait(false);

            // Act
            var resourceStore = _hostContainer.Container.Resolve<IResourceStore>();
            var resources = await resourceStore.GetAllResourcesAsync().ConfigureAwait(false);

            // Assert
            Assert.NotNull(resources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.Contains(resources.IdentityResources, x => !x.ShowInDiscoveryDocument);
            Assert.Contains(resources.ApiResources, x => !x.Scopes.Any(y => y.ShowInDiscoveryDocument));
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            // Arrange
            var resource = CreateIdentityTestResource();

            var mapper = _hostContainer.Container.Resolve<IMapper>();

            var identityResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.IdentityResource>>();
            await identityResourceRepository.AddAsync(mapper.Map<Entities.IdentityResource>(resource)).ConfigureAwait(false);

            // Act
            var resourceStore = _hostContainer.Container.Resolve<IResourceStore>();
            var resources = (await resourceStore.FindIdentityResourcesByScopeAsync(new List<string>
            {
                resource.Name
            }).ConfigureAwait(false)).ToList();


            // Assert
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);

            var foundScope = resources.Single();
            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            // Arrange
            var resource = CreateIdentityTestResource();

            var mapper = _hostContainer.Container.Resolve<IMapper>();

            var identityResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.IdentityResource>>();
            await identityResourceRepository.AddAsync(mapper.Map<Entities.IdentityResource>(resource)).ConfigureAwait(false);

            var apiResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.ApiResource>>();
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(CreateApiTestResource())).ConfigureAwait(false);

            // Act
            var resourceStore = _hostContainer.Container.Resolve<IResourceStore>();
            var resources = (await resourceStore.FindIdentityResourcesByScopeAsync(new List<string>
            {
                resource.Name
            }).ConfigureAwait(false)).ToList();

            // Assert
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.Single(resources);
        }

        [Fact]
        public async Task FindApiResourceAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            // Arrange
            var resource = CreateApiTestResource();

            var mapper = _hostContainer.Container.Resolve<IMapper>();

            var apiResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.ApiResource>>();
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(resource)).ConfigureAwait(false);

            // Act
            var resourceStore = _hostContainer.Container.Resolve<IResourceStore>();
            var foundResource = await resourceStore.FindApiResourceAsync(resource.Name).ConfigureAwait(false);

            // Assert
            Assert.NotNull(foundResource);
            Assert.NotNull(foundResource.UserClaims);
            Assert.NotEmpty(foundResource.UserClaims);
            Assert.NotNull(foundResource.ApiSecrets);
            Assert.NotEmpty(foundResource.ApiSecrets);
            Assert.NotNull(foundResource.Scopes);
            Assert.NotEmpty(foundResource.Scopes);
            Assert.Contains(foundResource.Scopes, x => x.UserClaims.Any());
        }

        [Fact]
        public async Task FindApiResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            // Arrange
            var resource = CreateApiTestResource();

            var mapper = _hostContainer.Container.Resolve<IMapper>();

            var apiResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.ApiResource>>();
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(resource)).ConfigureAwait(false);

            // Act
            var resourceStore = _hostContainer.Container.Resolve<IResourceStore>();
            var resources = (await resourceStore.FindApiResourcesByScopeAsync(new List<string>
            {
                resource.Scopes.First().Name
            }).ConfigureAwait(false)).ToList();

            // Assert
            Assert.NotEmpty(resources);
            Assert.NotNull(resources);

            Assert.NotNull(resources.First().UserClaims);
            Assert.NotEmpty(resources.First().UserClaims);
            Assert.NotNull(resources.First().ApiSecrets);
            Assert.NotEmpty(resources.First().ApiSecrets);
            Assert.NotNull(resources.First().Scopes);
            Assert.NotEmpty(resources.First().Scopes);
            Assert.Contains(resources.First().Scopes, x => x.UserClaims.Any());
        }

        [Fact]
        public async Task FindApiResourcesByScopeAsync_WhenMultipleResourcesExist_ExpectOnlyRequestedResourcesReturned()
        {
            // Arrange
            var resource = CreateApiTestResource();

            var mapper = _hostContainer.Container.Resolve<IMapper>();

            var apiResourceRepository = _hostContainer.Container.Resolve<IRepository<Entities.ApiResource>>();
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(resource)).ConfigureAwait(false);
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(CreateApiTestResource())).ConfigureAwait(false);
            await apiResourceRepository.AddAsync(mapper.Map<Entities.ApiResource>(CreateApiTestResource())).ConfigureAwait(false);

            // Act
            var resourceStore = _hostContainer.Container.Resolve<IResourceStore>();
            var resources = (await resourceStore.FindApiResourcesByScopeAsync(new List<string>
            {
                resource.Scopes.First().Name
            }).ConfigureAwait(false)).ToList();

            // Assert
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.Single(resources);
        }
    }
}