using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraInvestimento.Domain.Entities
{
    public class MovimentacoesResumo
    {
        public int Id { get; set; }
        public int Total { get; set; }
        public decimal ValorInvestido { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public int? TickerId { get; set; }  // Chave estrangeira para Ticker
        public Ticker? Ticker { get; set; }  // Navegação para o objeto Ticker
        public decimal DividendoTotal { get; set; }
        public decimal DY { get; set; }
    }
}
