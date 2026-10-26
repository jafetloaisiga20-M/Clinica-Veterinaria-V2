using APLICATION.Interfaces;
using APLICATION.Servicios;
using INFRASTRUCTURE.DataBase;
using INFRASTRUCTURE.Repository;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//conecction de la base de datos(tambien en appsetting.json)
var conecctionString = builder.Configuration.GetConnectionString("default");
builder.Services.AddSingleton(new DBconnecctionFactory(conecctionString!));

//citas
builder.Services.AddScoped<ICitasRepository, CitasRepository>();
builder.Services.AddScoped<CitasServices>();
//clientes
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ClienteServices>();
//detallefactura
builder.Services.AddScoped<IDetalleFacturaRepository, DetalleFacturaRepository>();
builder.Services.AddScoped<DetalleFacturaServices>();

//enmienda
builder.Services.AddScoped<IEnmiendaRepository, EnmiendaMedicaRepository>();
builder.Services.AddScoped<EnmiendaMedicaServices>();
//expedienteMedico
builder.Services.AddScoped<IExpedienteMedicoRepository, ExpedienteMedicoRepository>();
builder.Services.AddScoped<ExpedienteMedicoServices>();
//factura
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<FacturaServices>();
//paciente
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<PacienteServices>();
//roles
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<RolesServices>();
//servicio
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<ServiciosServices>();
//sucursal
builder.Services.AddScoped<ISucursalRepository, SucursalRepository>();
builder.Services.AddScoped<SucursalServices>();
//turnoGuardia
builder.Services.AddScoped<ITurnoGuardiaRepository, TurnoGuardiaRepository>();
builder.Services.AddScoped<TurnoGuardioServices>();
//usuario
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioServices>();

//veterinario
builder.Services.AddScoped<IVeterinarioRepository, VeterinarioRepository>();
builder.Services.AddScoped<VeterinarioServices>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
