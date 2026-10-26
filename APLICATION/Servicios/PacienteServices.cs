using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class PacienteServices
    {


        /// <summary>
        /// Servicios (Casos de Uso): Orquestan la lógica de la aplicación. 
        /// Se encargan de inyectar las interfaces de los repositorios, ejecutar reglas de validación, procesar la información y mapear las Entidades del Dominio hacia los DTOs que se devolverán al controlador.
        /// </summary>

        private readonly IPacienteRepository _repository;

        public PacienteServices(IPacienteRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<PacienteDTOs>> ListarPaciente()
        {
            var lista = await _repository.ListarPacienteAsync();
            return lista.Select(p => new PacienteDTOs
            {
              ID_PACIENTE = p.ID_PACIENTE,
              ID_CLIENTE = p.ID_CLIENTE,
              NOMBRE = p.NOMBRE,
              ESPECIE = p.ESPECIE,
              RAZA = p.RAZA,
              SEXO = p.SEXO,
              FECHA_NACIMIENTO = p.FECHA_NACIMIENTO,
              PESO_ACTUAL = p.PESO_ACTUAL,
              ESTADO = p.ESTADO
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevoPaciente(PacienteDTOs Pac)
        {
            var oPaciente = new Paciente
            {
                ID_CLIENTE = Pac.ID_CLIENTE,
                NOMBRE = Pac.NOMBRE,
                ESPECIE = Pac.ESPECIE,
                RAZA = Pac.RAZA,
                SEXO = Pac.SEXO,
                FECHA_NACIMIENTO = Pac.FECHA_NACIMIENTO,
                PESO_ACTUAL = Pac.PESO_ACTUAL,
                ESTADO = Pac.ESTADO
            };
            await _repository.NuevoPaciente(oPaciente);
        }

        //Editar ---------------------------------------------------------------------
        public async Task EditarPaciente(PacienteDTOs Pac)
        {
            var oPaciente = new Paciente
            {
                ID_PACIENTE = Pac.ID_PACIENTE,
                ID_CLIENTE = Pac.ID_CLIENTE,
                NOMBRE = Pac.NOMBRE,
                ESPECIE = Pac.ESPECIE,
                RAZA = Pac.RAZA,
                SEXO = Pac.SEXO,
                FECHA_NACIMIENTO = Pac.FECHA_NACIMIENTO,
                PESO_ACTUAL = Pac.PESO_ACTUAL,
                ESTADO = Pac.ESTADO
            };
            await _repository.EditarPaciente(oPaciente);
        }

        //Eliminar ---------------------------------------------------------------------
        public async Task ELiminarPaciente(int id)
        {
            await _repository.EliminarPaciente(id);
        }

    }
}
