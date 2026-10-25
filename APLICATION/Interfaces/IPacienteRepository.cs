using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IPacienteRepository
    {
        Task<IEnumerable<Paciente>> ListarPacienteAsync();
        Task NuevoPaciente(Paciente oPaciente);
        Task EditarPaciente(Paciente oPaciente);
        Task EliminarPaciente(int id);
    }
}
