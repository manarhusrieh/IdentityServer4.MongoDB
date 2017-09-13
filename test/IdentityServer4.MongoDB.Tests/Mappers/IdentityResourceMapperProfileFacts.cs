using AutoMapper;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Mappers
{
    public class IdentityResourceMapperProfileFacts
    {
        [Fact]
        public void Map()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<IdentityResourceMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));
            var model = new Models.IdentityResource();

            // Act
            var entity = mapper.Map<Entities.IdentityResource>(model);
            model = mapper.Map<Models.IdentityResource>(entity);

            // Assert
            Assert.NotNull(entity);
            Assert.NotNull(model);
            mapperConfiguration.AssertConfigurationIsValid();
        }
    }
}