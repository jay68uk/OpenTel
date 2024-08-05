using System.Diagnostics;
using FastEndpoints;
using OpenTel.Api.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddOpenTelemetry();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseFastEndpoints();
app.Run();
