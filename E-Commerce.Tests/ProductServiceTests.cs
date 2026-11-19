using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using Services;
using Shared;
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

        [Fact]
        public async Task GetProductByIdAsync_ProductExists_ReturnsMappedDto()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product" };
            var dto = new ProductResultDTO { Id = 1, Name = "Test Product" };

            _productRepo.Setup(r => r.GetAsync(It.IsAny<Specifications<Product>>()))
                        .ReturnsAsync(product);

            _mapper.Setup(m => m.Map<ProductResultDTO?>(product))
                   .Returns(dto);

            // Act
            var result = await _sut.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsPaginatedResultWithCorrectCounts()
        {

            // Arrange
            var products = new List<Product>
    {
        new Product { Id = 1, Name = "P1" },
        new Product { Id = 2, Name = "P2" }
    };
            var dtos = new List<ProductResultDTO>
    {
        new ProductResultDTO { Id = 1, Name = "P1" },
        new ProductResultDTO { Id = 2, Name = "P2" }
    };

            _productRepo.Setup(r => r.GetAllAsync(It.IsAny<Specifications<Product>>()))
                        .ReturnsAsync(products);

            _productRepo.Setup(r => r.CountAsync(It.IsAny<Specifications<Product>>()))
                        .ReturnsAsync(10);

            _mapper.Setup(m => m.Map<IEnumerable<ProductResultDTO>>(products))
                   .Returns(dtos);

            var parameters = new ProductSpecificationsParameters { pageIndex = 1 };

            // Act
            var result = await _sut.GetAllProductsAsync(parameters);

            // Assert
            Assert.Equal(1, result.PageIndex);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(10, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetAllProductsAsync_NoProducts_ReturnsEmptyPaginatedResult()
        {
            // Arrange

            var products = new List<Product>();
           
            var dtos = new List<ProductResultDTO>();

            _productRepo.Setup(r => r.GetAllAsync(It.IsAny<Specifications<Product>>()))
                        .ReturnsAsync(products);
            _productRepo.Setup(r => r.CountAsync(It.IsAny<Specifications<Product>>()))
                        .ReturnsAsync(0);

            _mapper.Setup(m => m.Map<IEnumerable<ProductResultDTO>>(products))

                          .Returns(dtos);

            var parameters = new ProductSpecificationsParameters { pageIndex = 1 };
            // Act
            var result = await _sut.GetAllProductsAsync(parameters);

            // Assert

            Assert.Equal(1, result.PageIndex);
            Assert.Equal(0, result.PageSize);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

    }
}

        
    