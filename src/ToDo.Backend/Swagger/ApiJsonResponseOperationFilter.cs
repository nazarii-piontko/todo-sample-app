using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ToDo.Backend.Swagger
{
    internal sealed class ApiJsonResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor.EndpointMetadata
                .All(o => o.GetType() != typeof(ApiControllerAttribute))) 
                return;
            
            foreach (var response in operation.Responses.Values)
            {
                response.Content.Clear();
                response.Content.Add("application/json", new OpenApiMediaType());
            }
        }
    }
}