using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class EnmiendaMedicaRepository : IEnmiendaRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public EnmiendaMedicaRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }

        public Task<IEnumerable<Enmienda>> ListarEnmiendaAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevoEnmiendaAsync(Enmienda oEnmienda)
        {
            throw new NotImplementedException();
        }

        public Task EditarEnmiendaAsync(Enmienda oEnmienda)
        {
            throw new NotImplementedException();
        }

        public Task EliminarEnmienda(int id)
        {
            throw new NotImplementedException();
        }

     
    }
}
