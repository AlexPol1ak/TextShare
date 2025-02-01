using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.DAL.Data;
using TextShare.Domain.Entities.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Database connect
string mySqlConnectionLocal = "MySQLConnectionLocal";
string mySqlConnectionBeget = "MySqlConnectionBeget";
string connectionString = builder.Configuration.GetConnectionString(mySqlConnectionLocal)
    ?? throw new InvalidOperationException("Connection string 'Connection string' not found.");
builder.Services.AddDbContext<TextShareContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), d => d.MigrationsAssembly("TextShare.API")));

// Files
builder.Services.AddScoped<IPhysicalFile>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    return new PhysicalFileService(env.WebRootPath);
});

// Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(
    options =>
    {
        options.Password.RequireDigit = false; 
        options.Password.RequireLowercase = false; 
        options.Password.RequireNonAlphanumeric = false; 
        options.Password.RequireUppercase = false; 
        options.Password.RequiredLength = 1;
        options.Password.RequiredUniqueChars = 1;
    }
    )
    .AddEntityFrameworkStores<TextShareContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);  
});

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

app.MapControllers();

app.Run();
