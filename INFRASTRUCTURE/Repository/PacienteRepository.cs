using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class PacienteRepository:IPacienteRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>


        private DBconnecctionFactory _dBconnecttionFactory;

        public PacienteRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }

        public Task<IEnumerable<Paciente>> ListarPacienteAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevoPaciente(Paciente oPaciente)
        {
            throw new NotImplementedException();
        }


        public Task EditarPaciente(Paciente oPaciente)
        {
            throw new NotImplementedException();
        }

        public Task EliminarPaciente(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
