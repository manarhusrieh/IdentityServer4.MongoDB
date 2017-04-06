using System.Collections.Generic;
using IdentityServer4.MongoDB.Repositories;

namespace IdentityServer4.MongoDB.Entities
{
    public class IdentityResource : Entity
    {
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<IdentityClaim> UserClaims { get; set; }
    }
}
