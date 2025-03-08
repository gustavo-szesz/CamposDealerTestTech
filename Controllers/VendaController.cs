using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CamposDealerTesteTec.API.Controllers
{
    public class VendaController : Controller
    {
        private readonly DataLoadService _dataLoadService;

        public VendaController(DataLoadService dataLoadService)
        {
            _dataLoadService = dataLoadService;
        }

        // GET: Venda
        public IActionResult Index(string? searchString = null)
        {
            var vendas = _dataLoadService.Vendas;

            // Atualizar objetos relacionados para garantir que estejam disponíveis
            foreach (var venda in vendas)
            {
                venda.Cliente = _dataLoadService.Clientes.FirstOrDefault(c => c.idCliente == venda.idCliente);
                venda.Produto = _dataLoadService.Produtos.FirstOrDefault(p => p.idProduto == venda.idProduto);
            }

            // Filtrar por nome do cliente ou descrição do produto se a string de pesquisa foi fornecida
            if (!string.IsNullOrEmpty(searchString))
            {
                vendas = new System.Collections.ObjectModel.ObservableCollection<Venda>(
                    vendas.Where(v => 
                        (v.Cliente != null && v.Cliente.nmCliente.Contains(searchString, StringComparison.OrdinalIgnoreCase)) || 
                        (v.Produto != null && v.Produto.dscProduto.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    )
                );
            }

            return View(vendas);
        }

        // GET: Venda/Details/5
        public IActionResult Details(int id)
        {
            var venda = _dataLoadService.Vendas.FirstOrDefault(v => v.idVenda == id);
            if (venda == null)
            {
                return NotFound();
            }

            venda.Cliente = _dataLoadService.Clientes.FirstOrDefault(c => c.idCliente == venda.idCliente);
            venda.Produto = _dataLoadService.Produtos.FirstOrDefault(p => p.idProduto == venda.idProduto);
            
            // Adicionar listas para os dropdowns
            ViewBag.Clientes = new SelectList(_dataLoadService.Clientes, "idCliente", "nmCliente");
            ViewBag.Produtos = new SelectList(_dataLoadService.Produtos, "idProduto", "dscProduto", null, "vlrUnitario");

            return View(venda);
        }

        // GET: Venda/Create
        public IActionResult Create()
        {
            // Preparar listas para dropdown
            ViewBag.Clientes = new SelectList(_dataLoadService.Clientes, "idCliente", "nmCliente");
            ViewBag.Produtos = new SelectList(_dataLoadService.Produtos, "idProduto", "dscProduto");
            
            return View(new Venda { dthVenda = DateTime.Now });
        }

        // POST: Venda/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idCliente,idProduto,qtdVenda,vlrUnitarioVenda,dthVenda")] Venda venda)
        {
            if (ModelState.IsValid)
            {
                // Calcular o valor total é feito no serviço
                await _dataLoadService.CreateVendaAsync(venda);
                return RedirectToAction(nameof(Index));
            }

            // Recarregar listas para dropdown se houver erro
            ViewBag.Clientes = new SelectList(_dataLoadService.Clientes, "idCliente", "nmCliente");
            ViewBag.Produtos = new SelectList(_dataLoadService.Produtos, "idProduto", "dscProduto");
            
            return View(venda);
        }

        // GET: Venda/Edit/5
        public IActionResult Edit(int id)
        {
            var venda = _dataLoadService.Vendas.FirstOrDefault(v => v.idVenda == id);
            if (venda == null)
            {
                return NotFound();
            }

            venda.Cliente = _dataLoadService.Clientes.FirstOrDefault(c => c.idCliente == venda.idCliente);
            venda.Produto = _dataLoadService.Produtos.FirstOrDefault(p => p.idProduto == venda.idProduto);
            
            // Adicionar listas para os dropdowns
            ViewBag.Clientes = new SelectList(_dataLoadService.Clientes, "idCliente", "nmCliente", venda.idCliente);
            ViewBag.Produtos = new SelectList(_dataLoadService.Produtos, "idProduto", "dscProduto", venda.idProduto);

            return View(venda);
        }

        // POST: Venda/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idVenda,idCliente,idProduto,qtdVenda,vlrUnitarioVenda,dthVenda")] Venda venda)
        {
            if (id != venda.idVenda)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Calcular valor total é feito no serviço
                await _dataLoadService.UpdateVendaAsync(venda);
                return RedirectToAction(nameof(Index));
            }

            // Recarregar listas para dropdown se houver erro
            ViewBag.Clientes = new SelectList(_dataLoadService.Clientes, "idCliente", "nmCliente");
            ViewBag.Produtos = new SelectList(_dataLoadService.Produtos, "idProduto", "dscProduto");
            
            return View(venda);
        }

        // GET: Venda/Delete/5
        public IActionResult Delete(int id)
        {
            var venda = _dataLoadService.Vendas.FirstOrDefault(v => v.idVenda == id);
            if (venda == null)
            {
                return NotFound();
            }

            venda.Cliente = _dataLoadService.Clientes.FirstOrDefault(c => c.idCliente == venda.idCliente);
            venda.Produto = _dataLoadService.Produtos.FirstOrDefault(p => p.idProduto == venda.idProduto);

            return View(venda);
        }

        // POST: Venda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataLoadService.DeleteVendaAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Venda/Import
        public async Task<IActionResult> Import()
        {
            await _dataLoadService.LoadVendasAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}