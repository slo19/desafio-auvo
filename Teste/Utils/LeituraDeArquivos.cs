using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desafio_auvo.Utils
{
    internal class LeituraDeArquivos
    {
        public static async Task<List<string>> ListarArquivos(string caminho)
        {
            try
            {
                if (Directory.Exists(caminho))
                {
                    return Directory.GetFiles(caminho).Where(a => a.EndsWith(".csv")).ToList();
                }
            }
            catch (DirectoryNotFoundException e) { Console.WriteLine($"Diretório {caminho} não existe"); }
            catch (UnauthorizedAccessException e) { Console.WriteLine($"Não possui permissão para ler a pasta {caminho}"); }
            catch (PathTooLongException e) { Console.WriteLine("Caminho muito longo"); }
            catch (IOException e) { Console.WriteLine("Nâo foi possível ler a pasta"); }
            return null;
        }
    }
}
