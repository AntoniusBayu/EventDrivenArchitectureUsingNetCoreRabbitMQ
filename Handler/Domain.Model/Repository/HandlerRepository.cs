using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class HandlerRepository : IHandlerRepository
    {
        private readonly string connectionString;

        public HandlerRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> Insert(HandlerModel data)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await connection.ExecuteAsync("spInsertHandler", new { _orderid = data.OrderID, _result = data.Result, _message = data.Message }, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                transaction.Commit();
                return data.OrderID;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return -1;
            }
        }

        public async Task Update(HandlerModel data)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await connection.ExecuteAsync("spUpdateHandler", new { _orderid = data.OrderID, _result = data.Result, _message = data.Message }, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }
}
