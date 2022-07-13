using HealthCheckApi;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddCheck("Ping Zühlke", new PingHealthCheck("www.zuehlke.com", 100))
    .AddCheck("Ping Google", new PingHealthCheck("www.google.com", 100));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var options = new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = JsonConvert.SerializeObject(new
        {
            status = report.Status.ToString(),
            errors = report.Entries.Select(e => new {key = e.Key, value = e.Value.Status.ToString()})
        });

        await context.Response.WriteAsync(result);
    }
};

app.UseHealthChecks("/healthcheck", options);

app.MapControllers();

app.Run();