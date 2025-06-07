using Alertae.Models;
using Alertae.Services;
using Alertae.Views;
using System;
using System.Threading.Tasks;

namespace Alertae
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "AlertaE - Sistema de Gestão de Incidentes de Energia";

            var oracleService = new OracleService();
            var viaCepService = new ViaCepService();

            var cadastroView = new CadastroView(oracleService, viaCepService);
            var loginView = new LoginView(oracleService, cadastroView);

            var menuInicial = new MenuInicial(loginView, cadastroView);

            Usuario usuarioLogado = null;

            usuarioLogado = await menuInicial.ExibirMenu();

            if (usuarioLogado == null)
            {
                Console.WriteLine("Programa encerrado.");
                return;
            }

            var incidenteView = new IncidenteView(oracleService, usuarioLogado);

            var menuPrincipalView = new MenuPrincipalView(oracleService, usuarioLogado, incidenteView);
            menuPrincipalView.ExibirMenu();

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}