using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface ITurnoGuardiaRepository
    {
        Task<IEnumerable<Citas>> ListarTurnoAsync();
        Task NuevoTurno(TurnoGuardia oTurno);
        Task EditarTurno(TurnoGuardia oTurno);
        Task EliminarTurno(int id);
    }
}
