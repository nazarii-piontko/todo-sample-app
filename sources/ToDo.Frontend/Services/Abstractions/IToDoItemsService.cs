using System.Collections.Generic;
using System.Threading.Tasks;
using ToDo.Backend.DTO.ToDo;

namespace ToDo.Frontend.Services.Abstractions
{
    public interface IToDoItemsService
    {
        Task<List<ToDoItem>> GetItemsAsync(long listId);

        Task<ToDoItem> CreateItemAsync(long listId, string name);
        
        Task<ToDoItem> EditItemAsync(long listId, long itemId, string name);
        
        Task DeleteItemAsync(long listId, long itemId);
    }
}