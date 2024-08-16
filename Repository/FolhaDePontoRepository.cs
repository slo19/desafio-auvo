using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using desafio_auvo.Models;
using desafio_auvo.Repository.Interfaces;
using desafio_auvo.Utils;
using desafio_auvo.Models;

namespace desafio_auvo.Repository
{
    internal class FolhaDePontoRepository : IFolhaDePontoRepository
    {
        CsvConfiguration config = new CsvConfiguration(new CultureInfo("pt-BR", true))
        {
            HasHeaderRecord = true,
            NewLine = Environment.NewLine,
        };

        public async Task<List<FolhasPorMesSetor>> GetAll(string caminho)
        {
            List<string> arquivos = await LeituraDeArquivos.ListarArquivos(caminho);
            if(arquivos == null) {
                Console.WriteLine("Diretório não encontrado.");
                return null;
            }
                    
            List<FolhasPorMesSetor> folhasPorMes = new();
            foreach (var arquivo in arquivos)
            {  
                try
                {
                    using (var reader = new StreamReader(arquivo))
                    using (var csv = new CsvReader(reader, config))
                    {
                        csv.Context.RegisterClassMap<FolhaDePontoMap>();
                        FolhasPorMesSetor folhaDoMes = new();
                        folhaDoMes.FolhasDePonto = csv.GetRecords<FolhaDePontoModel>().ToList();
                        folhaDoMes.NomeArquivo = arquivo.Split("\\").Last().Split(".")[0];
                        folhasPorMes.Add(folhaDoMes);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Não foi possível ler arquivo {arquivo}");
                }
            }
            return folhasPorMes;
        }

        public async Task<FolhasPorMesSetor> Get(string caminho)
        {
            FolhasPorMesSetor folhaDoMes = new();
            
                try
                {
                    using (var reader = new StreamReader(caminho))
                    using (var csv = new CsvReader(reader, config))
                    {
                        csv.Context.RegisterClassMap<FolhaDePontoMap>();
                        folhaDoMes.FolhasDePonto = csv.GetRecords<FolhaDePontoModel>().ToList();
                        folhaDoMes.NomeArquivo = caminho.Split("\\").Last().Split(".")[0];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Não foi possível ler arquivo {caminho}");
                return null;
                }
            
            return folhaDoMes;
        }
    }
}
