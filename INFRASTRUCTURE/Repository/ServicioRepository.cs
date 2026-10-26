using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class ServicioRepository : IServicioRepository
    {

        private DBconnecctionFactory _dBconnecttionFactory;

        public ServicioRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory= dbconnecttionFactory;
        }

        public Task<IEnumerable<Servicio>> ListarServicioAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevoServicio(Servicio oServicio)
        {
            throw new NotImplementedException();
        }

        public Task EditarServicio(Servicio oServicio)
        {
            throw new NotImplementedException();
        }

        public Task EliminaServicio(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
