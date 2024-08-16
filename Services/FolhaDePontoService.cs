using desafio_auvo.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using desafio_auvo.Repository.Interfaces;
using desafio_auvo.Repository;
using System.Reflection.Metadata.Ecma335;
using desafio_auvo.Models;
using desafio_auvo.Utils;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections.Concurrent;

namespace desafio_auvo.Services
{
    public class FolhaDePontoService : IFolhaDePontoService
    {
        private readonly IFolhaDePontoRepository _repository;

        public FolhaDePontoService(IFolhaDePontoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OrdemDePagamentoModel>> ProcessarFolhasDePontoAsync(string caminho)
        {
                var arquivos = await LeituraDeArquivos.ListarArquivos(caminho);

            if(arquivos == null || arquivos.Count == 0)
            {
                Console.WriteLine("Não foi encontrado nenhum arquivo");
                return null;
            }

                var ordensProcessadas = new ConcurrentBag<OrdemDePagamentoModel>();

                var tasks = new List<Task>();

                foreach (var arquivo in arquivos)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var folhaDePonto = await _repository.Get(arquivo);
                        var ordemDePagamento = await  ProcessarFolhas(folhaDePonto);
                        if (ordemDePagamento != null)
                            ordensProcessadas.Add(ordemDePagamento);
                    }));
                }

                await Task.WhenAll(tasks);

                return ordensProcessadas.ToList();
        }

        private async Task<OrdemDePagamentoModel> ProcessarFolhas (FolhasPorMesSetor folhas)
        {
            var ordem = await InstanciarOrdem(folhas.NomeArquivo);

            try
            {
                decimal totalExtras = 0;
                decimal totalDebito = 0;
                foreach (var folha in folhas.FolhasDePonto.GroupBy(f => f.Codigo).ToList())
                {
                    var diasTrabalhados = await CalcularDiasTrabalhados(folha.ToList());
                    var diasExtras = diasTrabalhados - Calendario.DiasDoMes(ordem.MesVigencia, ordem.AnoVigencia);
                    var horasTrabalhadas = await CalcularHorasTrabalhadas(folha.ToList());
                    var tempoExtra = horasTrabalhadas - new TimeSpan(Calendario.DiasDoMes(ordem.MesVigencia, ordem.AnoVigencia) * 8, 0, 0);
                    var horasExtra = tempoExtra.TotalHours;
                    var totalReceber = await CalcularPagamento(folha.ToList());
                        totalExtras += decimal.Parse(horasExtra.ToString()) * (await ConverterValorHora(folha.First().ValorHora));
                        totalDebito += decimal.Parse((horasExtra * -1).ToString()) * (await ConverterValorHora(folha.First().ValorHora));
                        var funcionario = new FuncionarioModel
                    {
                        Codigo = folha.Key,
                        Nome = folha.First().Nome,
                        DiasTrabalhados = diasTrabalhados,
                        DiasExtras = diasExtras < 0 ? 0 : diasExtras,
                        DiasFalta = diasExtras < 0 ? diasExtras : 0,
                        TotalReceber = Arredondar(totalReceber),
                        HorasDebito = horasExtra < 0 ? Arredondar(horasExtra) : 0,
                        HorasExtras = horasExtra > 0 ? Arredondar(horasExtra) : 0
                    };

                    ordem.Funcionarios.Add(funcionario);
                }
            
                ordem = await CalcularTotais(ordem , totalExtras, totalDebito);
                return ordem;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar o arquivo {folhas.NomeArquivo}");
                return null;
            }
        }
        private async Task<OrdemDePagamentoModel> InstanciarOrdem(string nomeArquivo) 
            {
            var arrayNome = nomeArquivo.Split("-").ToArray();
            var ordemDePagamento = new OrdemDePagamentoModel
            {
                Departamento = arrayNome[0],
                MesVigencia = arrayNome[1],
                AnoVigencia = int.Parse(arrayNome[2])
            };

            return ordemDePagamento;
        }

        private async Task<int> CalcularDiasTrabalhados(List<FolhaDePontoModel> folhasDePonto )
        {
            return folhasDePonto.Count();
        }

        private async Task<TimeSpan> CalcularTempoDeAlmoco(string almoco)
        {
            var horarios = almoco.Split("-");
            return TimeSpan.Parse(horarios[1].Trim()) - TimeSpan.Parse(horarios[0].Trim());
        }

        private async Task<TimeSpan> CalcularHorasTrabalhadas(List<FolhaDePontoModel> folhasDePonto)
        {
            TimeSpan horas = new();

            foreach(var folha in folhasDePonto)
            {
                horas += folha.Saida - folha.Entrada - (await CalcularTempoDeAlmoco(folha.Almoco));
            }

            return horas;
        }

        private async Task<decimal> CalcularPagamento(List<FolhaDePontoModel> folhasDePonto)
        {
            decimal total = 0;
            TimeSpan horas = new();

            foreach (var folha in folhasDePonto)
            {
                horas = folha.Saida - folha.Entrada - (await CalcularTempoDeAlmoco(folha.Almoco));
                var horasTotais = decimal.Parse(horas.TotalHours.ToString());
                total +=  horasTotais * (await ConverterValorHora(folha.ValorHora));
            }

            return total;
        }

        private async Task<decimal> ConverterValorHora(string valorHora)
        {
            string valor = valorHora.Split("$")[1].Trim();
            return decimal.Parse(valor);
        }

        private async Task<OrdemDePagamentoModel> CalcularTotais(OrdemDePagamentoModel ordem, decimal totalExtras, decimal totalDebito)
        {
            ordem.TotalPagar = Arredondar(ordem.Funcionarios.Sum(f => f.TotalReceber));
            ordem.TotalExtras = totalExtras > 0 ? Arredondar(totalExtras) : 0;
            ordem.TotalDescontos = totalDebito > 0 ? Arredondar(totalDebito) : 0;
            return ordem;
        }

        private decimal Arredondar(decimal val)
        {
            return decimal.Round(val, 2, MidpointRounding.AwayFromZero);
        }
        private double Arredondar(double val)
        {
            return double.Round(val, 2, MidpointRounding.AwayFromZero);
        }

    }
}
