using desafio_auvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafio_auvo.Repository.Interfaces
{
    public interface IFolhaDePontoRepository 
    {
        Task<List<FolhasPorMesSetor>> GetAll(string caminho);
    }
}
