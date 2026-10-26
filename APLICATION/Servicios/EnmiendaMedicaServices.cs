using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class EnmiendaMedicaServices
    {
        /// <summary>
        /// Servicios (Casos de Uso): Orquestan la lógica de la aplicación. 
        /// Se encargan de inyectar las interfaces de los repositorios, ejecutar reglas de validación, procesar la información y mapear las Entidades del Dominio hacia los DTOs que se devolverán al controlador.
        /// </summary>

        private readonly IEnmiendaRepository _repository;

        public EnmiendaMedicaServices(IEnmiendaRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<EnmiendaDTOs>> ListarEnmienda()
        {
            var lista = await _repository.ListarEnmiendaAsync();
            return lista.Select(p => new EnmiendaDTOs
            {
               ID_ENMIENDA = p.ID_ENMIENDA,
               ID_EXPEDIENTE = p.ID_EXPEDIENTE,
               ID_VETERINARIO = p.ID_VETERINARIO,
               NOTA_ADICIONAL = p.NOTA_ADICIONAL,
               FECHA_ENMIENDA = p.FECHA_ENMIENDA
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevaEnmienda(EnmiendaDTOs EnmiendaM)
        {
            var oEnmienda = new Enmienda
            {
                ID_EXPEDIENTE = EnmiendaM.ID_EXPEDIENTE,
                ID_VETERINARIO = EnmiendaM.ID_VETERINARIO,
                NOTA_ADICIONAL = EnmiendaM.NOTA_ADICIONAL                
            };
            await _repository.NuevoEnmiendaAsync(oEnmienda);
        }

       
    }
}
