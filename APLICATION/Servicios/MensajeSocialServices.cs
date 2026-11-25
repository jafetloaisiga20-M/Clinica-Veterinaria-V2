using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class MensajeSocialServices
    {
        private readonly IMensajeSocialRepository _repo;

        public MensajeSocialServices(IMensajeSocialRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<MensajeSocialDTOs>> Listar()
        {
            var lista = await _repo.ListarAsync();
            return lista.Select(m => new MensajeSocialDTOs
            {
                ID_MENSAJE = m.ID_MENSAJE,
                CANAL = m.CANAL,
                REMITENTE = m.REMITENTE,
                TEXTO = m.TEXTO,
                FECHA_RECEPCION = m.FECHA_RECEPCION,
                ESTADO = m.ESTADO,
                ID_CITA = m.ID_CITA
            });
        }

        public async Task Nuevo(MensajeSocialDTOs dto)
        {
            var m = new MensajeSocial
            {
                CANAL = dto.CANAL,
                REMITENTE = dto.REMITENTE,
                TEXTO = dto.TEXTO,
                FECHA_RECEPCION = dto.FECHA_RECEPCION == default ? DateTime.UtcNow : dto.FECHA_RECEPCION,
                ESTADO = string.IsNullOrWhiteSpace(dto.ESTADO) ? "Nuevo" : dto.ESTADO,
                ID_CITA = dto.ID_CITA
            };
            await _repo.NuevoAsync(m);
        }

        public async Task MarcarAtendido(int id, int? idCita)
        {
            await _repo.MarcarAtendidoAsync(id, idCita);
        }
    }
}