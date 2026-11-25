using APLICATION.Interfaces;
using APLICATION.Servicios;
using INFRASTRUCTURE.DataBase;
using INFRASTRUCTURE.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS: en local se permite Vite por defecto. En producción se define con la
// variable de entorno Cors__AllowedOrigins (lista separada por comas).
var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:5173")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", policy =>
    {
        if (allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    });
});

// Necesario cuando la app corre detrás de un proxy inverso (Render, Azure, nginx).
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllers();
 
// Add services to the container.
builder.Services.AddControllersWithViews();

// Conservar los nombres originales de las propiedades (ID_CLIENTE, NOMBRE_COMPLETO, ...)
// en lugar del camelCase por defecto, para que el frontend y los clientes vean claves estables.
builder.Services.Configure<JsonOptions>(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

// Autenticacion JWT (roles: Administrador, Veterinario, Recepcion, Cliente).
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-secret-key-change-in-production-32chars-minimum!!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "SaludAnimal",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "SaludAnimal",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// Servicios
builder.Services.AddScoped<AuthServices>();

// Conexión a la base de datos (local en appsettings.json, en la nube por variable de entorno).
var connectionString = builder.Configuration.GetConnectionString("default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Falta la cadena de conexión. Configura ConnectionStrings__default en el entorno.");
}
builder.Services.AddSingleton(new DBconnecctionFactory(connectionString));

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

// Mensajes de redes sociales (leads de WhatsApp/Facebook)
builder.Services.AddScoped<IMensajeSocialRepository, MensajeSocialRepository>();
builder.Services.AddScoped<MensajeSocialServices>();
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

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // HSTS: solo tiene sentido sobre HTTPS (en Azure se activa "HTTPS Only").
    app.UseHsts();
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();
// 2. ACTIVAR EL CORS
app.UseCors("PermitirReact");

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Liveness: responde 200 sin tocar la base de datos. Sirve para monitoreo
// frecuente (cada 5 min) sin despertar la auto-pausa de Azure SQL free.
app.MapGet("/health/live", () => Results.Ok(new
{
    status = "ok",
    utc = DateTime.UtcNow
}));

// Readiness: verifica tambien la conexion a la base de datos.
// Health check: sirve para monitoreo, para Azure/Render y para las pruebas de QA.
app.MapGet("/health", async (DBconnecctionFactory db) =>
{
    try
    {
        await using var con = db.CreateConnection();
        await con.OpenAsync();
        await using var cmd = new SqlCommand("SELECT 1", con);
        await cmd.ExecuteScalarAsync();
        return Results.Ok(new
        {
            status = "ok",
            database = "ok",
            environment = app.Environment.EnvironmentName,
            utc = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            status = "error",
            database = "error",
            // No exponer detalles internos en produccion.
            detail = app.Environment.IsDevelopment()
                ? ex.Message
                : "Revisa los logs del servidor."
        }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
