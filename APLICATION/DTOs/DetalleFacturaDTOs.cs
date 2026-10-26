using System;
using System.Collections.Generic;
using System.Text;

namespace APLICATION.DTOs
{
    public class DetalleFacturaDTOs
    {
        /// <summary>
        /// DTO (Data Transfer Object): Objeto plano utilizado para transferir datos entre la presentación (API/Frontend) y la aplicación. 
        /// Sirve para estructurar la información exacta que requiere o envía el cliente, ocultando la estructura real de las entidades de base de datos.
        /// </summary>
        public int ID_FACTURA { get; set; }
        public DateTime FECHA_EMISION { get; set; }
        public string? NOMBRE_COMPLETO { get; set; }
        public int ID_SERVICIO { get; set; }
        public int CANTIDAD { get; set; }
        public decimal PRECIO_UNITARIO { get; set; }
        public decimal SUBTOTAL_LINEA { get; set; }
        public string? ESTADO_FACTURA { get; set; }
    }
}
