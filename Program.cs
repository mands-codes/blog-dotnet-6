using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

LoadConfiguration(builder);

ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.UseStaticFiles();
app.MapControllers();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    Console.WriteLine("Development environment detected.");
}

app.Run();

void LoadConfiguration(WebApplicationBuilder builder)
{
    Configuration.JwtKey = builder.Configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = builder.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKeyValue = builder.Configuration.GetValue<string>("ApiKeyValue");

    var smtp = new SmtpConfiguration();
    builder.Configuration.GetSection("SmtpConfiguration").Bind(smtp);
    Configuration.Smtp = smtp;
    Configuration.DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x => {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache();
    builder.Services.AddResponseCompression(options =>
    {
        options.Providers.Add<GzipCompressionProvider>();
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });
    builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<BlogDataContext>(options =>
    {
        options.UseSqlServer(Configuration.DefaultConnection);
    });
    builder.Services.AddTransient<TokenService>();
}