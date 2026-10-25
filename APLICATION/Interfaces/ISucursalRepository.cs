using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface ISucursalRepository
    {
        Task<IEnumerable<Sucursal>> ListarSucursalAsync();
        Task NuevaSucursal(Sucursal oSucursal);
        Task EditarSucursal(Sucursal oSucursal);
        Task EliminarSucursal(int id);
    }
}
