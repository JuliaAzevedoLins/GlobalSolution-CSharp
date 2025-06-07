using Alertae.Models;
using Alertae.Services;
using System;
using System.Threading.Tasks;

namespace Alertae.Views
{
    public class MenuInicial
    {
        private readonly LoginView _loginView;
        private readonly CadastroView _cadastroView;

        public MenuInicial(LoginView loginView, CadastroView cadastroView)
        {
            _loginView = loginView;
            _cadastroView = cadastroView;
        }

        public async Task<Usuario> ExibirMenu()
        {
            int opcao;
            Usuario usuarioLogado = null;

            void DesenharCabecalho()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╔═════════════════════════════════════════════════════╗");
                Console.WriteLine("║                  ALERTAE - Bem-vindo                ║");
                Console.WriteLine("╠═════════════════════════════════════════════════════╣");
                Console.ResetColor();
            }

            do
            {
                Console.Clear();
                DesenharCabecalho();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("║                     MENU INICIAL                    ║");
                Console.WriteLine("╠═════════════════════════════════════════════════════╣");
                Console.WriteLine("║ 1. Login                                            ║");
                Console.WriteLine("║ 2. Cadastro                                         ║");
                Console.WriteLine("║ 0. Sair                                             ║");
                Console.WriteLine("╚═════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.Write("Opção: ");

                if (int.TryParse(Console.ReadLine(), out opcao))
                {
                    switch (opcao)
                    {
                        case 1:
                            usuarioLogado = _loginView.ExibirLogin();
                            if (usuarioLogado != null)
                            {
                                return usuarioLogado;
                            }
                            break;
                        case 2:
                            await _cadastroView.ExibirCadastro();
                            break;
                        case 0:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nSaindo do programa AlertaE.");
                            Console.ResetColor();
                            return null;
                        default:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\nOpção inválida. Pressione qualquer tecla para continuar...");
                            Console.ResetColor();
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nEntrada inválida. Digite um número. Pressione qualquer tecla para continuar...");
                    Console.ResetColor();
                    Console.ReadKey();
                }
            } while (usuarioLogado == null);

            return usuarioLogado;
        }
    }
}
