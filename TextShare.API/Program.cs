using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.DAL.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Database connect
string mySqlConnectionLocal = "MySQLConnectionLocal";
string mySqlConnectionBeget = "MySqlConnectionBeget";
string connectionString = builder.Configuration.GetConnectionString(mySqlConnectionLocal)
    ?? throw new InvalidOperationException("Connection string 'Connection string' not found.");
builder.Services.AddDbContext<TextShareContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), d => d.MigrationsAssembly("TextShare.API")));

builder.Services.AddScoped<IPhysicalFile>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    return new PhysicalFileService(env.WebRootPath);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
