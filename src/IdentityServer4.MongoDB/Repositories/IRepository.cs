using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IdentityServer4.MongoDB.Repositories
{
    public interface IRepository<T, in TKey> where T : IEntity<TKey>
    {
        IMongoCollection<T> Collection { get; }

        IMongoQueryable<T> AsQueryable();

        Task<T> GetAsync(TKey id);

        Task<T> AddAsync(T entity);
        Task AddAsync(IEnumerable<T> entities);

        Task<T> UpdateAsync(T entity);

        Task DeleteAsync(TKey id);
        Task DeleteAsync(T entity);
        Task DeleteAsync(Expression<Func<T, bool>> predicate);

        Task<long> CountAsync();
        Task<long> CountAsync(Expression<Func<T, bool>> predicate);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }

    public interface IRepository<T> : IRepository<T, ObjectId> where T : IEntity<ObjectId>
    {
    }
}