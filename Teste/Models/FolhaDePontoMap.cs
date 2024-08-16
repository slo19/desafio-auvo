using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafio_auvo.Models
{
    public class FolhaDePontoMap : ClassMap<FolhaDePontoModel>
    {
        public FolhaDePontoMap() 
        {
            Map(m => m.Codigo).Name("Código");
            Map(m => m.Nome).Name("Nome");
            Map(m => m.ValorHora).Name("Valor hora");
            Map(m => m.Data).Name("Data");
            Map(m => m.Entrada).Name("Entrada");
            Map(m => m.Saida).Name("Saída");
            Map(m => m.Almoco).Name("Almoço");
        }
    }

}
