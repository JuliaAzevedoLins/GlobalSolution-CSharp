using Alertae.Config;
using Alertae.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Alertae.Services
{
    public class ViaCepService
    {
        private readonly HttpClient _httpClient;

        public ViaCepService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Endereco> ConsultarCep(string cep)
        {
            cep = cep.Replace("-", "").Replace(".", "");

            if (cep.Length != 8)
            {
                Console.WriteLine("\n[ERRO] CEP deve conter 8 dígitos.");
                return null;
            }

            string url = string.Format(AppConfig.ViaCepApiUrl, cep);

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                Endereco endereco = JsonSerializer.Deserialize<Endereco>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (endereco != null && endereco.Erro)
                {
                    Console.WriteLine("\n[AVISO] CEP não encontrado.");
                    return null;
                }

                return endereco;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"\n[ERRO] Erro de comunicação com a API ViaCEP: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"\n[ERRO] Erro ao processar resposta da API ViaCEP: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERRO] Ocorreu um erro inesperado ao consultar o CEP: {ex.Message}");
                return null;
            }
        }
    }
}