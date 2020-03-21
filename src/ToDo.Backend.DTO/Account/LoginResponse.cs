using System;

namespace ToDo.Backend.DTO.Account
{
    public sealed class LoginResponse
    {
        public string Token { get; set; }
        
        public DateTime Expires { get; set; }
    }
}