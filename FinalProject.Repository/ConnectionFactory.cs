using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FinalProject.Repository
{
    public class ConnectionFactory
    {
        private string _connectionString;

        public ConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<SqlConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}