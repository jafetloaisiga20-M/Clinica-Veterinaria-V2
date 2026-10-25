using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface ICitasRepository
    {
        Task<IEnumerable<Citas>> ListarCitasAsync();
        Task NuevaCita(Citas oCitas);
        Task EditarCitas(Citas oCitas);
        Task EliminarCitas(int id);
    }
}
