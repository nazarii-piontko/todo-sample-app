using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ToDo.Backend.Swagger
{
    internal sealed class ApiVersionOperationFilter : IOperationFilter
    {
        private const string VersionParamName = "version";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParam = operation.Parameters?.FirstOrDefault(p =>
                p.Name.Equals(VersionParamName, StringComparison.OrdinalIgnoreCase)
                && p.In == ParameterLocation.Path);

            if (versionParam == null) 
                return;

            var attr = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<ApiVersionAttribute>()
                .FirstOrDefault();
            if (attr == null)
                return;

            var versions = attr.Versions;
            if (versions.Count == 0)
                return;

            versionParam.Description = "API Version";
            versionParam.Schema.Default = new OpenApiString(versions[^1].ToString());
        }
    }
}