using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class TurnoGuardioServices
    {
        private readonly ITurnoGuardiaRepository _repository;

        public TurnoGuardioServices(ITurnoGuardiaRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<TurnoGuardiaDTOs>> ListarTurnoGuardia()
        {
            var lista = await _repository.ListarTurnoAsync();
            return lista.Select(p => new TurnoGuardiaDTOs
            {
                ID_TURNO = p.ID_TURNO,
                ID_VETERINARIO = p.ID_VETERINARIO,
                FECHA = p.FECHA,
                HORA_INICIO = p.HORA_INICIO,
                HORA_FIN = p.HORA_FIN,
                Estado = p.Estado
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevoTurnoGuardia(TurnoGuardiaDTOs TurnoG)
        {
            var oTurno = new TurnoGuardia
            {
                ID_VETERINARIO = TurnoG.ID_VETERINARIO,
                FECHA = TurnoG.FECHA,
                HORA_INICIO = TurnoG.HORA_INICIO,
                HORA_FIN = TurnoG.HORA_FIN,
                Estado = TurnoG.Estado
            };
            await _repository.NuevoTurno(oTurno);
        }

        //Editar ---------------------------------------------------------------------
        public async Task EditarTurno(TurnoGuardiaDTOs TurnoG)
        {
            var oTurno = new TurnoGuardia
            {
                ID_VETERINARIO = TurnoG.ID_VETERINARIO,
                FECHA = TurnoG.FECHA,
                HORA_INICIO = TurnoG.HORA_INICIO,
                HORA_FIN = TurnoG.HORA_FIN,
                Estado = TurnoG.Estado
            };
            await _repository.EditarTurno(oTurno);
        }

        //Eliminar ---------------------------------------------------------------------
        public async Task ELiminarTurno(int id)
        {
            await _repository.EliminarTurno(id);
        }
    }
}
