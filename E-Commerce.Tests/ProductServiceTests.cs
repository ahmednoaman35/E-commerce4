using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using Services;
using Xunit;
namespace E_Commerce.Tests
{
    public class ProductServiceTests
    {

        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IGenericRepository<Product, int>> _productRepo = new();
        private readonly ProductService _sut;

        public ProductServiceTests()
        {
            _unitOfWork.Setup(u => u.GetRepository<Product, int>())
                       .Returns(_productRepo.Object);

            _sut = new ProductService(_unitOfWork.Object, _mapper.Object);
        }

        [Fact]
        public async Task GetProductById_ProductNotFound_ThrowsNotFoundException()
        {
            _productRepo.Setup(r => r.GetAsync(It.IsAny<Specifications<Product>>()))
                    .ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(
                () => _sut.GetProductByIdAsync(99));
        }
    }
}
    