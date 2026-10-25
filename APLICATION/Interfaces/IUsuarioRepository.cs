using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ListarUsuarioAsync();
        Task NuevoUsuario(Usuario oUsuario);
        Task EditarUsuario(Usuario oUsuario);
        Task EliminarUsuario(int id);
    }
}
