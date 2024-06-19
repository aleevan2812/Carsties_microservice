using SearchService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

try
{
    await DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

app.MapControllers();

app.Run();