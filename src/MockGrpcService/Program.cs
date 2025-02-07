using MockGrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
});

var app = builder.Build();
app.UseGrpcWeb();

// Configure the HTTP request pipeline.
app.MapGrpcService<ScoreService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
