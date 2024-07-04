using AuctionService.Consumers;
using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

    x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
});

builder.Services
    // Thêm dịch vụ xác thực vào ứng dụng và chỉ định rằng sẽ sử dụng xác thực JWT Bearer.
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Đặt địa chỉ authority (URL của nhà cung cấp định danh)
        options.Authority = builder.Configuration["IdentityServiceUrl"];

        // Tắt yêu cầu phải sử dụng HTTPS cho metadata. Điều này có thể hữu ích trong quá trình phát triển và kiểm thử, nhưng không nên sử dụng trong môi trường sản xuất.
        options.RequireHttpsMetadata = false;

        // Không xác thực audience của token. Điều này có nghĩa là ứng dụng sẽ không kiểm tra xem token có được phát hành cho đối tượng cụ thể nào hay không.
        options.TokenValidationParameters.ValidateAudience = false;

        // Đặt loại claim dùng làm tên người dùng là "username". Điều này có nghĩa là khi token được xác thực, giá trị của claim "username" sẽ được sử dụng làm tên người dùng trong ứng dụng.
        options.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();