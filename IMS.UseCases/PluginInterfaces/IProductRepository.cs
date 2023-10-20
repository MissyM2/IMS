using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
        Task AddProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(int productId);
        Task UpdateProductAsync(Product product);
    }
}
