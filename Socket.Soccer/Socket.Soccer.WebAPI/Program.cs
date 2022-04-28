using Microsoft.AspNetCore.Http.Connections;
using Socket.Soccer.WebAPI.Background;
using Socket.Soccer.WebAPI.Hubs;
using Socket.Soccer.WebAPI.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IClientStore, ClientStore>();
builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
builder.Services.AddHostedService<ScopedProcessingServiceHostedService>();
builder.Services.AddDistributedMemoryCache(options =>
{
    options.ExpirationScanFrequency = TimeSpan.FromSeconds(30);
    options.CompactionPercentage = 0.5;
});
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
    hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(5);
}).AddNewtonsoftJsonProtocol();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials()
);

app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GameHub>("/game", m =>
    {
        m.Transports = HttpTransportType.WebSockets;
        m.WebSockets.CloseTimeout = TimeSpan.FromSeconds(30);
    });
});

app.Run();