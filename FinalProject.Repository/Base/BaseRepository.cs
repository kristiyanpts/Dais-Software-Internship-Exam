using FinalProject.Repository.Helpers;
using Microsoft.Data.SqlClient;

namespace FinalProject.Repository.Base
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly ConnectionFactory _connectionFactory;
        protected Dictionary<string, string> _propertyToColumnMap;
        protected BaseRepository(ConnectionFactory connectionFactory)
        {
            _propertyToColumnMap = MapPropertiesToColumns();
            _connectionFactory = connectionFactory;
        }

        protected abstract string GetTableName();
        protected abstract IEnumerable<string> GetColumns();
        protected abstract T MapReaderToEntity(SqlDataReader reader);
        protected abstract Dictionary<string, string> MapPropertiesToColumns();

        protected async Task<int> Create(T entity, string idColumnName = null)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            var tableName = GetTableName();

            var properties = entity.GetType().GetProperties()
                .Where(p => _propertyToColumnMap[p.Name] != idColumnName)
                .ToList();

            var columns = properties.Select(p => _propertyToColumnMap[p.Name]).ToList();

            command.CommandText = QueryHelper.BuildInsertQuery(tableName, columns);

            foreach (var property in properties)
            {
                var value = property.GetValue(entity) ?? DBNull.Value;
                var columnName = _propertyToColumnMap[property.Name];

                command.Parameters.AddWithValue($"@{columnName}", value);
            }

            var id = await command.ExecuteScalarAsync();

            id = id == DBNull.Value ? -1 : id;

            return Convert.ToInt32(id);
        }

        protected async Task<T> Retrieve(string idColumnName, int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            var tableName = GetTableName();

            var parameters = new QueryParameters();
            parameters.AddWhere(idColumnName, id);

            var columns = parameters.SelectedColumns.Any()
                ? parameters.SelectedColumns
                : GetColumns();

            command.CommandText = QueryHelper.BuildSelectQuery(tableName, columns, parameters);
            AddParametersToCommand(command, parameters);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToEntity(reader);
            }

            return null;
        }

        protected async Task<T> Retrieve(string idColumnName, string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            var tableName = GetTableName();

            var parameters = new QueryParameters();
            parameters.AddWhere(idColumnName, id);

            var columns = parameters.SelectedColumns.Any()
                ? parameters.SelectedColumns
                : GetColumns();

            command.CommandText = QueryHelper.BuildSelectQuery(tableName, columns, parameters);
            AddParametersToCommand(command, parameters);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToEntity(reader);
            }

            return null;
        }

        protected async IAsyncEnumerable<T> RetrieveAll(QueryParameters parameters = null)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            var tableName = GetTableName();

            parameters ??= new QueryParameters();
            var columns = parameters.SelectedColumns.Any()
                ? parameters.SelectedColumns
                : GetColumns();

            command.CommandText = QueryHelper.BuildSelectQuery(tableName, columns, parameters);
            AddParametersToCommand(command, parameters);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return MapReaderToEntity(reader);
            }
        }

        protected async Task<bool> Delete(string idColumnName, int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            var tableName = GetTableName();

            var parameters = new QueryParameters();

            if (idColumnName != null) parameters.AddWhere(idColumnName, id);

            command.CommandText = QueryHelper.BuildDeleteQuery(tableName, parameters);
            AddParametersToCommand(command, parameters);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        protected async Task<bool> Delete(string idColumnName, string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            var tableName = GetTableName();

            var parameters = new QueryParameters();

            if (idColumnName != null) parameters.AddWhere(idColumnName, id);

            command.CommandText = QueryHelper.BuildDeleteQuery(tableName, parameters);
            AddParametersToCommand(command, parameters);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        protected void AddParametersToCommand(SqlCommand command, QueryParameters parameters)
        {
            if (parameters == null) return;

            var parameterValues = parameters.GetParameters();
            foreach (var param in parameterValues)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
            }
        }
    }
}