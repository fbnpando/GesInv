using Microsoft.EntityFrameworkCore;
using GesInv.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Registra el DbContext con la cadena de conexión
builder.Services.AddDbContext<GestionInvContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conn")));

// Agrega los servicios MVC y configura el manejo de referencias cíclicas
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Configura para preservar referencias cíclicas
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Configura CORS para permitir solicitudes de orígenes externos
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin() // Permite solicitudes de cualquier origen
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});



var app = builder.Build();

// Configura la tubería de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Usa CORS con la política definida
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

// Configura el enrutamiento de controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
