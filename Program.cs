using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using System;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using CamposDealerTesteTec.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Adicionar suporte a DbContext usando SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar HttpClient
builder.Services.AddHttpClient();

// Registrar ApiService
builder.Services.AddSingleton<ApiService>(sp => {
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    return new ApiService(httpClient);
});

// Registrar DataLoadService como Singleton para manter os dados durante a sessão
builder.Services.AddSingleton<DataLoadService>();

// Registrar repositórios
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Pré-carregar os dados quando o aplicativo iniciar
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    try
    {
        var dataLoadService = services.GetRequiredService<DataLoadService>();
        // Carregue dados iniciais de forma assíncrona
        dataLoadService.LoadClientesAsync().Wait();
        dataLoadService.LoadProdutosAsync().Wait();
        dataLoadService.LoadVendasAsync().Wait();
    }
    catch (Exception ex)
    {
        // Log the error
        Console.WriteLine($"Erro ao carregar dados iniciais: {ex.Message}");
    }
}

app.Run();