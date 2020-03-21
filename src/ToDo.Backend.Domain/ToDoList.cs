namespace ToDo.Backend.Domain
{
    public sealed class ToDoList
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long OwnerId { get; set; }
    }
}