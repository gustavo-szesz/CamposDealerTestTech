using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Cliente>> GetClientesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://camposdealer.dev/Sites/TesteAPI/cliente");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // Se o conteúdo estiver entre aspas, primeiro desserializa como string
                    if (content.StartsWith("\""))
                    {
                        content = JsonConvert.DeserializeObject<string>(content);
                    }
                    
                    return JsonConvert.DeserializeObject<List<Cliente>>(content) ?? new List<Cliente>();
                }
                return new List<Cliente>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter clientes: {ex.Message}");
                return new List<Cliente>();
            }
        }

        public async Task<List<Produto>> GetProdutosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://camposdealer.dev/Sites/TesteAPI/produto");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // Se o conteúdo estiver entre aspas, primeiro desserializa como string
                    if (content.StartsWith("\""))
                    {
                        content = JsonConvert.DeserializeObject<string>(content);
                    }
                    
                    return JsonConvert.DeserializeObject<List<Produto>>(content) ?? new List<Produto>();
                }
                return new List<Produto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter produtos: {ex.Message}");
                return new List<Produto>();
            }
        }

        public async Task<List<Venda>> GetVendasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://camposdealer.dev/Sites/TesteAPI/venda");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // Se o conteúdo estiver entre aspas, primeiro desserializa como string
                    if (content.StartsWith("\""))
                    {
                        content = JsonConvert.DeserializeObject<string>(content);
                    }
                    
                    return JsonConvert.DeserializeObject<List<Venda>>(content) ?? new List<Venda>();
                }
                return new List<Venda>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter vendas: {ex.Message}");
                return new List<Venda>();
            }
        }

        public async Task<Venda?> GetVendaByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://camposdealer.dev/Sites/TesteAPI/venda/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // Se o conteúdo estiver entre aspas, primeiro desserializa como string
                    if (content.StartsWith("\""))
                    {
                        content = JsonConvert.DeserializeObject<string>(content);
                    }
                    
                    return JsonConvert.DeserializeObject<Venda>(content);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter venda por ID: {ex.Message}");
                return null;
            }
        }
    }
}
