using System;
using IdentityServer4.MongoDB.Repositories;

namespace IdentityServer4.MongoDB.Entities
{
    public class PersistedGrant : Entity
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string SubjectId { get; set; }
        public string ClientId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? Expiration { get; set; }
        public string Data { get; set; }
    }
}