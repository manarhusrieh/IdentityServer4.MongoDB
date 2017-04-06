using System;
using MongoDB.Driver;

namespace IdentityServer4.MongoDB.Repositories
{
    public abstract class MongoRepositoryBase<T>
    {
        /// <summary>
        /// Lazy creation of mongo collection
        /// </summary>
        private readonly Lazy<IMongoCollection<T>> _lazyMongoCollection;

        protected MongoRepositoryBase()
        {
            _lazyMongoCollection = new Lazy<IMongoCollection<T>>(() =>
            {
                var mongoUrl = new MongoUrl(ConnectionString);
                var mongoClient = new MongoClient(mongoUrl);
                var database = mongoClient.GetDatabase(mongoUrl.DatabaseName, ReadPreferenceValue != null ? new MongoDatabaseSettings {ReadPreference = ReadPreferenceValue} : null);
                return database.GetCollection<T>(CollectionName, ReadPreferenceValue != null ? new MongoCollectionSettings {ReadPreference = ReadPreferenceValue} : null);
            }, true);
        }

        /// <summary>
        /// MongoCollection field.
        /// </summary>
        protected IMongoCollection<T> MongoCollection => _lazyMongoCollection.Value;

        /// <summary>
        /// Read preference.
        /// </summary>
        protected abstract ReadPreference ReadPreferenceValue { get; }

        /// <summary>
        /// Connection string.
        /// </summary>
        protected abstract string ConnectionString { get; }

        /// <summary>
        /// Determines the collectionname for T and assures it is not empty
        /// </summary>
        protected abstract string CollectionName { get; }
    }
}