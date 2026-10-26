using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class CitasServices
    {
        /// <summary>
        /// Servicios (Casos de Uso): Orquestan la lógica de la aplicación. 
        /// Se encargan de inyectar las interfaces de los repositorios, ejecutar reglas de validación, procesar la información y mapear las Entidades del Dominio hacia los DTOs que se devolverán al controlador.
        /// </summary>

        private readonly ICitasRepository _repository;

        public CitasServices(ICitasRepository repository) 
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<CitasDTOs>> ListarCitas()
        {
            var lista = await _repository.ListarCitasAsync();
            return lista.Select(p => new CitasDTOs
            {
               ID_CITAS = p.ID_CITAS,
               ID_PACIENTE = p.ID_PACIENTE,
               ID_SUCURSAL = p.ID_SUCURSAL,
               ID_VETERINARIO = p.ID_VETERINARIO,
               FECHA_HORA = p.FECHA_HORA,
               ORIGEN_CONTACTO = p.ORIGEN_CONTACTO,
               ESTADO_CITA = p.ESTADO_CITA
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task Nuevacita(CitasDTOs Cit)
        {
            var oCitas = new Citas
            {
                ID_PACIENTE = Cit.ID_PACIENTE,
                ID_SUCURSAL = Cit.ID_SUCURSAL,
                ID_VETERINARIO = Cit.ID_VETERINARIO,
                FECHA_HORA = Cit.FECHA_HORA,
                ORIGEN_CONTACTO = Cit.ORIGEN_CONTACTO,
                ESTADO_CITA = Cit.ESTADO_CITA
            };
            await _repository.NuevaCita(oCitas);
        }

        //Editar ---------------------------------------------------------------------
        public async Task Editarcita(CitasDTOs Cit)
        {
            var oCitas = new Citas
            {
                ID_CITAS = Cit.ID_CITAS,
                ID_PACIENTE = Cit.ID_PACIENTE,
                ID_SUCURSAL = Cit.ID_SUCURSAL,
                ID_VETERINARIO = Cit.ID_VETERINARIO,
                FECHA_HORA = Cit.FECHA_HORA,
                ORIGEN_CONTACTO = Cit.ORIGEN_CONTACTO,
                ESTADO_CITA = Cit.ESTADO_CITA
            };
            await _repository.EditarCitas(oCitas);
        }

        //Eliminar ---------------------------------------------------------------------
        public async Task EliminarCita(int id)
        {
            await _repository.EliminarCitas(id);
        }
    }
}
