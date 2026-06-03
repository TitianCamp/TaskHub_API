using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TaskHub.Business.Validators;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

string connectionString = configuration.GetConnectionString("DefaultConnection")!;

JwtSettings jwtSettings = new(
    configuration["Jwt:Key"]!,
    configuration["Jwt:Issuer"]!,
    configuration["Jwt:Audience"]!,
    int.Parse(configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15"),
    int.Parse(configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")
    );

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(option => 
    option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = 
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
    }); 
builder.Services.AddAuthorization();

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<IJwtService, JwtService>();

builder.Services.AddScoped<UserValidator>();
builder.Services.AddScoped<IValidator<TeamRequest>, TeamValidator>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<TeamService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    string[] roles = { "Admin", "User" };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
