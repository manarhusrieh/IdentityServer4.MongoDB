using AutoMapper;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Mappers
{
    public class ClientMapperProfileFacts
    {
        [Fact]
        public void Map()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<ClientMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));
            var model = new Models.Client();

            // Act
            var entity = mapper.Map<Entities.Client>(model);
            model = mapper.Map<Models.Client>(entity);

            // Assert
            Assert.NotNull(entity);
            Assert.NotNull(model);
            mapperConfiguration.AssertConfigurationIsValid();
        }
    }
}