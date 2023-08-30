using Intranet.Data;
using Intranet.IServices;
using Intranet.Security;
using Intranet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB Setup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IntranetDbContext>(x => x.UseSqlServer(connectionString));


// Use Identity with roles (Custom AppUser to add more attributes to the user table)
builder.Services.AddIdentity<Intranet.Entities.AppUser, IdentityRole>().AddEntityFrameworkStores<IntranetDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;
});


// this is Options Pattern just to map the app settings config with the class 
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<IJwtService, JwtService>();

// within this section we are configuring the authentication and setting the default scheme with JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
    IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = true
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameters;
});


// This Section is for services DI
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

app.MapControllers();

app.Run();
