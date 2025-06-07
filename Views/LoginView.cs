using Alertae.Models;
using Alertae.Services;
using System;

namespace Alertae.Views
{
    public class LoginView
    {
        private readonly OracleService _oracleService;
        private readonly CadastroView _cadastroView;

        public LoginView(OracleService oracleService, CadastroView cadastroView)
        {
            _oracleService = oracleService;
            _cadastroView = cadastroView;
        }

        public Usuario ExibirLogin()
        {
            void DesenharCabecalho()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║                      LOGIN                         ║");
                Console.WriteLine("╠════════════════════════════════════════════════════╣");
                Console.ResetColor();
            }

            Console.Clear();
            DesenharCabecalho();
            string nomeUsuario;
            string senha;
            Usuario usuarioLogado = null;

            while (usuarioLogado == null)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("║ Digite 'cadastrar' para criar uma nova conta       ║");
                Console.WriteLine("║ ou 'sair' para fechar o programa.                  ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();

                Console.Write("Nome de Usuário: ");
                nomeUsuario = Console.ReadLine();

                if (nomeUsuario.ToLower() == "cadastrar")
                {
                    _cadastroView.ExibirCadastro().Wait();
                    Console.Clear();
                    DesenharCabecalho();
                    continue;
                }
                else if (nomeUsuario.ToLower() == "sair")
                {
                    return null;
                }

                Console.Write("Senha: ");
                senha = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nomeUsuario) || string.IsNullOrWhiteSpace(senha))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n[AVISO] Nome de usuário e senha não podem ser vazios. Tente novamente.");
                    Console.ResetColor();
                    continue;
                }

                usuarioLogado = _oracleService.ObterUsuarioPorNomeUsuarioESenha(nomeUsuario, senha);

                if (usuarioLogado == null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n[ERRO] Nome de usuário ou senha inválidos. Tente novamente.");
                    Console.ResetColor();
                }
            }
            return usuarioLogado;
        }

    }
}