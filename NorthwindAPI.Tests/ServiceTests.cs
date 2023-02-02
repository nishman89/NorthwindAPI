
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindAPI.Tests
{
    public class ServiceTests
    {
        private NorthwindContext _context;
        private ISupplierService _sut;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new DbContextOptionsBuilder<NorthwindContext>()
                .UseInMemoryDatabase(databaseName: "NorthwindDB").Options;
            _context = new NorthwindContext(options);
            _sut = new SupplierService(_context);
            _sut.CreateSupplierAsync(new Supplier { SupplierId = 1, CompanyName = "Sparta Global", City = "Birmingham", Country = "UK", ContactName ="Nish Mandal", ContactTitle = "Manager" }).Wait();
            _sut.CreateSupplierAsync(new Supplier { SupplierId = 2, CompanyName = "Nintendo", City = "Tokyo", Country = "Japan", ContactName = "Shigeru Miyamoto", ContactTitle = "CEO" }).Wait();
        }

        [Test]
        public void GivenValidID_GetSupplierById_ReturnsCorrectSupplier()
        {
            var result = _sut.GetSupplierByIdAsync(1).Result;
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<Supplier>());
            Assert.That(result.CompanyName, Is.EqualTo("Sparta Global"));
        }

        [Test]
        public void GivenAninValidID_GetSupplierById_ReturnsIncorrectSupplier()
        {
            var result = _sut.GetSupplierByIdAsync(1000).Result;
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenANewSupplier_WithANewProduct_CreateSupplierAddsThemToDatabase()
        {
            var newSupplier = new Supplier
            {
                ContactName = "Nish Mandal",
                ContactTitle = "Trainer",
                City = "Birmingham",
                Country = "UK",
                Products = new List<Product> { new Product { ProductName = "C#" } },
                CompanyName = "Sparta Global"
            };
            int numberOfSuppliersBefore = _context.Suppliers.Count();
            int numberofProductsBefore = _context.Products.Count();

            _sut.CreateSupplierAsync(newSupplier).Wait();
            int numberOfSuppliersAfter = _context.Suppliers.Count();
            int numberOfProductsAfter = _context.Products.Count();

            Assert.That(numberOfSuppliersBefore + 1, Is.EqualTo(numberOfSuppliersAfter));
            Assert.That(numberofProductsBefore + 1, Is.EqualTo(numberOfProductsAfter));

            _context.Suppliers.Remove(newSupplier);
        }
        public void GivenASupplier_Removes_RemovesThemFromDatabase_ButNotTheirProduct()
        {
            var newSupplier = new Supplier
            {
                ContactName = "Nish Mandal",
                ContactTitle = "Trainer",
                City = "Birmingham",
                Country = "UK",
                Products = new List<Product> { new Product { ProductName = "C#" } },
                CompanyName = "Sparta Global"
            };


            _sut.CreateSupplierAsync(newSupplier).Wait();
            int numberOfSuppliersBefore = _context.Suppliers.Count();
            int numberofProductsBefore = _context.Suppliers.Count();
            _sut.RemoveSupplierAsync(newSupplier);
            int numberOfSuppliersAfter = _context.Suppliers.Count();
            int numberOfProductsAfter = _context.Suppliers.Count();

            Assert.That(numberOfSuppliersBefore - 1, Is.EqualTo(numberOfSuppliersAfter));
            Assert.That(numberofProductsBefore, Is.EqualTo(numberOfProductsAfter));

            _context.Suppliers.Remove(newSupplier);
        }

        [Test]
        public void GetSuppliers_ReturnsAllSuppliers()
        {
            var suppliers = _sut.GetSuppliers();
            Assert.That(suppliers, Is.InstanceOf<List<Supplier>>());
            Assert.That(suppliers.Count(), Is.EqualTo(2));
        }


    }
}
