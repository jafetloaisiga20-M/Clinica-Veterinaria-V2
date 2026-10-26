using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class ExpedienteMedicoServices
    {

        /// <summary>
        /// Servicios (Casos de Uso): Orquestan la lógica de la aplicación. 
        /// Se encargan de inyectar las interfaces de los repositorios, ejecutar reglas de validación, procesar la información y mapear las Entidades del Dominio hacia los DTOs que se devolverán al controlador.
        /// </summary>


        private readonly IExpedienteMedicoRepository _repository;

        public ExpedienteMedicoServices(IExpedienteMedicoRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<ExpedienteMedicoDTOs>> ListarExpedienteMEdico()
        {
            var lista = await _repository.ListarExpedienteAsync();
            return lista.Select(p => new ExpedienteMedicoDTOs
            {
                ID_EXPEDIENTE = p.ID_EXPEDIENTE,
                ID_CITAS = p.ID_CITAS,
                ID_PACIENTE = p.ID_PACIENTE,
                MOTIVO_VISITA = p.MOTIVO_VISITA,
                SINTOMAS= p.SINTOMAS,
                DIAGNOSTICO = p.DIAGNOSTICO,
                TRATAMIENTOS = p.TRATAMIENTOS,
                RECETA_MEDICA = p.RECETA_MEDICA,
                FECHA_REGISTRO = p.FECHA_REGISTRO,
              
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevoExpediente(ExpedienteMedicoDTOs ExM)
        {
            var oExpediente = new ExpedienteMedico
            {
                ID_CITAS = ExM.ID_CITAS,
                ID_PACIENTE = ExM.ID_PACIENTE,
                MOTIVO_VISITA = ExM.MOTIVO_VISITA,
                SINTOMAS = ExM.SINTOMAS,
                DIAGNOSTICO = ExM.DIAGNOSTICO,
                TRATAMIENTOS = ExM.TRATAMIENTOS,
                RECETA_MEDICA = ExM.RECETA_MEDICA,
                FECHA_REGISTRO = ExM.FECHA_REGISTRO,
            };
            await _repository.NuevoExpediente(oExpediente);
        }

        //Editar ---------------------------------------------------------------------
        public async Task EditarExpediente(ExpedienteMedicoDTOs ExM)
        {
            var oExpediente = new ExpedienteMedico
            {
                ID_EXPEDIENTE = ExM.ID_EXPEDIENTE,
                ID_CITAS = ExM.ID_CITAS,
                ID_PACIENTE = ExM.ID_PACIENTE,
                MOTIVO_VISITA = ExM.MOTIVO_VISITA,
                SINTOMAS = ExM.SINTOMAS,
                DIAGNOSTICO = ExM.DIAGNOSTICO,
                TRATAMIENTOS = ExM.TRATAMIENTOS,
                RECETA_MEDICA = ExM.RECETA_MEDICA,
                FECHA_REGISTRO = ExM.FECHA_REGISTRO,
            };
            await _repository.EditarExpediente(oExpediente);
        }

        //Eliminar ---------------------------------------------------------------------
        public async Task EliminarExpediente(int id)
        {
            await _repository.EliminarExpediente(id);
        }
    }
}
