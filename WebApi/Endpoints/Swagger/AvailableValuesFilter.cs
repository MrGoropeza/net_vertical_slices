using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Endpoints.Swagger;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = true)]
public class AvailableValuesAttribute(params string[] values) : Attribute
{
    public string[] Values { get; } = values;
}

public class AvailableValuesFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var attributes = context
            ?.ParameterInfo?.GetCustomAttributes(true)
            .OfType<AvailableValuesAttribute>()
            .ToList();

        if (attributes is null)
        {
            return;
        }

        var allowedValues = attributes
            .SelectMany(r => r.Values)
            .Select(x => new OpenApiString(x.ToString()))
            .ToArray();

        if (schema.Type is "array")
        {
            schema.Items = new OpenApiSchema { Enum = allowedValues };
            return;
        }

        schema.Enum = allowedValues;
    }
}
