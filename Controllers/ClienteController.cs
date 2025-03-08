using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace CamposDealerTesteTec.API.Controllers
{
    public class ClienteController : Controller
    {
        // Injeção de dependência do serviço de carregamento de dados
        // campos privados para que seja possível acessar o serviço de carregamento de dados   
        private readonly DataLoadService _dataLoadService;

        public ClienteController(DataLoadService dataLoadService)
        {
            _dataLoadService = dataLoadService;
        }

        // NOTA: método pode ser simplificado usando .ToList() ao invés de 
        // new System.Collections.ObjectModel.ObservableCollection<Cliente>
        // Metodo GET: Cliente
        public IActionResult Index(string? searchString = null)
        {
            var clientes = _dataLoadService.Clientes;

            // Filtrar por nome se a string de pesquisa foi fornecida, 
            // 1° Requisito do CRUD de Cliente: 

            if (!string.IsNullOrEmpty(searchString))
            {
                // LINQ query para filtrar clientes por nome e StringComparision para ignorar maiúsculas e minúsculas
                clientes = new System.Collections.ObjectModel.ObservableCollection<Cliente>(
                    clientes.Where(c => c.nmCliente.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            return View(clientes);
        }

        // Metodo GET: Cliente/Details/ 
        // 1.1 Requisito do CRUD de Cliente, listar detalhes do clientes por ID (usado 
        // mais no debug do código)
        public IActionResult Details(int id)
        {
            var cliente = _dataLoadService.Clientes.FirstOrDefault(c => c.idCliente == id);
            if (cliente == null)
            {
                return NotFound(WebUtility.HtmlEncode("Cliente não encontrado, ID inválida."));
            }

            return View(cliente);
        }

        // GET: Cliente/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cliente/Create
        // [ValidateAntiForgeryToken] adicionei como boa prática para proteger contra ataques CSRF
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("nmCliente,Cidade")] Cliente cliente)
        {
            // Validação do modelo
            if (ModelState.IsValid)
            {
                await _dataLoadService.CreateClienteAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Cliente/Edit/
        // Similar ao método Details, mas com a diferença de que o método Edit é para edição
        public IActionResult Edit(int id)
        {
            var cliente = _dataLoadService.Clientes.FirstOrDefault(c => c.idCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        // [ValidateAntiForgeryToken] adicionei como boa prática para proteger contra ataques CSRF
        // [Bind("idCliente,nmCliente,Cidade")] para proteger contra overposting
        // Apesar de ser um método POST, ele é usado para atualizar um cliente, 
        // foi utilizado o POST como boa prática sobre compatibilidade com navegadores e HTML
        // 3° Requisito do CRUD de Cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idCliente,nmCliente,Cidade")] Cliente cliente)
        {
            if (id != cliente.idCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _dataLoadService.UpdateClienteAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Cliente/Delete/
        // 4° Requisito do CRUD de Cliente, deletar cliente 
        public IActionResult Delete(int id)
        {
            // Carregar lista/ObservableCollection de clientes de maneira descrescente
            var cliente = _dataLoadService.Clientes.FirstOrDefault(c => c.idCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Cliente/Delete/5
        // Confirmar exclusão de cliente
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataLoadService.DeleteClienteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Cliente/Import
        public async Task<IActionResult> Import()
        {
            await _dataLoadService.LoadClientesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}