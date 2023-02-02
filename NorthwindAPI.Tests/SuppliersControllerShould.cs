using Microsoft.Extensions.Logging;
using NorthwindAPI.Controllers;
using NorthwindAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NorthwindAPI.Tests
{
    public class Tests
    {
        private SuppliersController? _sut;

        [Test]
        public void BeAbleToBeConstructed()
        {
            var mockService = new Mock<ISupplierService>();
            var mockLogger = new Mock<ILogger<SuppliersController>>();
            _sut = new SuppliersController(mockLogger.Object, mockService.Object);
            Assert.That(_sut, Is.InstanceOf<SuppliersController>());
        }

        [Category("Happy Path")]
        [Category("GetSuppliers")]
        [Test]
        public void ReturnsListOfSupplierDTOs_WhenGetSuppliersIsCalled()
        {
            var mockService = new Mock<ISupplierService>();
            var mockLogger = new Mock<ILogger<SuppliersController>>();
            mockService.Setup(sc => sc.GetSuppliers()).Returns(new List<Supplier>() {new Supplier(), new Supplier() });
            _sut = new SuppliersController(mockLogger.Object, mockService.Object);
            var result = _sut.GetSuppliers();
            Assert.That(result, Is.InstanceOf<IEnumerable<SupplierDTO>>());
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Category("Happy Path")]
        [Category("GetSupplier")]
        [Test]
        public async Task ReturnsCorrectSupplierDTO_WhenGetSupplierIsCalled_WithValidIdAsync()
        {
            var mockService = new Mock<ISupplierService>();
            var mockLogger = new Mock<ILogger<SuppliersController>>();
            mockService.Setup(sc => sc.GetSupplierByIdAsync(1)).ReturnsAsync(new Supplier { ContactName = "Nish", CompanyName = "Sparta Global" });
            mockService.Setup(sc => sc.SupplierExists(1)).Returns(true);
            _sut = new SuppliersController(mockLogger.Object, mockService.Object);
            var result = await _sut.GetSupplier(1);
            Assert.That(result.Value, Is.InstanceOf<SupplierDTO>());
            Assert.That(result.Value.ContactName, Is.EqualTo("Nish"));
            Assert.That(result.Value.CompanyName, Is.EqualTo("Sparta Global"));
        }

        [Category("Sad Path")]
        [Category("GetSupplier")]
        [Test]
        public async Task ReturnsNotFound_WhenGetSupplierIsCalled_WithInvalidIdAsync()
        {
            var mockService = new Mock<ISupplierService>();
            var mockLogger = new Mock<ILogger<SuppliersController>>();
            mockService.Setup(sc => sc.SupplierExists(1)).Returns(false);
            _sut = new SuppliersController(mockLogger.Object, mockService.Object);
            var result = await _sut.GetSupplier(1);
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Category("Sad Path")]
        [Category("PutSuplier")]
        [Test]
        public async Task ReturnsBadRequest_WhenPutSupplierIsCalled_WithInValidIdAsync()
        {
            var mockService = new Mock<ISupplierService>();
            var mockLogger = new Mock<ILogger<SuppliersController>>();
            mockService.Setup(sc => sc.GetSupplierByIdAsync(1).Result).Returns(new Supplier { ContactName = "Nish", CompanyName = "Sparta Global" });
            mockService.Setup(sc => sc.SupplierExists(1)).Returns(false);
            _sut = new SuppliersController(mockLogger.Object, mockService.Object);
            var result = await _sut.PutSupplier(1, new SupplierDTO { ContactName = "Nish Kumar" });
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Category("Happy Path")]
        [Category("PutSuplier")]
        [Test]
        public async Task ReturnsNotContent_WhenPutSupplierIsCalled_WithValidIdAsync()
        {
            var mockService = new Mock<ISupplierService>();
            var mockLogger = new Mock<ILogger<SuppliersController>>();
            mockService.Setup(sc => sc.GetSupplierByIdAsync(1).Result).Returns(new Supplier { ContactName = "Nish", CompanyName = "Sparta Global" });
            mockService.Setup(sc => sc.SupplierExists(1)).Returns(true);
            _sut = new SuppliersController(mockLogger.Object, mockService.Object);
            var result = await _sut.PutSupplier(1, new SupplierDTO { SupplierId = 1, ContactName = "Nish Kumar" });
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
