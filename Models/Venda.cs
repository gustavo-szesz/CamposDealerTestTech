using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Venda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idVenda { get; set; }
        
        [Required]
        public int idCliente { get; set; }
        
        [Required]
        public int idProduto { get; set; }
        
        [Required]
        public int qtdVenda { get; set; }
        
        [Required]
        public decimal vlrUnitarioVenda { get; set; }
        
        [Required]
        public DateTime dthVenda { get; set; }
        
        [Required]
        public float vlrTotalVenda { get; set; }
        
        [ForeignKey("idCliente")]
        public virtual Cliente Cliente { get; set; }
        
        [ForeignKey("idProduto")]
        public virtual Produto Produto { get; set; }
    }
}