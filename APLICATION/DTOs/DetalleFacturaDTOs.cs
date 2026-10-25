using System;
using System.Collections.Generic;
using System.Text;

namespace APLICATION.DTOs
{
    public class DetalleFacturaDTOs
    {
       public int ID_DETALLE { get; set; }
       public int ID_FACTURA { get; set; }
       public int ID_SERVICIO { get; set; }
       public int CANTIDAD { get; set; }
       public decimal PRECIO_UNITARIO { get; set; }
      public decimal SUBTOTAL_LINEA {  get; set; }
    }
}
