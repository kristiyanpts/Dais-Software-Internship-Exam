using FinalProject.Repository.Base;
using FinalProject.Repository.Helpers;
using FinalProject.Repository.Interfaces.Payment;
using Microsoft.Data.SqlClient;

namespace FinalProject.Repository.Implementations.Payment
{
    public class PaymentRepository : BaseRepository<Models.Payment>, IPaymentRepository
    {
        public PaymentRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        private const string _idField = "id";
        protected override string GetTableName() => "payments";

        protected override IEnumerable<string> GetColumns() => new[] { _idField, "user_id", "from_account_id", "to_account_id", "amount", "description", "status", "created_at", };

        protected override Models.Payment MapReaderToEntity(SqlDataReader reader)
        {
            return new Models.Payment
            {
                Id = Convert.ToInt32(reader[_idField]),
                UserId = Convert.ToInt32(reader["user_id"]),
                FromAccountId = Convert.ToInt32(reader["from_account_id"]),
                ToAccountId = Convert.ToInt32(reader["to_account_id"]),
                Amount = Convert.ToDecimal(reader["amount"]),
                Description = Convert.ToString(reader["description"]),
                Status = Enum.Parse<Models.PaymentStatus>(Convert.ToString(reader["status"])),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
            };
        }

        protected override Dictionary<string, string> MapPropertiesToColumns()
        {
            return new Dictionary<string, string>
            {
                { "Id", _idField },
                { "UserId", "user_id" },
                { "FromAccountId", "from_account_id" },
                { "ToAccountId", "to_account_id" },
                { "Amount", "amount" },
                { "Description", "description" },
                { "Status", "status" },
                { "CreatedAt", "created_at" },
            };
        }

        public async Task<int> Create(Models.Payment entity)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            var tableName = GetTableName();

            var properties = entity.GetType().GetProperties()
                .Where(p => _propertyToColumnMap[p.Name] != _idField)
                .ToList();

            var columns = properties.Select(p => _propertyToColumnMap[p.Name]).ToList();

            command.CommandText = QueryHelper.BuildInsertQuery(tableName, columns);

            foreach (var property in properties)
            {
                var value = property.GetValue(entity) ?? DBNull.Value;

                if (property.Name == "Status")
                {
                    value = Convert.ToString(property.GetValue(entity));
                }

                var columnName = _propertyToColumnMap[property.Name];

                command.Parameters.AddWithValue($"@{columnName}", value);
            }

            var id = await command.ExecuteScalarAsync();

            id = id == DBNull.Value ? -1 : id;

            return Convert.ToInt32(id);
        }

        public Task<Models.Payment> Retrieve(int id)
        {
            return base.Retrieve(_idField, id);
        }

        public Task<Models.Payment> Retrieve(string id)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Models.Payment> RetrieveAll(QueryParameters queryParameters = null)
        {
            return base.RetrieveAll(queryParameters);
        }

        public async Task<bool> Update(int id, PaymentUpdate update)
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

                    if (property.Name == "Status")
                    {
                        value = Convert.ToString(property.GetValue(update));
                    }

                    parameters.AddUpdateField(columnName, value);
                }
            }

            command.CommandText = QueryHelper.BuildUpdateQuery(GetTableName(), parameters);
            AddParametersToCommand(command, parameters);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public Task<bool> Update(string id, PaymentUpdate update)
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