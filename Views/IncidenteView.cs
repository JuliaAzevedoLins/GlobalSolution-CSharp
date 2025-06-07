using Alertae.Models;
using Alertae.Services;
using Alertae.Utils;
using System;

namespace Alertae.Views
{
    public class IncidenteView
    {
        private readonly OracleService _oracleService;
        private readonly Usuario _usuarioLogado;

        public IncidenteView(OracleService oracleService, Usuario usuarioLogado)
        {
            _oracleService = oracleService;
            _usuarioLogado = usuarioLogado;
        }

        public void RegistrarNovoIncidente()
        {
            void DesenharCabecalho()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                Console.WriteLine("║             REGISTRAR NOVO INCIDENTE DE ENERGIA          ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
                Console.ResetColor();
            }

            Console.Clear();
            DesenharCabecalho();

            string tipoFalha, descricao, localizacao, impactoStr, dataOcorrenciaStr;
            DateTime dataOcorrencia;
            string[] tiposValidos = { "CIBERNETICA", "FISICA", "DESCONHECIDA" };
            string[] impactosValidos = { "BAIXO", "MEDIO", "ALTO", "CRITICO" };

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"║ Tipo de Falha ({string.Join(", ", tiposValidos)}): ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                tipoFalha = Console.ReadLine()?.Trim().ToUpper();
                try
                {
                    if (string.IsNullOrWhiteSpace(tipoFalha))
                        throw new ArgumentException("O tipo de falha não pode ser vazio.");
                    if (Array.IndexOf(tiposValidos, tipoFalha) == -1)
                        throw new ArgumentException($"Tipo de falha inválido. Escolha entre: {string.Join(", ", tiposValidos)}.");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║ Descrição do Incidente: ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                descricao = Console.ReadLine()?.Trim();
                try
                {
                    if (string.IsNullOrWhiteSpace(descricao))
                        throw new ArgumentException("A descrição do incidente não pode ser vazia.");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║ Localização (Ex: Rua X, Setor Y): ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                localizacao = Console.ReadLine()?.Trim();
                try
                {
                    if (string.IsNullOrWhiteSpace(localizacao))
                        throw new ArgumentException("A localização não pode ser vazia.");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"║ Impacto ({string.Join(", ", impactosValidos)}): ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                impactoStr = Console.ReadLine()?.Trim().ToUpper();
                try
                {
                    if (string.IsNullOrWhiteSpace(impactoStr))
                        throw new ArgumentException("O impacto não pode ser vazio.");
                    if (Array.IndexOf(impactosValidos, impactoStr) == -1)
                        throw new ArgumentException($"Impacto inválido. Escolha entre: {string.Join(", ", impactosValidos)}.");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║ Data e Hora da Ocorrência (DD/MM/YYYY HH:MM): ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                dataOcorrenciaStr = Console.ReadLine();
                try
                {
                    if (string.IsNullOrWhiteSpace(dataOcorrenciaStr))
                        throw new ArgumentException("A data de ocorrência não pode ser vazia.");
                    if (!Validacao.ValidarData(dataOcorrenciaStr, out dataOcorrencia))
                        throw new ArgumentException("Formato de data e hora inválido. Use DD/MM/YYYY HH:MM.");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }
            }

            IncidenteEnergia novoIncidente = new IncidenteEnergia
            {
                IdUsuarioRegistro = _usuarioLogado.Id,
                TipoFalha = tipoFalha,
                Descricao = descricao,
                Localizacao = localizacao,
                Impacto = impactoStr,
                DataOcorrencia = dataOcorrencia,
                DataRegistro = DateTime.Now,
                Status = "ABERTO"
            };

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
            if (_oracleService.RegistrarIncidente(novoIncidente))
            {
                Console.WriteLine("║        Incidente registrado com sucesso!                   ║");
            }
            else
            {
                Console.WriteLine("║        Erro ao registrar incidente.                        ║");
            }
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }
    }
}
