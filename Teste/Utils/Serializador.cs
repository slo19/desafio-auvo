using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using desafio_auvo.Models;
using System.Runtime.InteropServices;

namespace desafio_auvo.Utils
{
    public class Serializador
    {

        public static void SalvarJSON(List<OrdemDePagamentoModel> ordensDePagamento, string caminho)
        {
            string saida = "";
            string json = JsonConvert.SerializeObject(ordensDePagamento);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                saida = $"{caminho}\\saida.json";
            else
                saida = $"{caminho}/saida.json";

            File.WriteAllText(saida, json);

            Console.WriteLine($"Arquivo salvo em {saida}");
        }
    }
}
