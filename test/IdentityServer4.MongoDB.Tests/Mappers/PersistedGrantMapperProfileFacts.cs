using AutoMapper;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Mappers
{
    public class PersistedGrantMapperProfileFacts
    {
        [Fact]
        public void Map()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<PersistedGrantMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));
            var model = new Models.PersistedGrant();

            // Act
            var entity = mapper.Map<Entities.PersistedGrant>(model);
            model = mapper.Map<Models.PersistedGrant>(entity);

            // Assert
            Assert.NotNull(entity);
            Assert.NotNull(model);
            mapperConfiguration.AssertConfigurationIsValid();
        }
    }
}