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
    [Route("api/v{version:apiVersion}/to-do-lists")]
    public class ToDoListController : ApiController
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ToDoListController(AppDbContext context, 
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet("")]
        [ProducesResponseType(typeof(List<DTO.ToDo.ToDoList>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetToDoLists()
        {
            var response = await _context.ToDoLists.AsNoTracking()
                .Where(l => l.OwnerId == UserId)
                .ProjectTo<DTO.ToDo.ToDoList>(_mapper.ConfigurationProvider)
                .ToListAsync()
                .ConfigureAwait(false);
            
            return Ok(response);
        }
        
        [HttpGet("{listId:long}")]
        [ProducesResponseType(typeof(DTO.ToDo.ToDoList), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetToDoList([FromRoute] long listId)
        {
            var list = await GetToDoListAsync(listId)
                .ConfigureAwait(false);

            if (list == null)
                return NotFound(new DTO.EmptyResponse());

            var response = _mapper.Map<DTO.ToDo.ToDoList>(list);
            
            return Ok(response);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(DTO.ToDo.ToDoList), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateToDoList([FromBody] DTO.ToDo.CreateToDoListRequest request)
        {
            var list = _mapper.Map<ToDoList>(request);
            list.OwnerId = UserId;

            await _context.ToDoLists.AddAsync(list)
                .ConfigureAwait(false);
            
            await _context.SaveChangesAsync()
                .ConfigureAwait(false);

            var response = _mapper.Map<DTO.ToDo.ToDoList>(list);
            
            return CreatedAtAction(nameof(GetToDoList), 
                new
                {
                    listId = list.Id,
                    version = "1.0"
                }, 
                response);
        }

        [HttpPut("{listId:long}")]
        [ProducesResponseType(typeof(DTO.ToDo.ToDoList), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.EmptyResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditToDoList([FromRoute] long listId,
            [FromBody] DTO.ToDo.EditToDoListRequest request)
        {
            var toDoList = await GetToDoListAsync(listId)
                .ConfigureAwait(false);

            if (toDoList == null)
                return NotFound(new DTO.EmptyResponse());

            _mapper.Map(request, toDoList);

            await _context.SaveChangesAsync()
                .ConfigureAwait(false);

            var response = _mapper.Map<DTO.ToDo.ToDoList>(toDoList);
            
            return Ok(response);
        }

        [HttpDelete("{listId:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteToDoList([FromRoute] long listId)
        {
            var list = await GetToDoListAsync(listId)
                .ConfigureAwait(false);

            if (list == null)
                return NotFound(new DTO.EmptyResponse());

            _context.ToDoLists.Remove(list);
            
            await _context.SaveChangesAsync()
                .ConfigureAwait(false);

            return NoContent();
        }

        private async Task<ToDoList> GetToDoListAsync(long listId)
        {
            var list = await _context.ToDoLists
                .FirstOrDefaultAsync(l => l.Id == listId && l.OwnerId == UserId)
                .ConfigureAwait(false);
            return list;
        }
    }
}