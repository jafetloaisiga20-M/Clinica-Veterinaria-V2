using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace INFRASTRUCTURE.DataBase
{
    public class DBconnecctionFactory
    {
        /// <summary>
        /// Factory de Base de Datos: Centraliza la creación y configuración de las conexiones a la base de datos (ADO.NET). 
        /// Gestiona la cadena de conexión de forma centralizada para que los repositorios solo se preocupen por ejecutar consultas.
        /// </summary>  


        private readonly string _connectionString;

        public DBconnecctionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
