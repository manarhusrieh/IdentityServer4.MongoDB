using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using IdentityServer4.Models;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.MongoDB.Repositories;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Services
{
    public class CorsPolicyServiceTests : IClassFixture<HostContainer>
    {
        private readonly HostContainer _hostContainer;

        public CorsPolicyServiceTests(HostContainer hostContainer)
        {
            hostContainer.RegisterHttpContext();
            _hostContainer = hostContainer;
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue()
        {
            // Arrange
            const string testCorsOrigin = "https://jingwu.me/";

            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.Client>>();
            await repository.AddAsync(mapper.Map<IEnumerable<Entities.Client>>(
                new[]
                {
                    new Client
                    {
                        ClientId = Guid.NewGuid().ToString(),
                        ClientName = Guid.NewGuid().ToString(),
                        AllowedCorsOrigins = new List<string> {"https://www.selz.com"}
                    },
                    new Client
                    {
                        ClientId = "2",
                        ClientName = "2",
                        AllowedCorsOrigins = new List<string> {"https://www.selz.com", testCorsOrigin}
                    }
                })
            ).ConfigureAwait(false);

            // Act
            var corsPolicyService = _hostContainer.Container.Resolve<ICorsPolicyService>();
            var result = await corsPolicyService.IsOriginAllowedAsync(testCorsOrigin).ConfigureAwait(false);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse()
        {
            // Arrange
            var mapper = _hostContainer.Container.Resolve<IMapper>();
            var repository = _hostContainer.Container.Resolve<IRepository<Entities.Client>>();
            await repository.AddAsync(mapper.Map<Entities.Client>(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> {"https://www.selz.com"}
                })
            ).ConfigureAwait(false);

            // Act
            var corsPolicyService = _hostContainer.Container.Resolve<ICorsPolicyService>();
            var result = await corsPolicyService.IsOriginAllowedAsync("InvalidOrigin").ConfigureAwait(false);

            // Assert
            Assert.False(result);
        }
    }
}