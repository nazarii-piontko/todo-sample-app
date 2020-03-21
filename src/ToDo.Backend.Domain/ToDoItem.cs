namespace ToDo.Backend.Domain
{
    public sealed class ToDoItem
    {
        public long Id { get; set; }
        
        public string Text { get; set; }

        public ToDoList List { get; set; }
    }
}