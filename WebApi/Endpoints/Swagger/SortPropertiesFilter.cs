using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Endpoints.Swagger;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public class SortPropertiesAttribute(Type type, params string[] excludedProps) : Attribute
{
    public Type Type { get; } = type;
    public string[] ExcludedProps { get; } = excludedProps;
}

public class SortPropertiesFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var attribute = context
            ?.ParameterInfo?.GetCustomAttributes(true)
            .OfType<SortPropertiesAttribute>()
            .FirstOrDefault();

        if (attribute is null)
        {
            return;
        }

        var allowedValues = attribute.Type
            .GetProperties()
            .Where(x => !attribute.ExcludedProps.Contains(x.Name))
            .SelectMany(x => new OpenApiString[] { new($"+{x.Name}"), new($"-{x.Name}") })
            .ToArray();

        if (schema is { Type: "array" })
        {
            schema.Items = new OpenApiSchema { Enum = allowedValues };
        }
        else
        {
            schema.Enum = allowedValues;
        }
    }
}
