using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using System.Diagnostics.CodeAnalysis;

namespace FluentValidation.AspNetCore.Http.Tests;

public class MinimalHostingTestApp : IDisposable, IAsyncDisposable
{
    private WebApplication? _webApplication;
    private TestServer? _testServer;
    private HttpClient? _client;
    private readonly Action<WebApplicationBuilder>? _configureBuilder;
    private readonly Action<WebApplication>? _configureApp;

    public MinimalHostingTestApp(
        Action<WebApplicationBuilder>? configureBuilder = default,
        Action<WebApplication>? configureApp = default)
    {
        _configureBuilder = configureBuilder;
        _configureApp = configureApp;
    }

    public TestServer Server
    {
        get
        {
            EnsureTestServer();
            return _testServer;
        }
    }

    public virtual IServiceProvider Services
    {
        get
        {
            EnsureTestServer();
            return _testServer.Services;
        }
    }

    [MemberNotNull(nameof(_testServer))]
    [MemberNotNull(nameof(_webApplication))]
    protected virtual async void EnsureTestServer()
    {
        if (_testServer != null)
        {
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
            return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.
        }

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Environment.EnvironmentName = "Testing";
        _configureBuilder?.Invoke(builder);

        var app = _webApplication = builder.Build();
        _configureApp?.Invoke(app);

        var task = _webApplication.RunAsync();
        _testServer = _webApplication.GetTestServer();
        await task;
    }

    public virtual HttpClient CreateClient()
    {
        if (_client is not null)
        {
            return _client;
        }

        EnsureTestServer();
        _client = _testServer.CreateClient();
        return _client;
    }

    #region Disposable
    private bool _disposedAsync;

    public virtual async ValueTask DisposeAsync()
    {
        if (_disposedAsync)
        {
            return;
        }

        _client?.Dispose();
        _testServer?.Dispose();

        if (_webApplication != null)
        {
            await _webApplication.StopAsync().ConfigureAwait(false);
            await _webApplication.DisposeAsync().ConfigureAwait(false);
        }

        _disposedAsync = true;

        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        if (!_disposedAsync)
        {
            DisposeAsync()
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}
