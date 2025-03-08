using System;
using System.Text.Json.Serialization;

namespace Models
{
    public class Venda
    {
        public int idVenda { get; set; }
        public int idCliente { get; set; }
        public int idProduto { get; set; }
        public int qtdVenda { get; set; }
        public float vlrUnitarioVenda { get; set; }
        
        [JsonConverter(typeof(MicrosoftDateConverter))]
        public DateTime dthVenda { get; set; }
        
        public float vlrTotalVenda { get; set; }
        
        // Navigation properties (optional)
        [JsonIgnore]
        public Cliente? Cliente { get; set; }
        
        [JsonIgnore]
        public Produto? Produto { get; set; }
    }
}