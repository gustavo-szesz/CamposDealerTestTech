using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class DataLoadService
    {
        private readonly ApiService _apiService;
        private int _nextClienteId = 1;
        private int _nextProdutoId = 1;
        private int _nextVendaId = 1;
        private List<Cliente> _clientes;

        public ObservableCollection<Cliente> Clientes { get; private set; } = new ObservableCollection<Cliente>();
        public ObservableCollection<Produto> Produtos { get; private set; } = new ObservableCollection<Produto>();
        public ObservableCollection<Venda> Vendas { get; private set; } = new ObservableCollection<Venda>();

        public DataLoadService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Métodos para Cliente
        public async Task<List<Cliente>> LoadClientesAsync()
        {
            // Carregar os clientes da API se ainda não estiverem carregados
            if (_clientes == null || _clientes.Count == 0)
            {
                _clientes = await _apiService.GetClientesAsync  ();
            }
            
            // Retornar a lista de clientes
            return _clientes;
        }

        public async Task<Cliente> CreateClienteAsync(Cliente cliente)
        {
            cliente.idCliente = _nextClienteId++;
            Clientes.Add(cliente);
            return cliente;
        }

        public async Task<Cliente?> UpdateClienteAsync(Cliente cliente)
        {
            var existingCliente = Clientes.FirstOrDefault(c => c.idCliente == cliente.idCliente);
            if (existingCliente != null)
            {
                existingCliente.nmCliente = cliente.nmCliente;
                existingCliente.Cidade = cliente.Cidade;
            }
            return existingCliente;
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = Clientes.FirstOrDefault(c => c.idCliente == id);
            if (cliente != null)
            {
                // Verificar se cliente está em alguma venda
                if (Vendas.Any(v => v.idCliente == id))
                {
                    return false; // Não pode excluir cliente que está em alguma venda
                }
                
                Clientes.Remove(cliente);
                return true;
            }
            return false;
        }

        // Adicionar um novo método que retorna a lista de clientes
        public async Task<List<Cliente>> GetClientesAsync()
        {
            // Se ainda não tiver carregado, carrega da API
            if (_clientes == null || !_clientes.Any())
            {
                var apiResponse = await _apiService.GetClientesAsync();
                _clientes = apiResponse;
            }
            
            return _clientes;
        }

        // Métodos para Produto
        public async Task LoadProdutosAsync()
        {
            var produtos = await _apiService.GetProdutosAsync();
            if (produtos != null && produtos.Any())
            {
                Produtos.Clear();
                foreach (var produto in produtos)
                {
                    Produtos.Add(produto);
                }
                _nextProdutoId = Produtos.Max(p => p.idProduto) + 1;
            }
        }

        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
            produto.idProduto = _nextProdutoId++;
            Produtos.Add(produto);
            return produto;
        }

        public async Task<Produto?> UpdateProdutoAsync(Produto produto)
        {
            var existingProduto = Produtos.FirstOrDefault(p => p.idProduto == produto.idProduto);
            if (existingProduto != null)
            {
                // Aqui estava o erro: o campo deve ser dscProduto, não nmProduto
                existingProduto.dscProduto = produto.dscProduto;
                existingProduto.vlrUnitario = produto.vlrUnitario;
            }
            return existingProduto;
        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var produto = Produtos.FirstOrDefault(p => p.idProduto == id);
            if (produto != null)
            {
                // Verificar se produto está em alguma venda
                if (Vendas.Any(v => v.idProduto == id))
                {
                    return false; // Não pode excluir produto que está em alguma venda
                }
                
                Produtos.Remove(produto);
                return true;
            }
            return false;
        }

        // Métodos para Venda
        public async Task LoadVendasAsync()
        {
            var vendas = await _apiService.GetVendasAsync();
            if (vendas != null && vendas.Any())
            {
                Vendas.Clear();
                foreach (var venda in vendas)
                {
                    // Atribuir objetos relacionados
                    venda.Cliente = Clientes.FirstOrDefault(c => c.idCliente == venda.idCliente);
                    venda.Produto = Produtos.FirstOrDefault(p => p.idProduto == venda.idProduto);
                    Vendas.Add(venda);
                }
                _nextVendaId = Vendas.Max(v => v.idVenda) + 1;
            }
        }

        public async Task<Venda> CreateVendaAsync(Venda venda)
        {
            // Calcular valor total
            venda.vlrTotalVenda = (float)(venda.qtdVenda * venda.vlrUnitarioVenda);
            venda.idVenda = _nextVendaId++;
            
            // Atribuir objetos relacionados
            venda.Cliente = Clientes.FirstOrDefault(c => c.idCliente == venda.idCliente);
            venda.Produto = Produtos.FirstOrDefault(p => p.idProduto == venda.idProduto);
            
            Vendas.Add(venda);
            return venda;
        }

        public async Task<Venda?> UpdateVendaAsync(Venda venda)
        {
            var existingVenda = Vendas.FirstOrDefault(v => v.idVenda == venda.idVenda);
            if (existingVenda != null)
            {
                existingVenda.idCliente = venda.idCliente;
                existingVenda.idProduto = venda.idProduto;
                existingVenda.qtdVenda = venda.qtdVenda;
                existingVenda.vlrUnitarioVenda = venda.vlrUnitarioVenda;
                existingVenda.dthVenda = venda.dthVenda;
                existingVenda.vlrTotalVenda = (float)(venda.qtdVenda * venda.vlrUnitarioVenda);
                
                // Atualizar objetos relacionados
                existingVenda.Cliente = Clientes.FirstOrDefault(c => c.idCliente == venda.idCliente);
                existingVenda.Produto = Produtos.FirstOrDefault(p => p.idProduto == venda.idProduto);
            }
            return existingVenda;
        }

        public async Task<bool> DeleteVendaAsync(int id)
        {
            var venda = Vendas.FirstOrDefault(v => v.idVenda == id);
            if (venda != null)
            {
                Vendas.Remove(venda);
                return true;
            }
            return false;
        }
    }
}