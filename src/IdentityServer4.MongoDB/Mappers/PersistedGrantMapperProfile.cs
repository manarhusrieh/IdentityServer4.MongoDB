using AutoMapper;

namespace IdentityServer4.MongoDB.Mappers
{
    /// <summary>
    /// AutoMapper Config for PersistedGrant
    /// Between Model and Entity
    /// </summary>
    public class PersistedGrantMapperProfile : Profile
    {
        public PersistedGrantMapperProfile()
        {
            // entity to model
            CreateMap<Entities.PersistedGrant, global::IdentityServer4.Models.PersistedGrant>(MemberList.Destination);

            // model to entity
            CreateMap<global::IdentityServer4.Models.PersistedGrant, Entities.PersistedGrant>(MemberList.Source);
        }
    }
}