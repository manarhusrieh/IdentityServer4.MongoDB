using System.Linq;
using AutoMapper;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Mappers
{
    public class ApiResourceMapperProfileTests
    {
        [Fact]
        public void Map()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<ApiResourceMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));
            var model = new Models.ApiResource(nameof(ApiResourceMapperProfileTests));

            // Act
            var entity = mapper.Map<Entities.ApiResource>(model);
            model = mapper.Map<Models.ApiResource>(entity);

            // Assert
            Assert.NotNull(entity);
            Assert.NotNull(model);
            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Map_Properties()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<ApiResourceMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));

            var model = new Models.ApiResource
            {
                Description = "description",
                DisplayName = "displayname",
                Name = "foo",
                Scopes = {new Models.Scope("foo1"), new Models.Scope("foo2")},
                Enabled = false
            };

            // Act
            var mappedEntity = mapper.Map<Entities.ApiResource>(model);
            var mappedModel = mapper.Map<Models.ApiResource>(mappedEntity);

            // Assert
            Assert.NotNull(mappedEntity);
            Assert.Equal(2, mappedEntity.Scopes.Count);
            Assert.NotNull(mappedEntity.Scopes.FirstOrDefault(x => x.Name == "foo1"));
            Assert.NotNull(mappedEntity.Scopes.FirstOrDefault(x => x.Name == "foo2"));

            Assert.NotNull(model);
            Assert.Equal("description", mappedModel.Description);
            Assert.Equal("displayname", mappedModel.DisplayName);
            Assert.False(mappedModel.Enabled);
            Assert.Equal("foo", mappedModel.Name);
        }

        [Fact]
        public void Map_Use_Defaults()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<ApiResourceMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));

            var entity = new Entities.ApiResource
            {
                Secrets = new System.Collections.Generic.List<Entities.ApiSecret>
                {
                    new Entities.ApiSecret
                    {
                    }
                }
            };

            var def = new Models.ApiResource
            {
                ApiSecrets = {new Models.Secret("foo")}
            };

            // Act
            var mappedModel = mapper.Map<Models.ApiResource>(entity);

            // Assert
            Assert.Equal(def.ApiSecrets.First().Type, mappedModel.ApiSecrets.First().Type);
        }
    }
}