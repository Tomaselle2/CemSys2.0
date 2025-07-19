using CemSys2.Business;
using CemSys2.Data;
using CemSys2.Interface;
using CemSys2.Interface.Tarifaria;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;
using CemSys2.Interface.Introduccion;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);



//para el manejo de sesiones
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60); // Tiempo de expiración por inactividad
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar el DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion")));

//contenedor de capa de datos
builder.Services.AddScoped(typeof(IRepositoryDB<>), typeof(ServiceGenericDB<>));
builder.Services.AddScoped<ITarifariaBD, TarifariaBD>();
builder.Services.AddScoped<IIntroduccionBD, IntroduccionBD>();

//contenedor de capa de negocio
builder.Services.AddScoped(typeof(IRepositoryBusiness<>), typeof(ServiceGenericBusiness<>));
builder.Services.AddScoped<ISeccionesBusiness, SeccionesBusiness>();
builder.Services.AddScoped<IParcelasBusiness, ParcelasBusiness>();
builder.Services.AddScoped<ITarifariaBusiness, TarifariaBusiness>();
builder.Services.AddScoped<IIntroduccionBusiness, IntroduccionBusiness>();

var app = builder.Build();


// Configura Rotativa con la ruta de wkhtmltopdf
string wwwroot = app.Environment.WebRootPath;
RotativaConfiguration.Setup(wwwroot, "rotativa");

app.UseSession();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
