using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CarteiraInvestimento.Application.Dtos
{
    public class MovimentacoesDto
    {
        public int Id { get; set; }
        [JsonPropertyName("Entrada/Saída")]
        public string EntradaSaida { get; set; }

        [JsonPropertyName("Data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("Movimentação")]
        public string Movimentacao { get; set; }

        [JsonPropertyName("Produto")]
        public string Produto { get; set; }

        [JsonPropertyName("Instituição")]
        public string Instituicao { get; set; }

        [JsonPropertyName("Quantidade")]
        public decimal Quantidade { get; set; }

        [JsonPropertyName("Preço unitário")]
        public string PrecoUnitario { get; set; }

        [JsonPropertyName("Valor da Operação")]
        public string ValorOperacao { get; set; }
    }
}
