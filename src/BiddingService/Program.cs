using BiddingService.Consumers;
using BiddingService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using MongoDB.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
	x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

	x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));

	x.UsingRabbitMq((context, cfg) =>
	{
		cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
		{
			host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
			host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
		});

		cfg.ConfigureEndpoints(context);
	});
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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<CheckAuctionFinished>();
builder.Services.AddScoped<GrpcAuctionClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await DB.InitAsync("BidDb", MongoClientSettings
	.FromConnectionString(builder.Configuration.GetConnectionString("BidDbConnection")));

app.Run();