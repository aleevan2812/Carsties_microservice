using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateWayService.Extenstions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppAuthetication();

if (builder.Environment.EnvironmentName.ToString().ToLower().Equals("production"))
{
    builder.Configuration.AddJsonFile("ocelot.Production.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.Development.json", optional: false, reloadOnChange: true);
}
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();
app.UseOcelot().GetAwaiter().GetResult();
app.Run();