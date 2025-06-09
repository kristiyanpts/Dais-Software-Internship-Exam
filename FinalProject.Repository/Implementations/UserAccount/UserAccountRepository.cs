using FinalProject.Repository.Base;
using FinalProject.Repository.Helpers;
using FinalProject.Repository.Interfaces.UserAccount;
using Microsoft.Data.SqlClient;

namespace FinalProject.Repository.Implementations.UserAccount
{
    public class UserAccountRepository : BaseRepository<Models.UserAccount>, IUserAccountRepository
    {
        public UserAccountRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        protected override string GetTableName() => "user_accounts";

        protected override IEnumerable<string> GetColumns() => new[] { "user_id", "account_id" };

        protected override Models.UserAccount MapReaderToEntity(SqlDataReader reader)
        {
            return new Models.UserAccount
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                AccountId = Convert.ToInt32(reader["account_id"]),
            };
        }

        protected override Dictionary<string, string> MapPropertiesToColumns()
        {
            return new Dictionary<string, string>
            {
                { "UserId", "user_id" },
                { "AccountId", "account_id" },
            };
        }

        public Task<int> Create(Models.UserAccount entity)
        {
            throw new NotImplementedException();
        }

        public Task<Models.UserAccount> Retrieve(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Models.UserAccount> Retrieve(string id)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Models.UserAccount> RetrieveAll(QueryParameters queryParameters = null)
        {
            return base.RetrieveAll(queryParameters);
        }

        public Task<bool> Update(int id, UserAccountUpdate update)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(string id, UserAccountUpdate update)
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