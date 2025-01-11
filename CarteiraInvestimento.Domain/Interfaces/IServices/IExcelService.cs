using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarteiraInvestimento.Domain.Entities;

namespace CarteiraInvestimento.Domain.Interfaces.IServices
{
    //polimorfismo
    public interface IExcelService
    {
        List<Movimentacoes> ReadMovimentacoes(Stream filePath, bool hasHeader = true);
    }
}
