using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace desafio_auvo.Models
{
    public class FolhaDePontoModel
    {
        [Name("Código")]
        public int Codigo { get; set; }
        [Name("Nome")]
        public string Nome { get; set; }
        [Name("Valor hora")]
        public string ValorHora { get; set; }
        [Name("Data")]
        public DateOnly Data { get; set; }
        [Name("Entrada")]
        public TimeOnly Entrada { get; set; }
        [Name("Saída")]
        public TimeOnly Saida { get; set; }
        [Name("Almoço")]
        public string Almoco { get; set; }
    }
}
