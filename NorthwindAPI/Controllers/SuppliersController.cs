using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindAPI.Models;
using NorthwindAPI.Models.DTO;
using NorthwindAPI.Services;

namespace NorthwindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;
        private readonly ILogger _logger;


        public SuppliersController(ILogger<SuppliersController> logger, ISupplierService service)
        {
            _logger = logger;
            _service = service;
        }

        // GET: api/Suppliers
        [HttpGet]
        public IEnumerable<SupplierDTO> GetSuppliers()
        {
            var suppliers = _service.GetSuppliers().Select(x => Utils.SupplierToDTO(x)).ToList();
            return suppliers;
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplier(int id)
        {
            if (!SupplierExists(id))
            {
                _logger.LogWarning($"No supplier with {id} not found");
                return NotFound();
            }
            _logger.LogInformation($"Supplier {id} Exists");

            var supplier = await _service.GetSupplierByIdAsync(id);
            return Utils.SupplierToDTO(supplier);
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetSupplierWithProducts(int id)
        {
            if (!SupplierExists(id))
            {
                return NotFound();
            }
            var products = await _service.GetProductsBySupplierIdAsync(id);
            return products.Select(p => Utils.ProductToDTO(p)).ToList();
        }
        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, SupplierDTO supplierDto)
        {
            //The id in the URI has to match the URI in the JSON request body we send

            if (id != supplierDto.SupplierId)
            {
                return BadRequest();
            }

            Supplier supplier = await _service.GetSupplierByIdAsync(id);


            //Null-coalescing oeprator returns the value of it's left hand 
            //operand if it isn't null.
            supplier.CompanyName = supplierDto.CompanyName ?? supplier.CompanyName;
            supplier.ContactName = supplierDto.ContactName ?? supplier.ContactName;
            supplier.ContactTitle = supplierDto.ContactTitle ?? supplier.ContactTitle;
            supplier.Country = supplierDto.Country ?? supplier.Country;


            try
            {
                await _service.SaveSupplierChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SupplierDTO>> PostSupplier(SupplierDTO supplierDto)
        {
            List<Product> products = new List<Product>();

            supplierDto.Products.ToList().ForEach(x => products.Add(new Product() { ProductName = x.ProductName, UnitPrice = x.UnitPrice }));
            await _service.AddProductsAsync(products);
            Supplier supplier = new Supplier
            {
                SupplierId = supplierDto.SupplierId,
                CompanyName = supplierDto.CompanyName,
                ContactName = supplierDto.ContactName,
                ContactTitle = supplierDto.ContactTitle,
                Country = supplierDto.Country,
                Products = products
            };

            await _service.CreateSupplierAsync(supplier);
            //Update IDs of DTOs
            supplierDto = _service.GetSuppliers()
                .Where(s => s.SupplierId == supplier.SupplierId)
                .Select(x => Utils.SupplierToDTO(x)).FirstOrDefault();


            return CreatedAtAction("GetSupplier", new { id = supplier.SupplierId }, supplierDto);
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _service.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            var supplierProducts = await _service.GetProductsBySupplierIdAsync(id);
            foreach (var prod in supplierProducts)
            {
                prod.Supplier = null;
            }
            await _service.RemoveSupplierAsync(supplier);
            return NoContent();
        }

        private bool SupplierExists(int id)
        {
            return _service.SupplierExists(id);
        }
    }
}
