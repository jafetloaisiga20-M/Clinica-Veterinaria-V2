using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class SucursalRepository : ISucursalRepository
    {
        private DBconnecctionFactory _dbConncetionFactory;

        public SucursalRepository(DBconnecctionFactory dbConncetionFactory)
        {
            _dbConncetionFactory = dbConncetionFactory;
        }

        public Task<IEnumerable<Sucursal>> ListarSucursalAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevaSucursal(Sucursal oSucursal)
        {
            throw new NotImplementedException();
        }

        public Task EditarSucursal(Sucursal oSucursal)
        {
            throw new NotImplementedException();
        }

        public Task EliminarSucursal(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
