using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarteiraInvestimento.Domain.Entities;

namespace CarteiraInvestimento.Domain.Interfaces
{
    public interface IIndicadoresRepository
    {
        void AddRange(List<Indicadores> result);
    }
}
