using System;
using System.Collections.Generic;
using System.Text;

namespace APLICATION.DTOs
{
    public class CitasDTOs
    {
        /// <summary>
        /// DTO (Data Transfer Object): Objeto plano utilizado para transferir datos entre la presentación (API/Frontend) y la aplicación. 
        /// Sirve para estructurar la información exacta que requiere o envía el cliente, ocultando la estructura real de las entidades de base de datos.
        /// </summary>
        public int ID_CITAS {  get; set; }
       public int ID_PACIENTE { get; set; }
       public int ID_SUCURSAL { get; set; }
       public int ID_VETERINARIO { get; set; }
       public DateTime FECHA_HORA { get; set; }
       public string? ORIGEN_CONTACTO { get; set; }
       public string? ESTADO_CITA { get; set; }

    }
}
