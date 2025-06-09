using Microsoft.Data.SqlClient;
using FinalProject.Repository.Base;
using FinalProject.Repository.Helpers;
using FinalProject.Repository.Interfaces.User;

namespace FinalProject.Repository.Implementations.User
{
    public class UserRepository : BaseRepository<Models.User>, IUserRepository
    {
        public UserRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        private const string _idField = "id";
        protected override string GetTableName() => "users";

        protected override IEnumerable<string> GetColumns() => new[] { _idField, "email", "password", "full_name" };

        protected override Models.User MapReaderToEntity(SqlDataReader reader)
        {
            return new Models.User
            {
                Id = Convert.ToInt32(reader[_idField]),
                Email = Convert.ToString(reader["email"]),
                Password = Convert.ToString(reader["password"]),
                FullName = Convert.ToString(reader["full_name"]),
            };
        }

        protected override Dictionary<string, string> MapPropertiesToColumns()
        {
            return new Dictionary<string, string>
            {
                { "Id", _idField },
                { "Email", "email" },
                { "Password", "password" },
                { "FullName", "full_name" },
            };
        }

        public Task<int> Create(Models.User entity)
        {
            return base.Create(entity, _idField);
        }

        public Task<Models.User> Retrieve(int id)
        {
            return base.Retrieve(_idField, id);
        }

        public Task<Models.User> Retrieve(string id)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Models.User> RetrieveAll(QueryParameters queryParameters = null)
        {
            return base.RetrieveAll(queryParameters);
        }

        public async Task<bool> Update(int id, UserUpdate update)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(string id, UserUpdate update)
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