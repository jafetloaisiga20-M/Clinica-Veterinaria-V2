using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IServicioRepository
    {
        Task<IEnumerable<Servicio>> ListarServicioAsync();
        Task NuevoServicio(Servicio oServicio);
        Task EditarServicio(Servicio oServicio);
        Task EliminaServicio(int id);
    }
}
