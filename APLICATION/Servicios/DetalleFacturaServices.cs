using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class DetalleFacturaServices
    {
        /// <summary>
        /// Servicios (Casos de Uso): Orquestan la lógica de la aplicación. 
        /// Se encargan de inyectar las interfaces de los repositorios, ejecutar reglas de validación, procesar la información y mapear las Entidades del Dominio hacia los DTOs que se devolverán al controlador.
        /// </summary>

        private readonly IDetalleFacturaRepository _repository;

        public DetalleFacturaServices(IDetalleFacturaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DetalleFacturaDTOs>> ListarDetalleFactura()
        {
            var lista = await _repository.ListarDetalleFacturaAsync();
            return lista.Select(p => new DetalleFacturaDTOs
            {
                ID_FACTURA = p.ID_FACTURA,
                FECHA_EMISION = p.FECHA_EMISION,
                NOMBRE_COMPLETO = p.NOMBRE_COMPLETO,
                ID_SERVICIO = p.ID_SERVICIO,
                CANTIDAD = p.CANTIDAD,
                PRECIO_UNITARIO = p.PRECIO_UNITARIO,
                SUBTOTAL_LINEA = p.SUBTOTAL_LINEA,
                ESTADO_FACTURA = p.ESTADO_FACTURA
            });
        }

        public async Task NuevoDetalleFactura(DetalleFacturaDTOs DTF)
        {
            var oDetalleF = new DetalleFactura
            {
                FECHA_EMISION = DTF.FECHA_EMISION,
                NOMBRE_COMPLETO = DTF.NOMBRE_COMPLETO,
                ID_SERVICIO = DTF.ID_SERVICIO,
                CANTIDAD = DTF.CANTIDAD,
                PRECIO_UNITARIO = DTF.PRECIO_UNITARIO,
                SUBTOTAL_LINEA = DTF.SUBTOTAL_LINEA,
                ESTADO_FACTURA = DTF.ESTADO_FACTURA
            };
        }
    }
}
