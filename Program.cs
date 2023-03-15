using gestionDiversidad.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add context of database
builder.Services.AddDbContext<TfgContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TfgContext")));

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

app.MapControllerRoute(
    name: "default",
  //pattern: "{controller=TUsuarios}/{action=Index}/{id?}");
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
