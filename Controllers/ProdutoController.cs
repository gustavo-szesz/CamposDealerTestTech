using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CamposDealerTesteTec.API.Controllers
{
    public class ProdutoController : Controller
    {
        // Injeção de dependência do serviço de carregamento de dados
        // campos privados para que seja possível acessar o serviço de carregamento de dados
        private readonly DataLoadService _dataLoadService;

        public ProdutoController(DataLoadService dataLoadService)
        {
            _dataLoadService = dataLoadService;
        }

        // GET: Produto
        public IActionResult Index(string? searchString = null)
        {
            var produtos = _dataLoadService.Produtos;

            // Filtrar por descrição se a string de pesquisa foi fornecida
            if (!string.IsNullOrEmpty(searchString))
            {
                produtos = new System.Collections.ObjectModel.ObservableCollection<Produto>(
                    produtos.Where(p => p.dscProduto.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            return View(produtos);
        }

        // GET: Produto/Details/5
        public IActionResult Details(int id)
        {
            var produto = _dataLoadService.Produtos.FirstOrDefault(p => p.idProduto == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("dscProduto,vlrUnitario")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                await _dataLoadService.CreateProdutoAsync(produto);
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produto/Edit/5
        public IActionResult Edit(int id)
        {
            var produto = _dataLoadService.Produtos.FirstOrDefault(p => p.idProduto == id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        // POST: Produto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idProduto,dscProduto,vlrUnitario")] Produto produto)
        {
            if (id != produto.idProduto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _dataLoadService.UpdateProdutoAsync(produto);
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produto/Delete/5
        public IActionResult Delete(int id)
        {
            var produto = _dataLoadService.Produtos.FirstOrDefault(p => p.idProduto == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataLoadService.DeleteProdutoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Produto/Import
        public async Task<IActionResult> Import()
        {
            await _dataLoadService.LoadProdutosAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}