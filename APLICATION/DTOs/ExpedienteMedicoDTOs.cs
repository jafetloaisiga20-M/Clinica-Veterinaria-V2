using System;
using System.Collections.Generic;
using System.Text;

namespace APLICATION.DTOs
{
    public class ExpedienteMedicoDTOs
    {
       public int ID_EXPEDIENTE { get; set; }
       public int ID_PACIENTE { get; set; }
       public int ID_CITAS { get; set; }
       public string? MOTIVO_VISITA { get; set; }
       public string? SINTOMAS { get; set; }
       public string? DIAGNOSTICO {  get; set; }
       public string? TRATAMIENTOS { get; set; }
       public string? RECETA_MEDICA { get; set; }
       public DateTime FECHA_REGISTRO { get; set; }
    }
}
