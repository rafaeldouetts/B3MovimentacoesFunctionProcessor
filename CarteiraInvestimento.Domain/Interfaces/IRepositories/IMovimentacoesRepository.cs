using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarteiraInvestimento.Domain.Entities;

namespace CarteiraInvestimento.Domain.Interfaces
{ 
    public interface IMovimentacoesRepository
    {
        Task AddRange(List<Movimentacoes> movimentacoes);
        Task<decimal> GetTotalInvestido();
        Task<List<Movimentacoes>> GetMovimentacoes(string chave, string entradaSaida, string[] ignorar, string contain);
        Task<List<Movimentacoes>> GetMovimentacoes(string chave, string[] movimentacoes, string entradaSaida, string[] ignorar, string contain);
    }
}
