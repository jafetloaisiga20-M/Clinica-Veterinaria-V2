using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class ExpedienteMedicoServices
    {
        private readonly IExpedienteMedicoRepository _repo;

        public ExpedienteMedicoServices(IExpedienteMedicoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ExpedienteMedicoDTOs>> Listar()
        {
            var lista = await _repo.ListarExpedienteAsync();
            return lista.Select(e => new ExpedienteMedicoDTOs
            {
                ID_EXPEDIENTE = e.ID_EXPEDIENTE,
                ID_PACIENTE = e.ID_PACIENTE,
                ID_CITAS = e.ID_CITAS,
                MOTIVO_VISITA = e.MOTIVO_VISITA,
                SINTOMAS = e.SINTOMAS,
                DIAGNOSTICO = e.DIAGNOSTICO,
                TRATAMIENTOS = e.TRATAMIENTOS,
                RECETA_MEDICA = e.RECETA_MEDICA,
                FECHA_REGISTRO = e.FECHA_REGISTRO
            });
        }

        public async Task Nuevo(ExpedienteMedicoDTOs dto)
        {
            var ent = new ExpedienteMedico
            {
                ID_PACIENTE = dto.ID_PACIENTE,
                ID_CITAS = dto.ID_CITAS,
                MOTIVO_VISITA = dto.MOTIVO_VISITA,
                SINTOMAS = dto.SINTOMAS,
                DIAGNOSTICO = dto.DIAGNOSTICO,
                TRATAMIENTOS = dto.TRATAMIENTOS,
                RECETA_MEDICA = dto.RECETA_MEDICA,
                FECHA_REGISTRO = DateTime.UtcNow
            };
            await _repo.NuevoExpediente(ent);
        }
    }
}