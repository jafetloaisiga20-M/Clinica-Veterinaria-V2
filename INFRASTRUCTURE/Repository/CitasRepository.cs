using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class CitasRepository : ICitasRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public CitasRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory = dbconnecttionFactory;
        }

        public Task<IEnumerable<Citas>> ListarCitasAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevaCita(Citas oCitas)
        {
            throw new NotImplementedException();
        }
        public Task EditarCitas(Citas oCitas)
        {
            throw new NotImplementedException();
        }

        public Task EliminarCitas(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
