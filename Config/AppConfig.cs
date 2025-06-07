namespace Alertae.Config
{
    public static class AppConfig
    {
        public static string OracleConnectionString { get; } = "User ID=RM98690;Password=041003;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));";
        public static string ViaCepApiUrl = "https://viacep.com.br/ws/{0}/json/";
    }
}