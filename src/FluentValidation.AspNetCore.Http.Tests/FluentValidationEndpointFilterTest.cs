using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace FluentValidation.AspNetCore.Http.Tests;

public class FluentValidationEndpointFilterTest
{
    public class SimpleTest : FluentValidationEndpointFilterTest
    {
        private readonly FluentValidationEndpointFilterTestApp _app = new(
            configureBuilder: builder => builder.Services
                .AddScoped<IValidator<MyEntity>, MyEntityValidator>(),
            configureApp: app => app
                .MapPost("/", (MyEntity entity, CancellationToken cancellationToken) =>
                {
                    return HttpResults.Ok(entity);
                })
                .AddEndpointFilter<FluentValidationEndpointFilter>(),
            configureFilter: settings => settings
                .ScanningStrategy = ScanningStrategy.ScanAllParams
        );

        [Fact]
        public async Task The_endpoint_should_return_BadRequest_when_the_input_is_invalid()
        {
            // Arrange
            var client = _app.CreateClient();
            var body = JsonContent.Create(new MyEntity(default));

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
            var body = JsonContent.Create(new MyEntity("Some name"));

            // Act
            var result = await client.PostAsync("/", body);

            // Assert
            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }

    public class ScanningStrategyTest : FluentValidationEndpointFilterTest
    {
        public class ScanAllParams : ScanningStrategyTest
        {
            [Fact]
            public async Task Should_validate_all_parameters()
            {
                // Arrange
                var app = new FluentValidationEndpointFilterTestApp(
                    configureBuilder: builder => builder.Services
                        .AddScoped<IValidator<MyEntity>, MyEntityValidator>(),
                    configureApp: app => app
                        .MapPost("/{id}", (string id, MyEntity entity) =>
                        {
                            return HttpResults.Ok(entity);
                        })
                        .AddEndpointFilter<FluentValidationEndpointFilter>(),
                    configureFilter: settings => settings
                        .ScanningStrategy = ScanningStrategy.ScanAllParams
                );
                var client = app.CreateClient();
                var body = JsonContent.Create(new MyEntity(default));

                // Act
                var result = await client.PostAsync("/asdf", body);

                // Assert
                Assert.False(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            }
        }

        public class ScanUntilNoValidatorFound : ScanningStrategyTest
        {
            [Fact]
            public async Task Should_not_validate_the_last_param()
            {
                // Arrange
                var app = new FluentValidationEndpointFilterTestApp(
                    configureBuilder: builder => builder.Services
                        .AddScoped<IValidator<MyEntity>, MyEntityValidator>(),
                    configureApp: app => app
                        .MapPost("/{id}", (string id, MyEntity entity) =>
                        {
                            return HttpResults.Ok(entity);
                        })
                        .AddEndpointFilter<FluentValidationEndpointFilter>(),
                    configureFilter: settings => settings
                        .ScanningStrategy = ScanningStrategy.ScanUntilNoValidatorFound
                );
                var client = app.CreateClient();
                var body = JsonContent.Create(new MyEntity(default));

                // Act
                var result = await client.PostAsync("/asdf", body);

                // Assert
                Assert.True(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

        }
    }

    public record class MyEntity(string? Name);
    public class MyEntityValidator : AbstractValidator<MyEntity>
    {
        public MyEntityValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}