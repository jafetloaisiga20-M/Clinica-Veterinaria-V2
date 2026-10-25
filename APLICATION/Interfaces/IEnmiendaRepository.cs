using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IEnmiendaRepository
    {
        Task<IEnumerable<Enmienda>> ListarEnmiendaAsync();
        Task NuevoEnmiendaAsync(Enmienda oEnmienda);
        Task EditarEnmiendaAsync(Enmienda oEnmienda);
        Task EliminarEnmienda(int id);
    }
}
