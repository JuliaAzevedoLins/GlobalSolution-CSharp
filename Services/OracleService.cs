using Alertae.Config;
using Alertae.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Alertae.Services
{
    public class OracleService
    {
        // Método para gerar o hash da senha (SHA256)
        private string GerarHashSenha(string senha)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public Usuario ObterUsuarioPorNomeUsuarioESenha(string nomeUsuario, string senha)
        {
            string senhaHash = GerarHashSenha(senha);
            using (var connection = new OracleConnection(AppConfig.OracleConnectionString))
            {
                string sql = "SELECT ID_USUARIO, NM_USUARIO, DS_SENHA, NR_CPF, DT_CADASTRO FROM TB_USUARIOS WHERE NM_USUARIO = :nomeUsuario AND DS_SENHA = :senhaHash";
                using (var command = new OracleCommand(sql, connection))
                {
                    command.Parameters.Add(new OracleParameter("nomeUsuario", nomeUsuario));
                    command.Parameters.Add(new OracleParameter("senhaHash", senhaHash));

                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Usuario
                                {
                                    Id = reader.GetInt32(0),
                                    NomeUsuario = reader.GetString(1),
                                    Senha = reader.GetString(2), // Aqui já é o hash
                                    CPF = reader.GetString(3),
                                    DataCadastro = reader.GetDateTime(4)
                                };
                            }
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[ERRO] Falha ao validar login: {ex.Message}");
                        RegistrarLog(0, $"Erro na validação de login para '{nomeUsuario}': {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public bool RegistrarUsuario(Usuario usuario, Endereco endereco)
        {
            using (var connection = new OracleConnection(AppConfig.OracleConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Inserir Usuário
                        string sqlUsuario = "INSERT INTO TB_USUARIOS (NM_USUARIO, DS_SENHA, NR_CPF, DT_CADASTRO) VALUES (:nomeUsuario, :senhaHash, :cpf, :dataCadastro) RETURNING ID_USUARIO INTO :idUsuario";
                        using (var commandUsuario = new OracleCommand(sqlUsuario, connection))
                        {
                            commandUsuario.Parameters.Add(new OracleParameter("nomeUsuario", usuario.NomeUsuario));
                            commandUsuario.Parameters.Add(new OracleParameter("senhaHash", GerarHashSenha(usuario.Senha))); // Hash da senha
                            commandUsuario.Parameters.Add(new OracleParameter("cpf", usuario.CPF));
                            commandUsuario.Parameters.Add(new OracleParameter("dataCadastro", usuario.DataCadastro));

                            // Parâmetro de retorno para o ID do usuário
                            var idUsuarioParam = new OracleParameter("idUsuario", OracleDbType.Int32, System.Data.ParameterDirection.Output);
                            commandUsuario.Parameters.Add(idUsuarioParam);

                            commandUsuario.ExecuteNonQuery();
                            usuario.Id = ((Oracle.ManagedDataAccess.Types.OracleDecimal)idUsuarioParam.Value).ToInt32();
                        }

                        // 2. Inserir Endereço
                        string sqlEndereco = "INSERT INTO TB_ENDERECOS (ID_USUARIO, DS_CEP, NM_LOGRADOURO, NR_NUMERO, DS_COMPLEMENTO, NM_BAIRRO, NM_LOCALIDADE, SG_UF) VALUES (:idUsuario, :cep, :logradouro, :numero, :complemento, :bairro, :localidade, :uf)";
                        using (var commandEndereco = new OracleCommand(sqlEndereco, connection))
                        {
                            commandEndereco.Parameters.Add(new OracleParameter("idUsuario", usuario.Id));
                            commandEndereco.Parameters.Add(new OracleParameter("cep", endereco.Cep));
                            commandEndereco.Parameters.Add(new OracleParameter("logradouro", endereco.Logradouro));
                            // Tratamento para número do endereço e complemento
                            commandEndereco.Parameters.Add(new OracleParameter("numero", string.IsNullOrEmpty(endereco.Complemento) ? (object)DBNull.Value : endereco.Complemento));
                            commandEndereco.Parameters.Add(new OracleParameter("complemento", DBNull.Value));
                            commandEndereco.Parameters.Add(new OracleParameter("bairro", endereco.Bairro));
                            commandEndereco.Parameters.Add(new OracleParameter("localidade", endereco.Localidade));
                            commandEndereco.Parameters.Add(new OracleParameter("uf", endereco.Uf));

                            commandEndereco.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        RegistrarLog(usuario.Id, $"Usuário '{usuario.NomeUsuario}' cadastrado com sucesso.");
                        return true;
                    }
                    catch (OracleException ex)
                    {
                        transaction.Rollback();
                        if (ex.Number == 1)
                        {
                            Console.WriteLine("\n[ERRO] Nome de usuário ou CPF já cadastrado. Por favor, tente outro.");
                            RegistrarLog(0, $"Falha ao cadastrar usuário (Nome de usuário ou CPF duplicado): {ex.Message}");
                        }
                        else
                        {
                            Console.WriteLine($"\n[ERRO] Falha ao registrar usuário: {ex.Message}");
                            RegistrarLog(0, $"Erro inesperado ao registrar usuário: {ex.Message}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"\n[ERRO] Falha ao registrar usuário: {ex.Message}");
                        RegistrarLog(0, $"Erro geral ao registrar usuário: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public bool RegistrarIncidente(IncidenteEnergia incidente)
        {
            using (var connection = new OracleConnection(AppConfig.OracleConnectionString))
            {
                string sql = "INSERT INTO TB_INCIDENTES_ENERGIA (ID_USUARIO_REGISTRO, TP_FALHA, DS_INCIDENTE, DS_LOCALIZACAO, DS_IMPACTO, DT_OCORRENCIA, DT_REGISTRO, DS_STATUS) VALUES (:idUsuarioRegistro, :tipoFalha, :descricao, :localizacao, :impacto, :dataOcorrencia, :dataRegistro, :status)";
                using (var command = new OracleCommand(sql, connection))
                {
                    command.Parameters.Add(new OracleParameter("idUsuarioRegistro", incidente.IdUsuarioRegistro));
                    command.Parameters.Add(new OracleParameter("tipoFalha", incidente.TipoFalha));
                    command.Parameters.Add(new OracleParameter("descricao", incidente.Descricao));
                    command.Parameters.Add(new OracleParameter("localizacao", incidente.Localizacao));
                    command.Parameters.Add(new OracleParameter("impacto", incidente.Impacto));
                    command.Parameters.Add(new OracleParameter("dataOcorrencia", incidente.DataOcorrencia));
                    command.Parameters.Add(new OracleParameter("dataRegistro", incidente.DataRegistro));
                    command.Parameters.Add(new OracleParameter("status", incidente.Status));

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            RegistrarLog(incidente.IdUsuarioRegistro, $"Incidente '{incidente.Descricao}' registrado com sucesso.");
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[ERRO] Falha ao registrar incidente: {ex.Message}");
                        RegistrarLog(incidente.IdUsuarioRegistro, $"Erro ao registrar incidente: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public List<IncidenteEnergia> ObterTodosIncidentes()
        {
            var incidentes = new List<IncidenteEnergia>();
            using (var connection = new OracleConnection(AppConfig.OracleConnectionString))
            {
                string sql = "SELECT ID_INCIDENTE, ID_USUARIO_REGISTRO, TP_FALHA, DS_INCIDENTE, DS_LOCALIZACAO, DS_IMPACTO, DT_OCORRENCIA, DT_REGISTRO, DS_STATUS, OBS_RESOLUCAO FROM TB_INCIDENTES_ENERGIA ORDER BY DT_REGISTRO DESC";
                using (var command = new OracleCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                incidentes.Add(new IncidenteEnergia
                                {
                                    Id = reader.GetInt32(0),
                                    IdUsuarioRegistro = reader.GetInt32(1),
                                    TipoFalha = reader.GetString(2),
                                    Descricao = reader.GetString(3),
                                    Localizacao = reader.GetString(4),
                                    Impacto = reader.GetString(5),
                                    DataOcorrencia = reader.GetDateTime(6),
                                    DataRegistro = reader.GetDateTime(7),
                                    Status = reader.GetString(8),
                                    ObservacaoResolucao = reader.IsDBNull(9) ? null : reader.GetString(9)
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[ERRO] Falha ao obter incidentes: {ex.Message}");
                        RegistrarLog(0, $"Erro ao obter incidentes: {ex.Message}");
                    }
                }
            }
            return incidentes;
        }

        public bool AtualizarStatusIncidente(int idIncidente, string novoStatus, string observacaoResolucao = null)
        {
            using (var connection = new OracleConnection(AppConfig.OracleConnectionString))
            {
                string sql = "UPDATE TB_INCIDENTES_ENERGIA SET DS_STATUS = :novoStatus, OBS_RESOLUCAO = :observacaoResolucao WHERE ID_INCIDENTE = :idIncidente";
                using (var command = new OracleCommand(sql, connection))
                {
                    command.Parameters.Add(new OracleParameter("novoStatus", novoStatus));
                    command.Parameters.Add(new OracleParameter("observacaoResolucao", string.IsNullOrEmpty(observacaoResolucao) ? (object)DBNull.Value : observacaoResolucao));
                    command.Parameters.Add(new OracleParameter("idIncidente", idIncidente));

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            RegistrarLog(0, $"Status do incidente {idIncidente} atualizado para '{novoStatus}'.");
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[ERRO] Falha ao atualizar status do incidente: {ex.Message}");
                        RegistrarLog(0, $"Erro ao atualizar status do incidente {idIncidente}: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public List<string> ObterLogsEventos()
        {
            var logs = new List<string>();
            using (var connection = new OracleConnection(AppConfig.OracleConnectionString))
            {
                string sql = "SELECT DT_EVENTO, NVL(U.NM_USUARIO, 'Sistema'), DS_ACAO FROM TB_LOG_EVENTOS LE LEFT JOIN TB_USUARIOS U ON LE.ID_USUARIO = U.ID_USUARIO ORDER BY DT_EVENTO DESC";
                using (var command = new OracleCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add($"{reader.GetDateTime(0):dd/MM/yyyy HH:mm:ss} - [{reader.GetString(1)}] - {reader.GetString(2)}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[ERRO] Falha ao obter logs de eventos: {ex.Message}");
                    }
                }
            }
            return logs;
        }

        public void RegistrarLog(int idUsuario, string acao)
        {
            using (var connection = new OracleConnection(AppConfig.OracleConnectionString))
            {
                string sql = "INSERT INTO TB_LOG_EVENTOS (ID_USUARIO, DS_ACAO) VALUES (:idUsuario, :acao)";
                using (var command = new OracleCommand(sql, connection))
                {
                    command.Parameters.Add(new OracleParameter("idUsuario", idUsuario == 0 ? (object)DBNull.Value : idUsuario));
                    command.Parameters.Add(new OracleParameter("acao", acao));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[ALERTA] Falha ao registrar log de evento: {ex.Message}");
                    }
                }
            }
        }
    }
}