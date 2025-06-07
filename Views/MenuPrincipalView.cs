using Alertae.Models;
using Alertae.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alertae.Views
{
    public class MenuPrincipalView
    {
        private readonly OracleService _oracleService;
        private readonly IncidenteView _incidenteView;
        private readonly Usuario _usuarioLogado;

        public MenuPrincipalView(OracleService oracleService, Usuario usuarioLogado, IncidenteView incidenteView)
        {
            _oracleService = oracleService;
            _usuarioLogado = usuarioLogado;
            _incidenteView = incidenteView;
        }

        public void ExibirMenu()
        {
            void DesenharCabecalho()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╔═════════════════════════════════════════════════════════════╗");
                Console.WriteLine($"║        Bem-vindo, {_usuarioLogado.NomeUsuario,-42}║");
                Console.WriteLine("╠═════════════════════════════════════════════════════════════╣");
                Console.ResetColor();
            }

            Console.Clear();
            DesenharCabecalho();

            int opcao;
            do
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("║                      MENU PRINCIPAL                         ║");
                Console.WriteLine("╠═════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║ 1. Registrar Novo Incidente de Energia                      ║");
                Console.WriteLine("║ 2. Visualizar Incidentes Registrados                        ║");
                Console.WriteLine("║ 3. Atualizar Status de Incidente                            ║");
                Console.WriteLine("║ 4. Visualizar Logs de Eventos                               ║");
                Console.WriteLine("║ 5. Gerar Relatório de Status (Básico)                       ║");
                Console.WriteLine("║ 0. Sair                                                     ║");
                Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.Write("Escolha uma opção: ");

                if (int.TryParse(Console.ReadLine(), out opcao))
                {
                    Console.Clear();
                    DesenharCabecalho();
                    switch (opcao)
                    {
                        case 1:
                            _incidenteView.RegistrarNovoIncidente();
                            break;
                        case 2:
                            VisualizarIncidentes();
                            break;
                        case 3:
                            AtualizarStatusIncidente();
                            break;
                        case 4:
                            VisualizarLogsEventos();
                            break;
                        case 5:
                            GerarRelatorioStatus();
                            break;
                        case 0:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nSaindo do sistema. Até mais!");
                            Console.ResetColor();
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nOpção inválida. Tente novamente.");
                            Console.ResetColor();
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nEntrada inválida. Por favor, digite um número.");
                    Console.ResetColor();
                }
                if (opcao != 0)
                {
                    Console.WriteLine();
                    Console.Write("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                    DesenharCabecalho();
                }
            } while (opcao != 0);
        }

        private void VisualizarIncidentes()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                INCIDENTES REGISTRADOS                       ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            List<IncidenteEnergia> incidentes = _oracleService.ObterTodosIncidentes();

            if (incidentes.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nNenhum incidente registrado ainda.");
                Console.ResetColor();
                return;
            }

            foreach (var inc in incidentes.OrderByDescending(i => i.DataRegistro))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n──────────────────────────────────────────────────────────────");
                Console.WriteLine($"ID: {inc.Id}");
                Console.WriteLine($"Registrado por Usuário ID: {inc.IdUsuarioRegistro}");
                Console.WriteLine($"Tipo de Falha: {inc.TipoFalha}");
                Console.WriteLine($"Descrição: {inc.Descricao}");
                Console.WriteLine($"Localização: {inc.Localizacao}");
                Console.WriteLine($"Impacto: {inc.Impacto}");
                Console.WriteLine($"Data de Ocorrência: {inc.DataOcorrencia:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Data de Registro: {inc.DataRegistro:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Status: {inc.Status}");
                if (!string.IsNullOrEmpty(inc.ObservacaoResolucao))
                {
                    Console.WriteLine($"Observação Resolução: {inc.ObservacaoResolucao}");
                }
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n──────────────────────────────────────────────────────────────");
            Console.ResetColor();
        }

        private void AtualizarStatusIncidente()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║             ATUALIZAR STATUS DE INCIDENTE                   ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Digite o ID do incidente para atualizar: ");
            if (!int.TryParse(Console.ReadLine(), out int idIncidente))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n[ERRO] ID de incidente inválido.");
                Console.ResetColor();
                return;
            }

            string[] statusValidos = { "ABERTO", "EM ANALISE", "RESOLVIDO", "FECHADO" };
            string novoStatus;

            while (true)
            {
                Console.Write($"Novo Status ({string.Join(", ", statusValidos)}): ");
                novoStatus = Console.ReadLine()?.Trim().ToUpper();
                try
                {
                    if (string.IsNullOrWhiteSpace(novoStatus))
                    {
                        throw new ArgumentException("O status não pode ser vazio.");
                    }
                    if (Array.IndexOf(statusValidos, novoStatus) == -1)
                    {
                        throw new ArgumentException($"Status inválido. Escolha entre: {string.Join(", ", statusValidos)}.");
                    }
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }
            }

            string observacaoResolucao = null;
            if (novoStatus == "RESOLVIDO" || novoStatus == "FECHADO")
            {
                Console.Write("Observações da resolução (opcional): ");
                observacaoResolucao = Console.ReadLine();
            }

            if (_oracleService.AtualizarStatusIncidente(idIncidente, novoStatus, observacaoResolucao))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nStatus do incidente {idIncidente} atualizado para '{novoStatus}' com sucesso!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nErro ao atualizar o status do incidente {idIncidente}. Verifique se o ID está correto.");
                Console.ResetColor();
            }
        }

        private void VisualizarLogsEventos()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      LOGS DE EVENTOS                        ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            List<string> logs = _oracleService.ObterLogsEventos();

            if (logs.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nNenhum log de evento registrado ainda.");
                Console.ResetColor();
                return;
            }

            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
            Console.ResetColor();
        }

        private void GerarRelatorioStatus()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  RELATÓRIO DE STATUS                        ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            List<IncidenteEnergia> incidentes = _oracleService.ObterTodosIncidentes();

            if (incidentes.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nNenhum incidente registrado para gerar relatório.");
                Console.ResetColor();
                return;
            }

            int totalIncidentes = incidentes.Count;
            int abertos = incidentes.Count(i => i.Status == "ABERTO");
            int emAnalise = incidentes.Count(i => i.Status == "EM ANALISE");
            int resolvidos = incidentes.Count(i => i.Status == "RESOLVIDO");
            int fechados = incidentes.Count(i => i.Status == "FECHADO");

            Console.WriteLine($"\nTotal de Incidentes Registrados: {totalIncidentes}");
            Console.WriteLine($"Incidentes Abertos: {abertos}");
            Console.WriteLine($"Incidentes Em Análise: {emAnalise}");
            Console.WriteLine($"Incidentes Resolvidos: {resolvidos}");
            Console.WriteLine($"Incidentes Fechados: {fechados}");

            Console.WriteLine("\nIncidentes por Tipo de Falha:");
            var incidentesPorTipo = incidentes.GroupBy(i => i.TipoFalha)
                                              .Select(g => new { Tipo = g.Key, Count = g.Count() })
                                              .OrderByDescending(x => x.Count);
            foreach (var tipo in incidentesPorTipo)
            {
                Console.WriteLine($"- {tipo.Tipo}: {tipo.Count}");
            }

            Console.WriteLine("\nIncidentes por Nível de Impacto:");
            var incidentesPorImpacto = incidentes.GroupBy(i => i.Impacto)
                                                 .Select(g => new { Impacto = g.Key, Count = g.Count() })
                                                 .OrderByDescending(x => x.Count);
            foreach (var impacto in incidentesPorImpacto)
            {
                Console.WriteLine($"- {impacto.Impacto}: {impacto.Count}");
            }
            Console.ResetColor();
        }
    }
}
