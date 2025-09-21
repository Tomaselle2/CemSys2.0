using CemSys2.Business;
using CemSys2.Data;
using CemSys2.Interface;
using CemSys2.Interface.Tarifaria;
using CemSys2.Models;
using Microsoft.EntityFrameworkCore;
using CemSys2.Interface.Introduccion;
using CemSys2.Interface.Personas;
using Rotativa.AspNetCore;
using CemSys2.Interface.Parcelas;
using CemSys2.Interface.Concesiones;
using PuppeteerSharp;
using CemSys2.Interface.Tramite;
using CemSys2.Interface.Facturas;



var builder = WebApplication.CreateBuilder(args);

// Registrar el holder del navegador
builder.Services.AddSingleton<BrowserHolder>();

// Registrar el servicio de inicialización del navegador
builder.Services.AddHostedService<BrowserInitializationService>();


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
builder.Services.AddScoped<IPersonasBD, PersonasBD>();
builder.Services.AddScoped<IParcelaBD, ParcelaBD>();
builder.Services.AddScoped<IConcesionesDB, ConcesionesBD>();
builder.Services.AddScoped<ITramiteBD, TramiteDB>();
builder.Services.AddScoped<IFacturasBD, FacturaBD>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//contenedor de capa de negocio
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped(typeof(IRepositoryBusiness<>), typeof(ServiceGenericBusiness<>));
builder.Services.AddScoped<ISeccionesBusiness, SeccionesBusiness>();
builder.Services.AddScoped<IParcelasBusiness, ParcelasBusiness>();
builder.Services.AddScoped<ITarifariaBusiness, TarifariaBusiness>();
builder.Services.AddScoped<IIntroduccionBusiness, IntroduccionBusiness>();
builder.Services.AddScoped<IPersonasBusiness, PersonasBusiness>();
builder.Services.AddScoped<IConcesionesBusiness, ConcesionesBusiness>();
builder.Services.AddScoped<ITramiteBusiness, TramiteBusiness>();
builder.Services.AddScoped<IFacturaBusiness, FacturasBusiness>();

var app = builder.Build();

// Cerrar el navegador al apagar la aplicación
app.Lifetime.ApplicationStopping.Register(() =>
{
    var holder = app.Services.GetService<BrowserHolder>();
    holder.Browser?.CloseAsync().GetAwaiter().GetResult();
});

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
