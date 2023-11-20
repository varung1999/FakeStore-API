using AutoMapper;
using FakeStoreTwo.Data;
using FakeStoreTwo.Models;
using FakeStoreTwo.Services.Interfaces;
using FakeStoreTwo.Services.Repositories;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreTwo.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ProductRepository _repository;
        private readonly IMapper mapper;

        public ProductService(IHttpClientFactory factory, ProductRepository repository)
        {
            _factory = factory;
            _repository = repository;
            mapper = GetMapper();
        }

        private IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductModel, Product>();
                cfg.CreateMap<Product, ProductModel>();
                cfg.CreateMap<RatingModel, Rating>();
                cfg.CreateMap<Rating, RatingModel>();

            });

            return configuration.CreateMapper();
        }

        private async Task<HttpResponseMessage> CommonCode(string url)
        {
            using HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(url);
            if(!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failure to get object. Status : {response.StatusCode}");
            }
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception($"Invalid URL: {url}");
            }
            return response;
        }


        public async Task<bool> DeleteProduct(int id)
        {
            var result = _repository.DeleteProduct(id);
            if(result == true)
            {
                return true;
            }
            return false;
        }

        public async Task<List<ProductModel>> GetAll()
        {
            var results = _repository.GetAll();
            if(results == null || results.Count() == 0)
            {
                var url = $"https://fakestoreapi.com/products";
                HttpResponseMessage response = await CommonCode(url);

                string jsonAsResult = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ProductModel>>(jsonAsResult);
                var add_data = mapper.Map<List<Product>>(result);
                _repository.Add(add_data);
                return result;
            }
            return mapper.Map<List<ProductModel>>(results);
        }

        public async Task<Dictionary<string, double>> GetAvgCountByCategory()
        {
            var results = await _repository.GetAvgCountByCategory();
            return results;
        }

        public async Task<Dictionary<string, double>> GetAvgRateByCategory()
        {
            var results = await _repository.GetAvgRateByCategory();
            return results;
        }

        public async Task<List<ProductModel>> GetByCategory(string categoryName)
        {
            var results = _repository.GetByCategory(categoryName);
            return mapper.Map<List<ProductModel>>(results);
        }

        public async Task<ProductModel> GetById(int id)
        {
            var result = _repository.GetById(id);
            return mapper.Map<ProductModel>(result);
        }

        public async Task<List<String>> GetCategories()
        {
            var result = _repository.GetAllCategories();
            return result;
        }

        public async Task<List<ProductModel>> PaginateProducts(int? page, int? pageSize, string? sortOrder, string? sortField)
        {
            var result = await _repository.PaginateProducts(page, pageSize, sortOrder, sortField);
            return mapper.Map<List<ProductModel>>(result);
        }

        public async Task<List<ProductModel>> PostMultipleProducts(List<ProductModel> products)
        {
            var data = mapper.Map<List<Product>>(products);
            _repository.PostMultiple(data);
            return products;
        }

        public async Task<ProductModel> PostProduct(ProductModel model)
        {
            var data = mapper.Map<Product>(model);
            var maxId = _repository.GetMaxID();
            data.Id = maxId + 1;
            var result = _repository.PostProduct(data);
            return mapper.Map<ProductModel>(result);
        }

        public async Task<bool> PutProduct(int id, ProductModel product)
        {
            var data = mapper.Map<Product>(product);
            var result = _repository.PutProduct(id, data);
            return result;
        }

        public async Task<List<ProductModel>> SearchProducts(string? searchField, string? searchParameter)
        {
            var results = await _repository.SearchProducts(searchField, searchParameter);
            return mapper.Map<List<ProductModel>>(results);
        }
    }
}
