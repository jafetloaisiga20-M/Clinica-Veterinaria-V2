using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IMensajeSocialRepository
    {
        Task<IEnumerable<MensajeSocial>> ListarAsync();
        Task NuevoAsync(MensajeSocial mensaje);
        Task MarcarAtendidoAsync(int id, int? idCita);
    }
}