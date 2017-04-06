using MongoDB.Driver;

namespace IdentityServer4.MongoDB.Options
{
    public class StoreOptions
    {
        public string ConnectionString { get; set; } = "mongodb://localhost/identityserver";
        public string CollectionNamePrefix { get; set; }
        public ReadPreference ReadPreference { get; set; }
    }
}