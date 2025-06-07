using System;

namespace Alertae.Models
{
    public class IncidenteEnergia
    {
        public int Id { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public string TipoFalha { get; set; } 
        public string Descricao { get; set; }
        public string Localizacao { get; set; }
        public string Impacto { get; set; } // Ex: "BAIXO", "MEDIO", "ALTO", "CRITICO"
        public DateTime DataOcorrencia { get; set; }
        public DateTime DataRegistro { get; set; }
        public string Status { get; set; } // Ex: "ABERTO", "EM ANALISE", "RESOLVIDO", "FECHADO"
        public string ObservacaoResolucao { get; set; }
    }
}