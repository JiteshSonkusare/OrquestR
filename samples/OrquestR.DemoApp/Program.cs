using Web.Endpoints;
using Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

app.UseDependencies();

app.MapUserEndpoints();

app.Run();