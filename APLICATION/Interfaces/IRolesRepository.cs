using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IRolesRepository
    {
        Task<IEnumerable<Roles>> ListarRolesAsync();
        Task NuevoRolAsync(Roles oRoles);
    }
}
