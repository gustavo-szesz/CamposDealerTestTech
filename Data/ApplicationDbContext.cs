using Microsoft.EntityFrameworkCore;
using Models;

namespace CamposDealerTesteTec.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do Cliente
            modelBuilder.Entity<Cliente>()
                .HasKey(c => c.idCliente);
                
            // Configuração do Produto
            modelBuilder.Entity<Produto>()
                .HasKey(p => p.idProduto);
                
            // Configuração da Venda e suas relações
            modelBuilder.Entity<Venda>()
                .HasKey(v => v.idVenda);
                
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey(v => v.idCliente)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Produto)
                .WithMany()
                .HasForeignKey(v => v.idProduto)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Configuração para o cálculo automático do vlrTotalVenda
            modelBuilder.Entity<Venda>()
                .Property(v => v.vlrTotalVenda)
                .HasComputedColumnSql("qtdVenda * CAST(vlrUnitarioVenda AS float)", stored: true);
        }
    }
}