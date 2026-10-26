using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class ClienteServices
    {
        /// <summary>
        /// Servicios (Casos de Uso): Orquestan la lógica de la aplicación. 
        /// Se encargan de inyectar las interfaces de los repositorios, ejecutar reglas de validación, procesar la información y mapear las Entidades del Dominio hacia los DTOs que se devolverán al controlador.
        /// </summary>

        private readonly IClienteRepository _repository;

        public ClienteServices(IClienteRepository repository)
        {
            _repository = repository;
        }

        //listar-------------------------------------------------------------
        public async Task<IEnumerable<ClienteDTOs>> ListarClietes()
        {
            var list = await _repository.ListarClientesAsync();
            return list.Select(p=> new ClienteDTOs {
                ID_CLIENTE = p.ID_CLIENTE,
                NOMBRE_COMPLETO = p.NOMBRE_COMPLETO,
                TELEFONO = p.TELEFONO,
                WHATSAPP = p.WHATSAPP,
                EMAIL = p.EMAIL,
                DIRECCION = p.DIRECCION,
                ESTADO = p.ESTADO
            });
        }

        //Nuevo------------------------------------------------------
        public async Task NuevoCliente(ClienteDTOs Clien)
        {
            var oCliente = new Cliente
            {
                NOMBRE_COMPLETO = Clien.NOMBRE_COMPLETO,
                TELEFONO = Clien.TELEFONO,
                WHATSAPP = Clien.WHATSAPP,
                EMAIL = Clien.EMAIL,
                DIRECCION = Clien.DIRECCION,
                ESTADO = Clien.ESTADO
            };
            await _repository.NuevoClienteAsync(oCliente);
        }
        //editar
        public async Task EditarCliente(ClienteDTOs Clien)
        {
            var oCliente = new Cliente
            {
                ID_CLIENTE = Clien.ID_CLIENTE,
                NOMBRE_COMPLETO = Clien.NOMBRE_COMPLETO,
                TELEFONO = Clien.TELEFONO,
                WHATSAPP = Clien.WHATSAPP,
                EMAIL = Clien.EMAIL,
                DIRECCION = Clien.DIRECCION,
                ESTADO = Clien.ESTADO
            };
            await _repository.EditarClienteAsync(oCliente);
        }

        //eliminar
        public async Task eliminarCliente(int id)
        {
            await _repository.EliminarClienteAsync(id);
        }
    }
}
