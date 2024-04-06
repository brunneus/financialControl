//using Microsoft.Extensions.Options;
//using MongoDB.Driver;
//using System.ComponentModel.DataAnnotations;
//using System.Linq.Expressions;
//using FinanceControl.Domain;

//namespace FinanceControl.Data
//{
//    public interface IRepository<T> where T : Entity
//    {
//        Task<int> UpdateAsync(T entity);
//        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
//        Task<int> AddAsync(T entity);
//        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
//        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
//    }

//    public class MongoOptions
//    {
//        [Required]
//        public string ConnectionString { get; set; }
//        public string DatabaseName { get; set; }
//    }

//    public abstract class MongoRepository<T> where T : Entity
//    {
//        private readonly IMongoCollection<T> _collection;
//        private readonly IOptions<MongoOptions> _mongoOptions;

//        public MongoRepository(IOptions<MongoOptions> mongoOptions)
//        {
//            var settings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
//            settings.ConnectTimeout = TimeSpan.FromSeconds(15);

//            var client = new MongoClient(settings);
//            var database = client.GetDatabase("bankAccount");
//            _collection = database.GetCollection<T>(typeof(T).Name);
//            _mongoOptions = mongoOptions;
//        }

//        public async Task<int> AddAsync(T entity)
//        {
//            await _collection.InsertOneAsync(entity);
//            return 1;
//        }

//        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
//        {
//            await _collection.InsertManyAsync(entities);
//            return 1;
//        }

//        public async Task<int> DeleteAsync(T entity)
//        {
//            await _collection.DeleteOneAsync(t => t.Id == entity.Id);
//            return 1;
//        }

//        public async Task<int> DeleteRangeAsync(IEnumerable<T> entities)
//        {
//            foreach (var entity in entities)
//            {
//                await _collection.DeleteOneAsync(t => t.Id == entity.Id);
//            }

//            return 1;
//        }

//        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
//        {
//            return await _collection
//                .Find(predicate)
//                .AnyAsync();
//        }

//        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
//        {
//            return await _collection
//                .Find(predicate)
//                .FirstOrDefaultAsync();
//        }

//        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
//        {
//            return (await _collection.Find(predicate).ToListAsync());
//        }

//        public async Task<T> GetByIdAsync(string id)
//        {
//            return await _collection
//                .Find(t => t.Id == id)
//                .FirstOrDefaultAsync();
//        }

//        public async Task<int> UpdateAsync(T entity)
//        {
//            var result = await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
//            return 1;
//        }

//        public async Task<int> UpdateRangeAsync(IEnumerable<T> entities)
//        {
//            foreach (var entity in entities)
//            {
//                await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
//            }

//            return 1;
//        }
//    }
//}
