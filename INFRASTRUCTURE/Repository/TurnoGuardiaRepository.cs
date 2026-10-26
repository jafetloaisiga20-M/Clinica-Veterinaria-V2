using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class TurnoGuardiaRepository : ITurnoGuardiaRepository
    {

        private DBconnecctionFactory _dBconnecttionFactory;
        public TurnoGuardiaRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory = dbconnecttionFactory;
        }

        public Task<IEnumerable<TurnoGuardia>> ListarTurnoAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevoTurno(TurnoGuardia oTurno)
        {
            throw new NotImplementedException();
        }

        public Task EditarTurno(TurnoGuardia oTurno)
        {
            throw new NotImplementedException();
        }

        public Task EliminarTurno(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}
