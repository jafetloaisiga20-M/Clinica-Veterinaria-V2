using System;
using System.Collections.Generic;
using System.Text;

namespace APLICATION.DTOs
{
    public  class ServicioDTOs
    {
       public int ID_SERVICIO { get; set; }
       public string? NOMBRE_SERVICIO { get; set; }
       public string?  CATEGORIA { get; set; }
       public string? DESCRIPCION_DE_SERVICIO { get; set; }
       public decimal PRECIO { get; set; }
       public string? ESTADO { get; set; }

    }
}
