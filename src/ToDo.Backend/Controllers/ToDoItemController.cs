using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Backend.Domain;
using ToDo.Backend.Persistence;

namespace ToDo.Backend.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/to-do-lists/{listId:long}/items")]
    public class ToDoItemController : ApiController
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ToDoItemController(AppDbContext context, 
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet("")]
        [ProducesResponseType(typeof(List<DTO.ToDo.ToDoItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetToDoItems([FromRoute] long listId)
        {
            var response = await _context.ToDoItems.Include(t => t.List).AsNoTracking()
                .Where(l => l.List.Id == listId && l.List.OwnerId == UserId)
                .ProjectTo<DTO.ToDo.ToDoItem>(_mapper.ConfigurationProvider)
                .ToListAsync()
                .ConfigureAwait(false);
            
            return Ok(response);
        }
        
        [HttpGet("{itemId:long}")]
        [ProducesResponseType(typeof(DTO.ToDo.ToDoItem), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetToDoItem([FromRoute] long listId,
            [FromRoute] long itemId)
        {
            var item = await GetToDoItemAsync(listId, itemId)
                .ConfigureAwait(false);

            if (item == null)
                return NotFound(new DTO.EmptyResponse());

            var response = _mapper.Map<DTO.ToDo.ToDoItem>(item);
            
            return Ok(response);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(DTO.ToDo.ToDoItem), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.EmptyResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateToDoItem([FromRoute] long listId, 
            [FromBody] DTO.ToDo.CreateToDoItemRequest request)
        {
            var toDoList = await _context.ToDoLists.FirstOrDefaultAsync(l => l.Id == listId && l.OwnerId == UserId)
                .ConfigureAwait(false);

            if (toDoList == null)
                return BadRequest(new DTO.EmptyResponse());
            
            var toDoItem = _mapper.Map<ToDoItem>(request);
            toDoItem.List = toDoList;

            await _context.ToDoItems.AddAsync(toDoItem)
                .ConfigureAwait(false);
            
            await _context.SaveChangesAsync()
                .ConfigureAwait(false);

            var response = _mapper.Map<DTO.ToDo.ToDoItem>(toDoItem);
            
            return CreatedAtAction(nameof(GetToDoItem), 
                new
                {
                    listId = toDoList.Id,
                    itemId = toDoItem.Id,
                    version = "1.0"
                }, 
                response);
        }

        [HttpPut("{itemId:long}")]
        [ProducesResponseType(typeof(DTO.ToDo.ToDoItem), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.EmptyResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditToDoItem([FromRoute] long listId,
            [FromRoute] long itemId,
            [FromBody] DTO.ToDo.EditToDoItemRequest request)
        {
            var toDoItem = await GetToDoItemAsync(listId, itemId)
                .ConfigureAwait(false);

            if (toDoItem == null)
                return NotFound(new DTO.EmptyResponse());

            _mapper.Map(request, toDoItem);

            await _context.SaveChangesAsync()
                .ConfigureAwait(false);

            var response = _mapper.Map<DTO.ToDo.ToDoItem>(toDoItem);
            
            return Ok(response);
        }

        [HttpDelete("{itemId:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteToDoItem([FromRoute] long listId,
            [FromRoute] long itemId)
        {
            var toDoItem = await GetToDoItemAsync(listId, itemId)
                .ConfigureAwait(false);

            if (toDoItem == null)
                return NotFound(new DTO.EmptyResponse());

            _context.ToDoItems.Remove(toDoItem);
            
            await _context.SaveChangesAsync()
                .ConfigureAwait(false);

            return NoContent();
        }

        private async Task<ToDoItem> GetToDoItemAsync(long listId, long itemId)
        {
            var toDoItem = await _context.ToDoItems.Include(t => t.List)
                .FirstOrDefaultAsync(l => l.Id == itemId && l.List.Id == listId && l.List.OwnerId == UserId)
                .ConfigureAwait(false);
            return toDoItem;
        }
    }
}