using System;

namespace Alertae.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }
        public string CPF { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}