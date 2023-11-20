using FakeStoreTwo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreTwo.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductModel>> GetAll();
        Task<ProductModel> GetById(int id);
        Task<List<ProductModel>> GetByCategory(string categoryName);
        Task<List<String>> GetCategories();
        Task<ProductModel> PostProduct(ProductModel model);
        Task<List<ProductModel>> PostMultipleProducts(List<ProductModel> products);
        Task<bool> PutProduct(int id, ProductModel product);
        Task<bool> DeleteProduct(int id);
        Task<List<ProductModel>> PaginateProducts(int? page, int? pageSize, string? sortOrder, string? sortField);
        Task<List<ProductModel>> SearchProducts(string? searchField, string? searchParameter);
        Task<Dictionary<string, double>> GetAvgRateByCategory();
        Task<Dictionary<string, double>> GetAvgCountByCategory();



    }
}
