using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer4.MongoDB.Repositories
{
    /// <summary>
    /// Abstract Entity for all the BusinessEntities.
    /// </summary>
    [BsonIgnoreExtraElements(Inherited = true)]
    public class Entity : IEntity<ObjectId>, IEntity
    {
        /// <summary>
        /// Gets or sets the id for this object (the primary record for an entity).
        /// </summary>
        /// <value>The id for this object (the primary record for an entity).</value>
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual ObjectId Id { get; set; }
    }
}