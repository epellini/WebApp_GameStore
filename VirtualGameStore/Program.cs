using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SendGrid.Extensions.DependencyInjection;
using VirtualGameStore.DataAccess;
using VirtualGameStore.Entities;
using VirtualGameStore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Get connection string and add DbContext:
var connStr = builder.Configuration.GetConnectionString("defaultConnection");
builder.Services.AddDbContext<GameStoreDbContext>(options => options.UseSqlServer(connStr));

// Add GameStoreManager service:
builder.Services.AddScoped<IGameStoreManager, GameStoreManager>();

//Set up the identity service options:
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
}).AddEntityFrameworkStores<GameStoreDbContext>().AddDefaultTokenProviders();

// Add Send Grid email service:
var emailStr = "SG." + builder.Configuration.GetValue<string>("EmailKey1") + "." + builder.Configuration.GetValue<string>("EmailKey2");
builder.Services.AddSendGrid(options => options.ApiKey = emailStr ?? throw new Exception("The 'SendGridApiKey' is not configured"));
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure the admin user if necessary:
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    await GameStoreDbContext.CreateAdminUser(scope.ServiceProvider);
}

app.Run();
