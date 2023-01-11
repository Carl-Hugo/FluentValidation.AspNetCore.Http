using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace FluentValidation.AspNetCore.Http.Tests;

public class FluentValidationEndpointFilterTest : IClassFixture<FluentValidationEndpointFilterTest.TestApp>
{
    private readonly TestApp _app;
    public FluentValidationEndpointFilterTest(TestApp app)
    {
        _app = app ?? throw new ArgumentNullException(nameof(app));
    }

    [Fact]
    public async Task The_endpoint_should_return_BadRequest_when_the_input_is_invalid()
    {
        // Arrange
        var client = _app.CreateClient();
        var body = JsonContent.Create(new MyEntity { Id = 123 });

        // Act
        var result = await client.PostAsync("/", body);

        // Assert
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task The_endpoint_should_return_BadRequest_when_the_input_is_null()
    {
        // Arrange
        var client = _app.CreateClient();
        var body = JsonContent.Create(null, typeof(object));

        // Act
        var result = await client.PostAsync("/", body);

        // Assert
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task The_endpoint_should_return_Ok_when_the_input_is_valid()
    {
        // Arrange
        var client = _app.CreateClient();
        var body = JsonContent.Create(new MyEntity { Id = 123, Name = "Some name" });

        // Act
        var result = await client.PostAsync("/", body);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    public class MyEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class MyEntityValidator : AbstractValidator<MyEntity>
    {
        public MyEntityValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class TestApp : MinimalHostingTestApp
    {
        public override void ConfigureFluentValidationEndpointFilterSettings(FluentValidationEndpointFilterSettings settings)
        {
            settings.ScanningStrategy = ScanningStrategy.ScanUntilNoValidatorFound;
        }

        public override void ConfigureWebApplication(WebApplication app)
        {
            app
                .MapPost("/", (MyEntity entity, CancellationToken cancellationToken) =>
                {
                    return HttpResults.Ok(entity);
                })
                .AddEndpointFilter<FluentValidationEndpointFilter>()
            ;
        }

        public override void ConfigureWebApplicationBuilder(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IValidator<MyEntity>, MyEntityValidator>();
        }
    }
}
