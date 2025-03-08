using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idCliente { get; set; }
        
        [Required]
        [StringLength(100)]
        public string nmCliente { get; set; }
        
        [StringLength(100)]
        public string Cidade { get; set; }
    }
}