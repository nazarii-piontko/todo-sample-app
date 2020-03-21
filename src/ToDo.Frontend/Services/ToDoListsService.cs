using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ToDo.Backend.DTO;
using ToDo.Backend.DTO.ToDo;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.Services
{
    public sealed class ToDoListsService : IToDoListsService
    {
        private readonly RestClient _restClient;

        public ToDoListsService(RestClient restClient)
        {
            _restClient = restClient;
        }
        
        public Task<List<ToDoList>> GetListsAsync()
        {
            return _restClient.GetAsync<List<ToDoList>>("api/v1.0/to-do-lists");
        }

        public Task<ToDoList> CreateListAsync(string name)
        {
            return _restClient.PostAsync<CreateToDoListRequest, ToDoList>("api/v1.0/to-do-lists", 
                request: new CreateToDoListRequest
                {
                    Name = name
                });
        }

        public Task<ToDoList> EditListAsync(long listId, string name)
        {
            return _restClient.PutAsync<EditToDoListRequest, ToDoList>("api/v1.0/to-do-lists/{listId}",
                urlSegmentParameters: new[] {new RequestParam("listId", listId.ToString(CultureInfo.InvariantCulture))},
                request: new EditToDoListRequest
                {
                    Name = name
                });
        }

        public Task DeleteListAsync(long listId)
        {
            return _restClient.DeleteAsync<EmptyResponse>("api/v1.0/to-do-lists/{listId}",
                urlSegmentParameters: new[] {new RequestParam("listId", listId.ToString(CultureInfo.InvariantCulture))});
        }
    }
}