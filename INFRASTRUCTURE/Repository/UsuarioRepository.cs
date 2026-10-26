using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using APLICATION.Servicios;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {

        private DBconnecctionFactory _dbConnectionFactory;

        public UsuarioRepository(DBconnecctionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public Task<IEnumerable<Usuario>> ListarUsuarioAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevoUsuario(Usuario oUsuario)
        {
            throw new NotImplementedException();
        }

        public Task EditarUsuario(Usuario oUsuario)
        {
            throw new NotImplementedException();
        }

        public Task EliminarUsuario(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}
