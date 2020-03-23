using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using ToDo.Backend.DTO.ToDo;
using Xunit;

namespace ToDo.Backend.Tests.Integration
{
    public sealed class ToDoListsTests
    {
        private readonly BackendApplicationFactory _factory;

        public ToDoListsTests()
        {
            _factory = new BackendApplicationFactory();
        }

        [Fact]
        public async Task When_RequestListsOnFreshAccount_Then_ListIsEmpty()
        {
            // Arrange
            var client = await CreateClient();

            // Act
            var response = await client.GetAsync("api/v1.0/to-do-lists");

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var lists = await response.AsTypeAsync<List<ToDoList>>();
            
            lists.Count.Should().Be(0);
        }
        
        [Fact]
        public async Task When_CreateNewList_Then_CreatedListEntityIsReturned()
        {
            // Arrange
            var client = await CreateClient();

            // Act
            var response = await client.PostAsync("api/v1.0/to-do-lists",
                    new CreateToDoListRequest
                    {
                        Name = "List #1"
                    }.AsJsonContent());

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var list = await response.AsTypeAsync<ToDoList>();

            list.Id.Should().BePositive();
            list.Name.Should().Be("List #1");
        }
        
        [Fact]
        public async Task When_CreateNewList_Then_CreatedListShouldBeReturnedOnQuery()
        {
            // Arrange
            var client = await CreateClient();

            // Act
            await client.PostAsync("api/v1.0/to-do-lists",
                    new CreateToDoListRequest
                    {
                        Name = "List #1"
                    }.AsJsonContent());
            
            var response = await client.GetAsync("api/v1.0/to-do-lists");

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var lists = await response.AsTypeAsync<List<ToDoList>>();
            
            lists.Count.Should().Be(1);
            
            lists.First().Id.Should().BePositive();
            lists.First().Name.Should().Be("List #1");
        }

        private async Task<HttpClient> CreateClient()
        {
            var client = _factory.CreateClient();
            
            var token = await client.GetUserTokenAsync();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            return client;
        }
    }
}