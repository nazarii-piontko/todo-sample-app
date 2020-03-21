using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ToDo.Backend.DTO;
using ToDo.Backend.DTO.ToDo;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.Services
{
    public sealed class ToDoItemsService : IToDoItemsService
    {
        private readonly RestClient _restClient;

        public ToDoItemsService(RestClient restClient)
        {
            _restClient = restClient;
        }
        
        public Task<List<ToDoItem>> GetItemsAsync(long listId)
        {
            return _restClient.GetAsync<List<ToDoItem>>("api/v1.0/to-do-lists/{listId}/items",
                urlSegmentParameters: new[]
                {
                    new RequestParam("listId", listId.ToString(CultureInfo.InvariantCulture))
                });
        }

        public Task<ToDoItem> CreateItemAsync(long listId, string name)
        {
            return _restClient.PostAsync<CreateToDoItemRequest, ToDoItem>("api/v1.0/to-do-lists/{listId}/items", 
                urlSegmentParameters: new[]
                {
                    new RequestParam("listId", listId.ToString(CultureInfo.InvariantCulture))
                },
                request: new CreateToDoItemRequest
                {
                    Text = name
                });
        }

        public Task<ToDoItem> EditItemAsync(long listId, long itemId, string name)
        {
            return _restClient.PutAsync<EditToDoItemRequest, ToDoItem>("api/v1.0/to-do-lists/{listId}/items/{itemId}",
                urlSegmentParameters: new[]
                {
                    new RequestParam("listId", listId.ToString(CultureInfo.InvariantCulture)),
                    new RequestParam("itemId", itemId.ToString(CultureInfo.InvariantCulture))
                },
                request: new EditToDoItemRequest
                {
                    Text = name
                });
        }

        public Task DeleteItemAsync(long listId, long itemId)
        {
            return _restClient.DeleteAsync<EmptyResponse>("api/v1.0/to-do-lists/{listId}/items/{itemId}",
                urlSegmentParameters: new[]
                {
                    new RequestParam("listId", listId.ToString(CultureInfo.InvariantCulture)),
                    new RequestParam("itemId", itemId.ToString(CultureInfo.InvariantCulture))
                });
        }
    }
}