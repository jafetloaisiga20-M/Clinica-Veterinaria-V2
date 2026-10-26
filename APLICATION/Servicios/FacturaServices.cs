using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class FacturaServices
    {
        /// <summary>
        /// Servicios (Casos de Uso): Orquestan la lógica de la aplicación. 
        /// Se encargan de inyectar las interfaces de los repositorios, ejecutar reglas de validación, procesar la información y mapear las Entidades del Dominio hacia los DTOs que se devolverán al controlador.
        /// </summary>


        private readonly IFacturaRepository _repository;

        public FacturaServices(IFacturaRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<FacturaDTOs>> ListarFactura()
        {
            var lista = await _repository.ListarFacturaAsync();
            return lista.Select(p => new FacturaDTOs
            {
                ID_FACTURA = p.ID_FACTURA,
                ID_CITA = p.ID_CITA,
                ID_CLIENTE = p.ID_CLIENTE,
                NUMERO_FACTURA = p.NUMERO_FACTURA,
                TICKET_IMPRESO = p.TICKET_IMPRESO,
                FECHA_EMISION = p.FECHA_EMISION,
                SUBTOTAL = p.SUBTOTAL,
                IMPUESTOS = p.IMPUESTOS,
                TOTAL_PAGAR  = p.TOTAL_PAGAR,
                METODO_PAGO = p.METODO_PAGO,
                ESTADO_FACTURA = p.ESTADO_FACTURA
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevaFactura(FacturaDTOs Fac)
        {
            var oFactura = new Factura
            {
                ID_CITA = Fac.ID_CITA,
                ID_CLIENTE = Fac.ID_CLIENTE,
                NUMERO_FACTURA = Fac.NUMERO_FACTURA,
                TICKET_IMPRESO = Fac.TICKET_IMPRESO,
                FECHA_EMISION = Fac.FECHA_EMISION,
                SUBTOTAL = Fac.SUBTOTAL,
                IMPUESTOS = Fac.IMPUESTOS,
                TOTAL_PAGAR = Fac.TOTAL_PAGAR,
                METODO_PAGO = Fac.METODO_PAGO,
                ESTADO_FACTURA = Fac.ESTADO_FACTURA
            };
            await _repository.NuevaFactura(oFactura);
        }

        //Editar ---------------------------------------------------------------------
        public async Task EditarFactura(FacturaDTOs Fac)
        {
            var oFactura = new Factura
            {
                ID_FACTURA = Fac.ID_FACTURA,
                ID_CITA = Fac.ID_CITA,
                ID_CLIENTE = Fac.ID_CLIENTE,
                NUMERO_FACTURA = Fac.NUMERO_FACTURA,
                TICKET_IMPRESO = Fac.TICKET_IMPRESO,
                FECHA_EMISION = Fac.FECHA_EMISION,
                SUBTOTAL = Fac.SUBTOTAL,
                IMPUESTOS = Fac.IMPUESTOS,
                TOTAL_PAGAR = Fac.TOTAL_PAGAR,
                METODO_PAGO = Fac.METODO_PAGO,
                ESTADO_FACTURA = Fac.ESTADO_FACTURA
            };
            await _repository.EditarFactura(oFactura);
        }

        //Eliminar ---------------------------------------------------------------------
        public async Task CambiarEstadoFactura(int id)
        {
            await _repository.EliminarFactura(id);
        }

    }
}
