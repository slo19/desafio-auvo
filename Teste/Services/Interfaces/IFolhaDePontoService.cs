using desafio_auvo.Models;
using desafio_auvo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafio_auvo.Services.Interfaces
{
    public interface IFolhaDePontoService
    {
        //Task<List<OrdemDePagamentoModel>> ProcessarFolhasDePonto(string caminho);
        Task<List<OrdemDePagamentoModel>> ProcessarFolhasDePontoAsync(string caminho);
    }
}
