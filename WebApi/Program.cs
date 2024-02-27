using WebApi.Application;
using WebApi.Endpoints;
using WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEndpoints();

app.Run();
