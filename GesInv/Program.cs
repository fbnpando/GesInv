using Microsoft.EntityFrameworkCore;
using GesInv.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Registra el DbContext con la cadena de conexi�n
builder.Services.AddDbContext<GestionInvContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conn")));

// Agrega los servicios MVC y configura el manejo de referencias c�clicas
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Configura para preservar referencias c�clicas
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Configura CORS para permitir solicitudes de or�genes externos
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

// Configura la tuber�a de solicitudes HTTP
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

// Usa CORS con la pol�tica definida
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

// Configura el enrutamiento de controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
