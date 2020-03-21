using System.Collections.Generic;

namespace ToDo.Backend.DTO
{
    public sealed class ErrorResponse
    {
        public ErrorResponse()
        {
        }
        
        public ErrorResponse(string error)
        {
            Errors.Add(error);
        }

        public ErrorResponse(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
        }
        
        public List<string> Errors { get; } = new List<string>();
    }
}