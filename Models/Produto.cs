using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idProduto { get; set; }
        
        [Required]
        [StringLength(200)]
        public string dscProduto { get; set; }
        
        [Required]
        public float vlrUnitario { get; set; }
    }
}