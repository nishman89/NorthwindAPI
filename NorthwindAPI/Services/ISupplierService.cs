using NorthwindAPI.Models;

namespace NorthwindAPI.Services
{
    public interface ISupplierService
    {
        public List<Supplier> GetSuppliers();
        public Task CreateSupplierAsync(Supplier supplier);
        public Task RemoveSupplierAsync(Supplier supplier);
        public Task<Supplier> GetSupplierByIdAsync(int id);
        public Task SaveSupplierChangesAsync();
        public bool SupplierExists(int id);
        public Task<List<Product>> GetProductsBySupplierIdAsync(int id);
        public Task CreateSuppliersAsync(IEnumerable<Supplier> suppliers);
        public Task AddProductsAsync(IEnumerable<Product> products);

    }
}
