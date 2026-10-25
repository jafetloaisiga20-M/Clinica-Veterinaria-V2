using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IExpedienteMedicoRepository
    {
        Task<IEnumerable<ExpedienteMedico>> ListarExpedienteAsync();
        Task NuevoExpediente(ExpedienteMedico oExpediente);
        Task EditarExpediente(ExpedienteMedico oExpediente);
        Task EliminarExpediente(int id);
    }
}
