using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ListarClientesAsync();
        Task NuevoClienteAsync(Cliente oCliente);
        Task EditarClienteAsync(Cliente oCliente);
        Task EliminarClienteAsync(int id);
    }
}
