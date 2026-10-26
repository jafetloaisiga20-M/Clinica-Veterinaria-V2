using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class RolesServices
    {
        private readonly IRolesRepository _repository;

        public RolesServices(IRolesRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<RolesDTOs>> ListarRoles()
        {
            var lista = await _repository.ListarRolesAsync();
            return lista.Select(p => new RolesDTOs
            {
               ID_ROL =p.ID_ROL,
               NOMBRE_ROL = p.NOMBRE_ROL
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevoRol(RolesDTOs Roles)
        {
            var oRoles = new Roles
            {
               NOMBRE_ROL = Roles.NOMBRE_ROL
            };
            await _repository.NuevoRolAsync(oRoles);
        }
    }
}
