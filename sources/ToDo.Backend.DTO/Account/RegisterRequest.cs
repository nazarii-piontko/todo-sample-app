namespace ToDo.Backend.DTO.Account
{
    public sealed class RegisterRequest
    {
        public string Email { get; set; }
        
        public string Password { get; set; }
    }
}