using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using System;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

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