using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;

namespace INFRASTRUCTURE.Repository
{
    public class RolesRepository : IRolesRepository
    {

        private DBconnecctionFactory _dBconnecttionFactory;

        public RolesRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }
        public Task<IEnumerable<Roles>> ListarRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task NuevoRolAsync(Roles oRoles)
        {
            throw new NotImplementedException();
        }
    }
}
