using FakeStoreTwo.Data;
using LinqKit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreTwo.Services.Repositories
{
    public class ProductRepository
    {
        private readonly IMongoCollection<Product> _collection;

        public ProductRepository(AppSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Product>("productsTwo");
        }

        public IEnumerable<Product> GetAll() => _collection.AsQueryable().ToList();

        public void Add(List<Product> products) => _collection.InsertManyAsync(products);

        public Product GetById(int id)
        {
            var result = _collection.Find(x=>x.Id == id).FirstOrDefault();
            return result;
        }

        public bool DeleteProduct(int id)
        {
            var result = _collection.DeleteOne(x => x.Id == id);
            if(result.DeletedCount > 0)
            {
                return true;
            }
            return false;
        }

        public List<Product> GetByCategory(string categoryName)
        {
            var filter = Builders<Product>.Filter.Eq(x=>x.Category, categoryName);
            var results = _collection.Find(filter).ToList();
            return results;
        }

        public List<String> GetAllCategories()
        {
            return _collection.Distinct(x=>x.Category,_=>true).ToList();
        }

        public List<Product> PostMultiple(List<Product> products)
        {
             _collection.InsertManyAsync(products);
            return products;
        }

        public Product PostProduct(Product product)
        {
            _collection.InsertOneAsync(product);
            return product;
        }

        public bool PutProduct(int id, Product product)
        {
            var filter = Builders<Product>.Filter.Eq(x=>x.Id,id);
            var result = _collection.ReplaceOne(filter, product);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public int GetMaxID()
        {
            var max = _collection.AsQueryable().Max(x=>x.Id);
            return max;
        }

        public async Task<List<Product>> PaginateProducts(int? page, int? pageSize, string? sortOrder, string? sortField)
        {
            var skip = (page - 1) * pageSize;
            var sort = Builders<Product>.Sort.Ascending(x => x.Id); 

            if (sortField != null)
            {
                if (sortField == "Id")
                {
                    sort = sortOrder == "asc" ? Builders<Product>.Sort.Ascending(x => x.Id) :
                           sortOrder == "desc" ? Builders<Product>.Sort.Descending(x => x.Id) :
                           Builders<Product>.Sort.Ascending(x => x.Id); 
                }
                else if (sortField == "Price")
                {
                    sort = sortOrder == "asc" ? Builders<Product>.Sort.Ascending(x => x.Price) :
                           sortOrder == "desc" ? Builders<Product>.Sort.Descending(x => x.Price) :
                           Builders<Product>.Sort.Ascending(x => x.Price); 
                }
            }

            var result = await _collection.Find(_ => true)
                .Sort(sort)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            return result;
        }

        public async Task<List<Product>> SearchProducts(string? searchField, string? searchParameter)
        {
            var predicate = PredicateBuilder.New<Product>(true);

            if(!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchParameter))
            {
                predicate.And(s=>s.Description.Contains(searchParameter));
                predicate.Or(s => s.Title.Contains(searchParameter));
                predicate.Or(s=>s.Category.Contains(searchParameter));
            }

            var result = await _collection.Find(predicate).ToListAsync();

            var mappedResult = result.Select(product => new Product
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Category = product.Category,
                Rating = new Rating
                {
                    Rate = product.Rating.Rate,
                    Count = product.Rating.Count,
                }
            }).ToList();

            return mappedResult;
        }

        public async Task<Dictionary<string,double>> GetAvgCountByCategory()
        {
            var groupResult = _collection.Aggregate().Group(x=>x.Category,
                g=> new KeyValuePair<string,double>(g.Key,g.Average(x=>x.Rating.Count))).ToList();
            return groupResult.ToDictionary(x=>x.Key,x=>x.Value);
        }

        public async Task<Dictionary<string, double>> GetAvgRateByCategory()
        {
            var groupResult = _collection.Aggregate().Group(x => x.Category,
                g => new KeyValuePair<string, double>(g.Key, (double)g.Average(x => x.Rating.Rate))).ToList();
            return groupResult.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
