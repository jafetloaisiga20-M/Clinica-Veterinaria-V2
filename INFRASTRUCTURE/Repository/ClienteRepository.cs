using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public ClienteRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory = dbconnecttionFactory;
        }

        public Task EditarClienteAsync(Cliente oCliente)
        {
            throw new NotImplementedException();
        }

        public Task EliminarClienteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Cliente>> ListarClientesAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevoClienteAsync(Cliente oCliente)
        {
            throw new NotImplementedException();
        }
    }
}
