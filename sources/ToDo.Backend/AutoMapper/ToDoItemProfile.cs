using AutoMapper;

namespace ToDo.Backend.AutoMapper
{
    public sealed class ToDoItemProfile : Profile
    {
        public ToDoItemProfile()
        {
            CreateMap<Domain.ToDoItem, DTO.ToDo.ToDoItem>();
            CreateMap<DTO.ToDo.CreateToDoItemRequest, Domain.ToDoItem>();
            CreateMap<DTO.ToDo.EditToDoItemRequest, Domain.ToDoItem>();
        }
    }
}