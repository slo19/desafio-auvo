using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafio_auvo.Models
{
    public class FolhasPorMesSetor
    {
        public string NomeArquivo { get; set; }
        public List<FolhaDePontoModel> FolhasDePonto {  get; set; }
    }
}
