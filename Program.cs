using gestionDiversidad.Controllers;
using gestionDiversidad.Models;
using Microsoft.EntityFrameworkCore;
using gestionDiversidad.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllersWithViews();

//Add memory cache
builder.Services.AddDistributedMemoryCache();

//Add context of database
builder.Services.AddDbContext<TfgContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TfgContext")));

//Inyect "ServiceController"
builder.Services.AddScoped<IServiceController, ServiceController>();

//Add Razor with HttpContext
builder.Services.AddHttpContextAccessor();

// Configure the session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Born of the SeedData
//using (var serviceScope = app.Services.CreateScope())
//{
//    var context = serviceScope.ServiceProvider;
//    SeedData.Initialize_Usuarios(context);
//}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
  //pattern: "{controller=TUsuarios}/{action=Index}/{id?}");
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
