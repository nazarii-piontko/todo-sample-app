using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using ToDo.Backend.DTO;
using ToDo.Backend.DTO.Account;
using Xunit;

namespace ToDo.Backend.Tests.Integration
{
    public sealed class AccountTests
    {
        private readonly BackendApplicationFactory _factory;

        public AccountTests()
        {
            _factory = new BackendApplicationFactory();
        }

        [Fact]
        public async Task When_RegisterWithValidParameters_Then_SuccessResponse()
        {
            // Arrange
            var client = _factory.CreateClient();
            var email = AccountOperations.GenerateUserEmail();
            var password = "1234_Qwerty";

            // Act
            var response = await client.RegisterUserAsync(email, password);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [Fact]
        public async Task When_RegisterWithSmallPassword_Then_BadRequestResponse()
        {
            // Arrange
            var client = _factory.CreateClient();
            var email = AccountOperations.GenerateUserEmail();
            var password = "12_Qw";
            
            // Act
            var response = await client.RegisterUserAsync(email, password);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var error = await response.AsTypeAsync<ErrorResponse>();

            error.Errors.Count.Should().Be(1);
            error.Errors.First().Should().Be("Passwords must be at least 6 characters.");
        }
        
        [Fact]
        public async Task When_LoginWithValidParameters_Then_SuccessResponse()
        {
            // Arrange
            var client = _factory.CreateClient();
            var email = AccountOperations.GenerateUserEmail();
            var password = "1234_Qwerty";
            
            await client.RegisterUserAsync(email, password);
            
            // Act
            var response = await client.LoginUserAsync(email, password);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginResponse = await response.AsTypeAsync<LoginResponse>();

            loginResponse.Token.Should().NotBeNullOrEmpty();
            loginResponse.Expires.Should().BeAfter(DateTime.UtcNow);
        }
    }
}