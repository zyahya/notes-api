namespace Notes.Api.OpenApi;

using Microsoft.AspNetCore.OpenApi;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.OpenApi;

public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // 1. Ensure components are initialized
        document.Components ??= new OpenApiComponents();

        // 2. Define the Bearer Security Scheme
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token to access secured endpoints."
        };

        // 3. Add the scheme to the document's components
        document.AddComponent("Bearer", bearerScheme);

        // 4. Create a security requirement referencing the scheme using .NET 10 syntax
        var securityRequirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        };

        // 5. Apply the security requirement to all operations globally
        if (document.Paths != null)
        {
            foreach (var operation in document.Paths.Values.SelectMany(p => p.Operations))
            {
                operation.Value.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Value.Security.Add(securityRequirement);
            }
        }

        return Task.CompletedTask;
    }
}
