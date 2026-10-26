using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class ExpedienteMedicoRepository : IExpedienteMedicoRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public ExpedienteMedicoRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }


        public Task<IEnumerable<ExpedienteMedico>> ListarExpedienteAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevoExpediente(ExpedienteMedico oExpediente)
        {
            throw new NotImplementedException();
        }

        public Task EditarExpediente(ExpedienteMedico oExpediente)
        {
            throw new NotImplementedException();
        }

        public Task EliminarExpediente(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}
