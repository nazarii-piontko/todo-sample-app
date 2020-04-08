using System.Collections.Generic;
using System.Threading.Tasks;
using ToDo.Backend.DTO.ToDo;

namespace ToDo.Frontend.Services.Abstractions
{
    public interface IToDoListsService
    {
        Task<List<ToDoList>> GetListsAsync();

        Task<ToDoList> CreateListAsync(string name);
        
        Task<ToDoList> EditListAsync(long listId, string name);
        
        Task DeleteListAsync(long listId);
    }
}