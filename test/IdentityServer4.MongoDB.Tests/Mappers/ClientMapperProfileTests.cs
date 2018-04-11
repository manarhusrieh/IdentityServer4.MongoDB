using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.MongoDB.Tests.Mappers
{
    public class ClientMapperProfileTests
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

        [Fact]
        public void Map_Properties()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<ClientMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));
            var model = new Models.Client
            {
                Properties =
                {
                    {"foo1", "bar1"},
                    {"foo2", "bar2"}
                }
            };

            // Act
            var mappedEntity = mapper.Map<Entities.Client>(model);
            var mappedModel = mapper.Map<Models.Client>(mappedEntity);

            // Assert
            Assert.NotNull(mappedEntity);
            Assert.Equal(2, mappedEntity.Properties.Count);
            var foo1 = mappedEntity.Properties.FirstOrDefault(x => x.Key == "foo1");
            Assert.NotNull(foo1);
            Assert.Equal("bar1", foo1.Value);
            var foo2 = mappedEntity.Properties.FirstOrDefault(x => x.Key == "foo2");
            Assert.NotNull(foo2);
            Assert.Equal("bar2", foo2.Value);

            Assert.NotNull(mappedModel);
            Assert.Equal(2, mappedModel.Properties.Count);
            Assert.True(mappedModel.Properties.ContainsKey("foo1"));
            Assert.True(mappedModel.Properties.ContainsKey("foo2"));
            Assert.Equal("bar1", mappedModel.Properties["foo1"]);
            Assert.Equal("bar2", mappedModel.Properties["foo2"]);
        }

        [Fact]
        public void Map_Duplicated_Properties_ThorwsException()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<ClientMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));
            var entity = new Entities.Client
            {
                Properties = new System.Collections.Generic.List<Entities.ClientProperty>
                {
                    new Entities.ClientProperty {Key = "foo1", Value = "bar1"},
                    new Entities.ClientProperty {Key = "foo1", Value = "bar2"},
                }
            };

            // Act & Assert
            Assert.Throws<AutoMapperMappingException>(() => mapper.Map<Models.Client>(entity));
        }

        [Fact]
        public void Map_Use_Defaults()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(expression => { expression.AddProfile<ClientMapperProfile>(); });
            var mapper = new AutoMapperWrapper(new Mapper(mapperConfiguration));
            var entity = new Entities.Client
            {
                ClientSecrets = new List<Entities.ClientSecret>
                {
                    new Entities.ClientSecret
                    {
                    }
                }
            };

            var def = new Models.Client
            {
                ClientSecrets = {new Models.Secret("foo")}
            };

            // Act
            var mappedModel = mapper.Map<Models.Client>(entity);

            // Assert
            Assert.Equal(def.ProtocolType, mappedModel.ProtocolType);
            Assert.Equal(def.ClientSecrets.First().Type, mappedModel.ClientSecrets.First().Type);
        }
    }
}