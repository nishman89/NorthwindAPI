using Microsoft.EntityFrameworkCore;
using NorthwindAPI.Models;

namespace NorthwindAPI.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly NorthwindContext _context;
        public SupplierService(NorthwindContext context)
        {
            _context = context;
        }
        public async Task CreateSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task CreateSuppliersAsync(IEnumerable<Supplier> suppliers)
        {
            _context.Suppliers.AddRange(suppliers);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetProductsBySupplierIdAsync(int id)
        {
            return await _context.Products.Where(p => p.SupplierId == id).ToListAsync();
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers.Include(s => s.Products).Where(x => x.SupplierId == id).FirstOrDefaultAsync();
        }

        public List<Supplier> GetSuppliers()
        {
            return _context.Suppliers.Include(s => s.Products).ToList();
        }

        public async Task RemoveSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task SaveSupplierChangesAsync()
        {
            await _context.SaveChangesAsync(); 
        }

        public bool SupplierExists(int id)
        {
            return (bool)(_context.Suppliers?.Any(e => e.SupplierId == id)); 
        }

        public async Task AddProductsAsync(IEnumerable<Product> products)
        {
            await  _context.Products.AddRangeAsync();
            await _context.SaveChangesAsync();
        }

    }
}
