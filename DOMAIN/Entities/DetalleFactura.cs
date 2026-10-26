using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
    public class DetalleFactura
    {
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
