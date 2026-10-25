using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace INFRASTRUCTURE.DataBase
{
    public class DBconnecttionFactory
    {
        private readonly string _connectionString;

        public DBconnecttionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
