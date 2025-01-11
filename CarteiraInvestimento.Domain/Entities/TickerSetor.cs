using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraInvestimento.Domain.Entities
{
    public class TickerSetor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Peso { get; set; }
    }
}
