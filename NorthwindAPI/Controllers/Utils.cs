using NorthwindAPI.Models;
using NorthwindAPI.Models.DTO;

namespace NorthwindAPI.Controllers
{
    public static class Utils
    {
                public static SupplierDTO SupplierToDTO(Supplier supplier) =>
                 new SupplierDTO
                 {
                     SupplierId = supplier.SupplierId,
                     CompanyName = supplier.CompanyName,
                     ContactName = supplier.ContactName,
                     ContactTitle = supplier.ContactTitle,
                     Country = supplier.Country,
                     //Setting the products property
                     Products = supplier.Products.Select(x => ProductToDTO(x)).ToList()
                 };

        public static ProductDTO ProductToDTO(Product product) =>
                   new ProductDTO
                   {
                       ProductId = product.ProductId,
                       ProductName = product.ProductName,
                       SupplierId = product.SupplierId,
                       CategoryId = product.CategoryId,
                       UnitPrice = product.UnitPrice,

                   };
    }
}
