using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.Internal.DataAccess
{
    public class SqlDataAccess : IDisposable, ISqlDataAccess
    {
        private bool isClosed = false;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private readonly IConfiguration _config; // Is built-in in .NET CORE
        private readonly ILogger<SqlDataAccess> _logger;

        public SqlDataAccess(IConfiguration config,
            ILogger<SqlDataAccess> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GetConnectionString(string name)
        {
            //return ConfigurationManager.ConnectionStrings[name].ConnectionString; //NET FRAMEWORK
            return _config.GetConnectionString(name); // CORE
        }


        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure,
                    parameters, commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }


        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {

                connection.Execute(storedProcedure,
                        parameters, commandType: CommandType.StoredProcedure);

            }
        }


        public int CreateNotebook(string storedProcedure, NotebookModel notebook, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UserId", notebook.UserId);
                p.Add("@Name", notebook.Name);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute(storedProcedure,
                        p, commandType: CommandType.StoredProcedure);

                int newId = p.Get<int>("@Id");
                return newId;
            }
        }

        public int CreateOrder(string storedProcedure, OrderModel order, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UserId", order.UserId);
                p.Add("@SubTotal", order.SubTotal);
                p.Add("@Tax", order.Tax);
                p.Add("@Total", order.Total);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute(storedProcedure,
                        p, commandType: CommandType.StoredProcedure);

                int newId = p.Get<int>("@id");
                return newId;
            }
        }

        public int CreateProductTransaction(string storedProcedure, ProductModel product)
        {
            var p = new DynamicParameters();
            p.Add("@ProductName", product.ProductName);
            p.Add("@Description", product.Description);
            p.Add("@ProductImage", product.ProductImage);
            p.Add("@RetailPrice", product.RetailPrice);
            p.Add("@QuantityInStock", product.QuantityInStock);
            p.Add("@IsTaxable", product.IsTaxable);
            p.Add("@Sex", product.Sex);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            _connection.Execute(storedProcedure,
                    p, commandType: CommandType.StoredProcedure, transaction: _transaction);

            int newId = p.Get<int>("@id");
            return newId;
        }


        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            _connection = new SqlConnection(connectionString);
            _connection.Open();

            _transaction = _connection.BeginTransaction();

            isClosed = false;
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure,
                    parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);

        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure,
                parameters, commandType: CommandType.StoredProcedure,
                transaction: _transaction).ToList();

            return rows;
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();

            isClosed = true;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();

            isClosed = true;
        }

        public void Dispose()
        {
            if (isClosed == false)
            {
                try
                {
                    CommitTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Commit transaction failed in the dispose method.");
                }
            }

            _transaction = null;
            _connection = null;
        }
    }
}
