using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class EnmiendaMedicaServices
    {
        private readonly IEnmiendaRepository _repo;

        public EnmiendaMedicaServices(IEnmiendaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<EnmiendaDTOs>> Listar()
        {
            var lista = await _repo.ListarEnmiendaAsync();
            return lista.Select(e => new EnmiendaDTOs
            {
                ID_ENMIENDA = e.ID_ENMIENDA,
                ID_EXPEDIENTE = e.ID_EXPEDIENTE,
                ID_VETERINARIO = e.ID_VETERINARIO,
                NOTA_ADICIONAL = e.NOTA_ADICIONAL,
                FECHA_ENMIENDA = e.FECHA_ENMIENDA
            });
        }

        public async Task Nueva(EnmiendaDTOs dto)
        {
            var ent = new Enmienda
            {
                ID_EXPEDIENTE = dto.ID_EXPEDIENTE,
                ID_VETERINARIO = dto.ID_VETERINARIO,
                NOTA_ADICIONAL = dto.NOTA_ADICIONAL,
                FECHA_ENMIENDA = DateTime.UtcNow
            };
            await _repo.NuevoEnmiendaAsync(ent);
        }
    }
}