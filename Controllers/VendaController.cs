using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using CamposDealerTesteTec.API.Data;

namespace CamposDealerTesteTec.API.Controllers
{
    public class VendaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Venda
        public async Task<IActionResult> Index()
        {
            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .ToListAsync();
            return View(vendas);
        }

        // GET: Venda/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .FirstOrDefaultAsync(m => m.idVenda == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // GET: Venda/Create
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes, "idCliente", "nmCliente");
            ViewBag.Produtos = new SelectList(_context.Produtos, "idProduto", "dscProduto");
            return View();
        }

        // POST: Venda/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idCliente,idProduto,qtdVenda,vlrUnitarioVenda,dthVenda")] Venda venda)
        {
            if (ModelState.IsValid)
            {
                // Calcular o valor total
                venda.vlrTotalVenda = (float)(venda.qtdVenda * venda.vlrUnitarioVenda);
                
                _context.Add(venda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clientes = new SelectList(_context.Clientes, "idCliente", "nmCliente", venda.idCliente);
            ViewBag.Produtos = new SelectList(_context.Produtos, "idProduto", "dscProduto", venda.idProduto);
            return View(venda);
        }

        // GET: Venda/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas.FindAsync(id);
            if (venda == null)
            {
                return NotFound();
            }
            
            ViewBag.Clientes = new SelectList(_context.Clientes, "idCliente", "nmCliente", venda.idCliente);
            ViewBag.Produtos = new SelectList(_context.Produtos, "idProduto", "dscProduto", venda.idProduto);
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
                try
                {
                    // Calcular o valor total
                    venda.vlrTotalVenda = (float)(venda.qtdVenda * venda.vlrUnitarioVenda);
                    
                    _context.Update(venda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendaExists(venda.idVenda))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clientes = new SelectList(_context.Clientes, "idCliente", "nmCliente", venda.idCliente);
            ViewBag.Produtos = new SelectList(_context.Produtos, "idProduto", "dscProduto", venda.idProduto);
            return View(venda);
        }

        // GET: Venda/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .FirstOrDefaultAsync(m => m.idVenda == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // POST: Venda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);
            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Venda/GetProdutoValor/5
        [HttpGet]
        public async Task<IActionResult> GetProdutoValor(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Content(produto.vlrUnitario.ToString());
        }

        private bool VendaExists(int id)
        {
            return _context.Vendas.Any(e => e.idVenda == id);
        }
    }
}