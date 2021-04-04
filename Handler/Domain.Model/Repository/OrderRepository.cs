using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string connectionString;

        public OrderRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> Create(Order orderDetail)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await connection.ExecuteAsync("spInsertOrder", new { _OrderID = orderDetail.OrderID, _Quantity = orderDetail.Quantity, _ProductID = orderDetail.ProductID }, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                transaction.Commit();
                return orderDetail.OrderID;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return -1;
            }
        }

        public async Task Delete(int orderId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync("DELETE FROM [Order] WHERE OrderID = @orderId", new { orderId }, transaction: transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }
    }
}
