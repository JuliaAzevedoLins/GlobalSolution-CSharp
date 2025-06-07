using Alertae.Models;
using Alertae.Services;
using Alertae.Utils;
using System;
using System.Threading.Tasks;

namespace Alertae.Views
{
    public class CadastroView
    {
        private readonly OracleService _oracleService;
        private readonly ViaCepService _viaCepService;

        public CadastroView(OracleService oracleService, ViaCepService viaCepService)
        {
            _oracleService = oracleService;
            _viaCepService = viaCepService;
        }

        public async Task ExibirCadastro()
        {
            void DesenharCabecalho()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                  CADASTRO DE NOVO USUÁRIO                ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
                Console.ResetColor();
            }

            Console.Clear();
            DesenharCabecalho();

            string nomeUsuario, senha, confirmaSenha, cpf, cep, numeroEndereco;
            Endereco endereco = null;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("║ Nome de Usuário: ".PadRight(59) + "║\n");
            Console.ResetColor();
            Console.Write("» ");
            nomeUsuario = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nomeUsuario))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n[ERRO] Nome de usuário não pode ser vazio.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║ Senha (Mínimo 8 caracteres, com maiúsculas, minúsculas, números e especiais):\n");
                Console.Write("║ ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                senha = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║ Confirme a Senha: ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                confirmaSenha = Console.ReadLine();

                try
                {
                    if (string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(confirmaSenha))
                    {
                        throw new ArgumentException("Senha e confirmação de senha não podem ser vazias.");
                    }
                    if (senha != confirmaSenha)
                    {
                        throw new ArgumentException("As senhas não coincidem. Tente novamente.");
                    }
                    if (!Validacao.ValidarSenha(senha))
                    {
                        throw new ArgumentException("A senha não atende aos requisitos de segurança.");
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

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║ CPF (apenas números): ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                cpf = Console.ReadLine();
                try
                {
                    if (string.IsNullOrWhiteSpace(cpf))
                    {
                        throw new ArgumentException("CPF não pode ser vazio.");
                    }
                    if (!Validacao.ValidarCPF(cpf))
                    {
                        throw new ArgumentException("CPF inválido. Tente novamente.");
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

            while (endereco == null)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║ CEP: ".PadRight(59) + "║\n");
                Console.ResetColor();
                Console.Write("» ");
                cep = Console.ReadLine();
                try
                {
                    if (string.IsNullOrWhiteSpace(cep))
                    {
                        throw new ArgumentException("CEP não pode ser vazio.");
                    }
                    endereco = await _viaCepService.ConsultarCep(cep);
                    if (endereco == null)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Não foi possível encontrar o endereço para o CEP informado. Tente novamente.");
                        Console.ResetColor();
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╠═════════════════════════════════════════════════════════ ╣");
            Console.WriteLine("║ Endereço encontrado:                                     ║");
            Console.WriteLine($"║ Logradouro: {endereco.Logradouro,-44} ║");
            Console.WriteLine($"║ Bairro: {endereco.Bairro,-49}║");
            Console.WriteLine($"║ Cidade: {endereco.Localidade}/{endereco.Uf,-41} ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════ ╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("║ Número (opcional): ".PadRight(59) + "║\n");
            Console.ResetColor();
            Console.Write("» ");
            numeroEndereco = Console.ReadLine();
            endereco.Complemento = numeroEndereco;

            Usuario novoUsuario = new Usuario
            {
                NomeUsuario = nomeUsuario,
                Senha = senha,
                CPF = cpf,
                DataCadastro = DateTime.Now
            };

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╠═════════════════════════════════════════════════════════╣");
            if (_oracleService.RegistrarUsuario(novoUsuario, endereco))
            {
                Console.WriteLine("║           Usuário cadastrado com sucesso!              ║");
            }
            else
            {
                Console.WriteLine("║      Erro ao cadastrar usuário. Verifique os dados.    ║");
            }
            Console.WriteLine("╚═════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
