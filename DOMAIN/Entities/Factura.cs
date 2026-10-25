using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
    public class Factura
    {
        public int ID_FACTURA { get; set; }
        public int ID_CITA { get; set; }
        public int ID_CLIENTE { get; set; }
        public string? NUMERO_FACTURA { get; set; }
        public Boolean TICKET_IMPRESO { get; set; }
        public DateTime FECHA_EMISION { get; set; }
        public decimal SUBTOTAL { get; set; }
        public decimal IMPUESTOS { get; set; }
        public decimal TOTAL_PAGAR { get; set; }
        public string? METODO_PAGO { get; set; }
        public string? ESTADO_FACTURA { get; set; }
    }
}
