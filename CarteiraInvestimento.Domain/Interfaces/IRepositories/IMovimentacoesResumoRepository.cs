using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarteiraInvestimento.Domain.Entities;

namespace CarteiraInvestimento.Domain.Interfaces
{
    public interface IMovimentacoesResumoRepository
    {
        MovimentacoesResumo? Get(int id);
        MovimentacoesResumo? Get(string ticker);
        void AddRange(List<MovimentacoesResumo> list);
    }
}
