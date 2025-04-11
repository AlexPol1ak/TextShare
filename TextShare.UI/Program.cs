using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.DAL.Repositories;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Settings;
using TextShare.UI.Data;
using TextShare.UI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

string mySqlConnectionLocal = "MySQLConnectionLocal";
string mySqlConnectionBeget = "MySqlConnectionBeget";
string connectionString = builder.Configuration.GetConnectionString(mySqlConnectionLocal)
    ?? throw new InvalidOperationException("Connection string 'Connection string' not found.");

builder.Services.AddDbContext<TextShareContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), d => d.MigrationsAssembly("TextShare.UI")));

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<TextShareContext>()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

//builder.Services.AddAuthorization(opt =>
//{
//    opt.AddPolicy("admin", p => p.RequireClaim(ClaimTypes.Role, "admin"));
//});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Фильтр разлогирует пользователей с старыми куками
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<EnsureUserExistsFilter>();
});

builder.Services.Configure<ImageUploadSettings>(builder.Configuration.GetSection("ImageUploadSettings"));
builder.Services.Configure<ShelvesSettings>(builder.Configuration.GetSection("ShelvesSettings"));
builder.Services.Configure<GroupsSettings>(builder.Configuration.GetSection("GroupsSettings"));
builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUploadSettings"));

// Business Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IShelfService, ShelfService>();
builder.Services.AddScoped<IAccessRuleService, AccessRuleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IComplaintReasonService, ComplaintReasonService>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();
builder.Services.AddScoped<IPhysicalFile, PhysicalFileService>
(
    provider =>
    {
        var env = provider.GetRequiredService<IWebHostEnvironment>();
        return new PhysicalFileService(env.WebRootPath);
    }

);
builder.Services.AddScoped<IAccessСontrolService, AccessСontrolService>();
builder.Services.AddScoped<ITextFileService, TextFileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// раскомментировать в прод
app.UseStatusCodePagesWithReExecute("/Error/{0}");


app.UseHttpsRedirection();


//  Блокировка доступа к /TextFiles извне
app.UseBlockTextFiles();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


DbInitData initData = new(app);
await initData.SeedData();

app.Run();
