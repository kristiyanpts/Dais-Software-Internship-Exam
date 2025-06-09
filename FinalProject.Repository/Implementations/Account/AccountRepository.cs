using FinalProject.Repository.Base;
using FinalProject.Repository.Helpers;
using FinalProject.Repository.Interfaces.Account;
using Microsoft.Data.SqlClient;

namespace FinalProject.Repository.Implementations.Account
{
    public class AccountRepository : BaseRepository<Models.Account>, IAccountRepository
    {
        public AccountRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        private const string _idField = "id";
        protected override string GetTableName() => "accounts";

        protected override IEnumerable<string> GetColumns() => new[] { _idField, "account_number", "balance" };

        protected override Models.Account MapReaderToEntity(SqlDataReader reader)
        {
            return new Models.Account
            {
                Id = Convert.ToInt32(reader[_idField]),
                AccountNumber = Convert.ToString(reader["account_number"]),
                Balance = Convert.ToDecimal(reader["balance"]),
            };
        }

        protected override Dictionary<string, string> MapPropertiesToColumns()
        {
            return new Dictionary<string, string>
            {
                { "Id", _idField },
                { "AccountNumber", "account_number" },
                { "Balance", "balance" },
            };
        }

        public Task<int> Create(Models.Account entity)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Account> Retrieve(int id)
        {
            return base.Retrieve(_idField, id);
        }

        public Task<Models.Account> Retrieve(string id)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Models.Account> RetrieveAll(QueryParameters queryParameters = null)
        {
            return base.RetrieveAll(queryParameters);
        }

        public async Task<bool> Update(int id, AccountUpdate update)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = connection.CreateCommand();

            var parameters = new QueryParameters();
            parameters.AddWhere(_idField, id);

            foreach (var property in update.GetType().GetProperties())
            {
                var value = property.GetValue(update);
                if (value != null)
                {
                    var columnName = _propertyToColumnMap.ContainsKey(property.Name) ? _propertyToColumnMap[property.Name] : property.Name;
                    parameters.AddUpdateField(columnName, value);
                }
            }

            command.CommandText = QueryHelper.BuildUpdateQuery(GetTableName(), parameters);
            AddParametersToCommand(command, parameters);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public Task<bool> Update(string id, AccountUpdate update)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}