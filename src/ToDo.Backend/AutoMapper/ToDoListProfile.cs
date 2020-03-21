using AutoMapper;

namespace ToDo.Backend.AutoMapper
{
    public sealed class ToDoListProfile : Profile
    {
        public ToDoListProfile()
        {
            CreateMap<Domain.ToDoList, DTO.ToDo.ToDoList>();
            CreateMap<DTO.ToDo.CreateToDoListRequest, Domain.ToDoList>();
            CreateMap<DTO.ToDo.EditToDoListRequest, Domain.ToDoList>();
        }
    }
}