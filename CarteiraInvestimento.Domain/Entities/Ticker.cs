using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraInvestimento.Domain.Entities
{
    public class Ticker
    {
        public Ticker()
        {
            
        }
        public Ticker(string ticker, string chave)
        {
            Nome = ticker;
            Chave = chave;
        }
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Chave { get; set; }
    }
}
