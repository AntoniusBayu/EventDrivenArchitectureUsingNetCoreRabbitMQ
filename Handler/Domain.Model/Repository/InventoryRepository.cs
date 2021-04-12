using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly string connectionString;

        public InventoryRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task Update(int productID, int quantity)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await connection.ExecuteAsync("UPDATE INVENTORY SET STOCK = STOCK - @orderCount WHERE ProductID = @ProductID", new { orderCount = quantity, ProductID = productID }, transaction: transaction, commandType: System.Data.CommandType.Text);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
