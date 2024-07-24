using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OcelotGateWayService.Extenstions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppAuthetication(this WebApplicationBuilder builder)
    {
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

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("customPolicy", b =>
            {
                b.AllowAnyHeader()
                    .AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["ClientApp"]);
            });
        });

        return builder;
    }
}