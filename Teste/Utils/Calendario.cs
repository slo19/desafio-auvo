using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafio_auvo.Utils
{
    public class Calendario
    {
        public static int DiasDoMes(string mes, int ano)
        {
            mes = mes.ToLower();

            if (mes == "fevereiro" && ano % 4 == 0)
            {
                return 29;
            }

            Dictionary<string, int> meses = new Dictionary<string, int> 
            {
                {"janeiro", 31 },
                {"fevereiro", 28 },
                {"março", 31 },
                {"abril", 30 },
                {"maio", 31 },
                {"junho", 30 },
                {"julho", 31 },
                {"agosto", 31 },
                {"setembro", 30 },
                {"outubro", 31 },
                {"novembro", 30 },
                {"dezembro", 31 },
                
            };

            return meses[mes];
        }
    }
}
