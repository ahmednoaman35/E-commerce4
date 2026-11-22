using AutoMapper;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Services;
using Shared;
using Xunit;
namespace E_Commerce.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<UserManager<User>> _userManager;
        private readonly Mock<IMapper> _mapper = new();
        private readonly AuthenticationService _sut;

        public AuthenticationServiceTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManager = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null);

            _sut = new AuthenticationService(
      _userManager.Object,
      Options.Create(new JwtOptions
      {
          SecretKey = "unit-test-secret-key-0123456789-abcdef",
          Issure = "TestIssuer",
          Audience = "TestAudience",
          DurationInDays = 1
      }),
      _mapper.Object);
        }

        [Fact]
        public async Task LoginAsync_EmailNotFound_ThrowsUnAuthorizedException()
        {
            // Arrange
            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                        .ReturnsAsync((User?)null);

            var login = new LoginDTO { Email = "x@test.com", Password = "123" };

            // Act & Assert
            await Assert.ThrowsAsync<UnAuthorizedException>(
                () => _sut.LoginAsync(login));
        }


        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsUnAuthorizedException()
        {
            // Arrange
            var user = new User { Email = "x@test.com", DisplayName = "Ahmed" };

            _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                        .ReturnsAsync(user);

            _userManager.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(false);

            var login = new LoginDTO { Email = "x@test.com", Password = "wrong" };

            // Act & Assert
            await Assert.ThrowsAsync<UnAuthorizedException>(
                () => _sut.LoginAsync(login));
        }


        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUserWithToken()
        {
            // Arrange
            var user = new User
            {
                UserName = "ahmed",
                Email = "x@test.com",
                DisplayName = "Ahmed"
            };

            _userManager.Setup(m => m.FindByEmailAsync("x@test.com"))
                        .ReturnsAsync(user);
            _userManager.Setup(m => m.CheckPasswordAsync(user, "Pass@123"))
                        .ReturnsAsync(true);
            _userManager.Setup(m => m.GetRolesAsync(user))
                        .ReturnsAsync(new List<string> { "Customer" });

            var login = new LoginDTO { Email = "x@test.com", Password = "Pass@123" };

            // Act
            var result = await _sut.LoginAsync(login);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Ahmed", result.DisplayName);
            Assert.Equal("x@test.com", result.Email);
            Assert.Equal(3, result.Token.Split('.').Length);
        }
    }
}
