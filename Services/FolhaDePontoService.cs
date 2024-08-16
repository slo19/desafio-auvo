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

namespace desafio_auvo.Services
{
    public class FolhaDePontoService : IFolhaDePontoService
    {
        private readonly IFolhaDePontoRepository _repository;

        public FolhaDePontoService(IFolhaDePontoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OrdemDePagamentoModel>> ProcessarFolhasDePonto(string caminho)
        {
            
            List<OrdemDePagamentoModel> ordensDePagamento = new();
            var folhasPorMes = await _repository.GetAll(caminho);

            if(folhasPorMes == null)
            {
                return null;
            }
            foreach(var folhaDePagamento in folhasPorMes)
            {
                var ordem = await InstanciarOrdem(folhaDePagamento.NomeArquivo);

                try
                {
                    foreach (var folha in folhaDePagamento.FolhasDePonto.GroupBy(f => f.Codigo).ToList())
                    {
                        var diasTrabalhados = await CalcularDiasTrabalhados(folha.ToList());
                        var diasExtras = diasTrabalhados - Calendario.DiasDoMes(ordem.MesVigencia, ordem.AnoVigencia);
                        var horasTrabalhadas = await CalcularHorasTrabalhadas(folha.ToList());
                        var tempoExtra = horasTrabalhadas - new TimeSpan(Calendario.DiasDoMes(ordem.MesVigencia, ordem.AnoVigencia)*8, 0, 0);
                        var horasExtra = tempoExtra.TotalHours;
                        var totalReceber = await CalcularPagamento(folha.ToList());
                        var funcionario = new FuncionarioModel
                        {
                            Codigo = folha.Key,
                            Nome = folha.First().Nome,
                            DiasTrabalhados = diasTrabalhados,
                            DiasExtras = diasExtras < 0 ? 0 : diasExtras,
                            DiasFalta = diasExtras < 0 ? diasExtras : 0,
                            TotalReceber = totalReceber,
                            HorasDebito = horasExtra < 0 ? horasExtra : 0,
                            HorasExtras = horasExtra > 0? horasExtra : 0
                        };

                        ordem.Funcionarios.Add(funcionario);
                    }
                    ordem = await CalcularTotais(ordem);
                    ordensDePagamento.Add(ordem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o arquivo {folhaDePagamento.NomeArquivo}");
                }
            }
            return ordensDePagamento;   
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

        private async Task<OrdemDePagamentoModel> CalcularTotais(OrdemDePagamentoModel ordem)
        {
            ordem.TotalPagar = ordem.Funcionarios.Sum(f => f.TotalReceber);

            return ordem;
        }
    }
}
