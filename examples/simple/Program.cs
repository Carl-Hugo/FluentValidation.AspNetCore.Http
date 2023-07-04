using FluentValidation;
using FluentValidation.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.AddFluentValidationEndpointFilter();
builder.Services.AddScoped<IValidator<MyEntity>, MyEntityValidator>();

var app = builder.Build();
app
    .MapPost("/", (MyEntity entity, CancellationToken cancellationToken)
        => TypedResults.Ok(entity))
    .AddEndpointFilter<FluentValidationEndpointFilter>()
;
app.Run();

public record class MyEntity(string? Name);
public class MyEntityValidator : AbstractValidator<MyEntity>
{
    public MyEntityValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
