using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Middleware;
using PolyLearnAI.Api.Options;
using PolyLearnAI.Api.Services.Implementations;
using PolyLearnAI.Api.Services.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configuration ----------
var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(portEnv))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{portEnv}");
}

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
{
    jwt.Key = "PolyLearnAI_Super_Secret_Jwt_Prod_Key_2026_AtLeast32Bytes!";
}

// ---------- Database (EF Core + SQL Server / SQLite Cloud Auto-Fallback) ----------
var connStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var useSqlite = !OperatingSystem.IsWindows() && connStr.Contains("(localdb)", StringComparison.OrdinalIgnoreCase);
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    if (useSqlite)
    {
        opt.UseSqlite("Data Source=polylearn.db");
    }
    else
    {
        opt.UseSqlServer(connStr);
    }
});

// ---------- Authentication / Authorization ----------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

// ---------- CORS ----------
builder.Services.AddCors(opt => opt.AddPolicy("Frontend", p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// ---------- Application Services ----------
builder.Services.Configure<AiOptions>(builder.Configuration.GetSection(AiOptions.SectionName));
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<IAiService, AiService>();
builder.Services.AddHttpClient<IVocabHarvesterService, VocabHarvesterService>();

// ---------- MVC / OpenAPI ----------
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json
    app.MapScalarApiReference(); // /scalar/v1
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        ctx.Context.Response.Headers["Pragma"] = "no-cache";
        ctx.Context.Response.Headers["Expires"] = "0";
    }
}); // phục vụ Frontend trong wwwroot (tắt cache để luôn tải bản mới nhất)
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.SeedAdminUserAsync();

app.Run();
