using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IVeterinarioRepository
    {
        Task<IEnumerable<Veterinario>> ListarVeterinariosAsync();
        Task NuevoVeterinario(Veterinario oVeterinario);
        Task EditarVeterinario(Veterinario oVeterinario);
        Task EliminarVeterinario(int id);
    }
}
