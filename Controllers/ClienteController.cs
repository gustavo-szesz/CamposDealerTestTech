using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using CamposDealerTesteTec.API.Data;
using Services;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CamposDealerTesteTec.API.Controllers
{
    public class ClienteController : Controller
    {
        #region Injeção de Dependências - Construtor (Repository, DataLoadService e ApplicationDbContext)	
        /// <summary>
        /// Inicialização dos campos privados Repository e DataLoadService e ApplicationDbContext
        /// </summary>
        private readonly IRepository<Cliente> _repository;
        private readonly DataLoadService _dataLoadService;
        private readonly ApplicationDbContext _context;
        public ClienteController(
            IRepository<Cliente> repository,
            DataLoadService dataLoadService,
            ApplicationDbContext context)
        {
            _repository = repository;
            _dataLoadService = dataLoadService;
            _context = context;
        }
        #endregion

        #region Listagem de Clientes
        /// <summary>
        ///  Método Index - GET (Listagem de Clientes)
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var clientes = await _repository.GetAllAsync();
            return View(clientes);
        }
        #endregion

        #region Criação de Cliente (Atualmente não utilizado)
        /// <summary>
        ///  Método Create - GET (Criação de Cliente) 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }
        #endregion
        
        #region Criação de Cliente - Sobrecarga do Método Create, está em uso.
        /// <summary>
        /// Método Create - POST (Criação de Cliente), aceita o parametro cliente do tipo Cliente
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }
        #endregion

        #region Edição de Cliente - GET
        /// <summary>
        /// Método Edit - GET (Edição de Cliente), aceita o parametro id do tipo int pelo sistema.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }
        #endregion

        #region Edição de Cliente - Sobrecarga do Método Edit
        /// <summary>
        /// Método Edit - POST (Edição de Cliente), aceita os parametros id do tipo int e cliente do tipo Cliente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost]
        // POST está sendo utilizado por questão de compatibilidade com navegadores antigos;
        [ValidateAntiForgeryToken]
        // Anotation usada para proteger a aplicação de ataques CSRF;
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.idCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }
        #endregion

        #region Exclusão de Cliente - GET
        /// <summary>
        /// Método Delete - GET (Exclusão de Cliente), aceita o parametro id do tipo int pelo sistema.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }
        #endregion

        #region Exclusão de Cliente - POST
        /// <summary>
        /// Método Delete - POST (Exclusão de Cliente), aceita o parametro id do tipo int pelo sistema.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Detalhes do Cliente
        /// <summary>
        /// Método Details - GET (Detalhes do Cliente), aceita o parametro id do tipo int pelo sistema.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }
        #endregion

        #region Importação de Clientes
        /// <summary>
        ///  Método ImportarClientes - GET (Importação de Clientes) 
        ///  Método para importar clientes de um Endpoint externo para o Banco de Dados
        ///  Utiliza uma transação explícita para garantir a integridade dos dados.
        /// </summary>
        /// <returns></returns>

        // Ponto de atenção: Método está muito grande, pode ser quebrado em métodos menores
        [HttpGet]
        public async Task<IActionResult> ImportarClientes()
        {
            try
            {
                var clientes = await _dataLoadService.GetClientesAsync();
                
                // Se o cliente for null, não irá carregar o !clientes.Any() em memória, reduzindo espaço.
                if (clientes == null || !clientes.Any())
                {
                    TempData["ErrorMessage"] = "Nenhum cliente encontrado para importação.";
                    return RedirectToAction(nameof(Index));
                }
                
                int importados = 0;
                int atualizados = 0;
                
                // Usar uma transação explícita
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // importante! Habilitar IDENTITY_INSERT para permitir inserção de valores na PK
                        // O Endpoint externo já tem valores externos, então é necessário habilitar o IDENTITY_INSERT
                        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Clientes] ON");
                        
                        // Processar os clientes
                        foreach (var cliente in clientes)
                        {
                            var clienteExistente = await _context.Clientes
                                .AsNoTracking() // Importante para evitar conflitos de rastreamento
                                .FirstOrDefaultAsync(c => c.idCliente == cliente.idCliente);
                            
                            if (clienteExistente == null)
                            {
                                // Inserir com comando SQL direto
                                await _context.Database.ExecuteSqlRawAsync(
                                    "INSERT INTO [Clientes] ([idCliente], [nmCliente], [Cidade]) VALUES ({0}, {1}, {2})",
                                    cliente.idCliente, cliente.nmCliente, cliente.Cidade);
                                
                                importados++;
                            }
                            else
                            {
                                // Atualizar com comando SQL direto
                                await _context.Database.ExecuteSqlRawAsync(
                                    "UPDATE [Clientes] SET [nmCliente] = {1}, [Cidade] = {2} WHERE [idCliente] = {0}",
                                    cliente.idCliente, cliente.nmCliente, cliente.Cidade);
                                
                                atualizados++;
                            }
                        }
                        
                        // Desativar IDENTITY_INSERT
                        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Clientes] OFF");
                        
                        // Confirmar a transação
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        // Em caso de erro, reverter a transação
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
                
                // Preparar mensagem de sucesso
                string mensagem = $"{importados} clientes importados";
                if (atualizados > 0)
                    mensagem += $" e {atualizados} atualizados";
                
                TempData["SuccessMessage"] = mensagem + " com sucesso!";
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao importar clientes: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            #endregion
        }
    }
}