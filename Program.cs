using desafio_auvo.Repository;
using desafio_auvo.Repository.Interfaces;
using desafio_auvo.Services;
using desafio_auvo.Services.Interfaces;
using desafio_auvo.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace desafio_auvo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var folhaDePontoService = serviceProvider.GetService<IFolhaDePontoService>();

            string caminho = "";
            if (args.Length < 1)
            {
                Console.WriteLine("Digite o caminho da pasta com os arquivos:");
                caminho  = Console.ReadLine();
            }
            
            else
            {
                caminho = args[0];
            }

            var arquivos = await folhaDePontoService.ProcessarFolhasDePontoAsync(caminho);

            if(arquivos == null || arquivos.Count() == 0)
            {
                Console.WriteLine("Tente outro diretório");                
            }
            else
            {
                Serializador.SalvarJSON(arquivos, caminho);
            }
            
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IFolhaDePontoService, FolhaDePontoService>()
                .AddScoped<IFolhaDePontoRepository, FolhaDePontoRepository>();
        }
    }
}