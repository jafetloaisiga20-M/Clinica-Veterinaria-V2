using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
       [Range(1, int.MaxValue, ErrorMessage = "El paciente es obligatorio.")]
       public int ID_PACIENTE { get; set; }
       [Range(1, int.MaxValue, ErrorMessage = "La sucursal es obligatoria.")]
       public int ID_SUCURSAL { get; set; }
       [Range(1, int.MaxValue, ErrorMessage = "El veterinario es obligatorio.")]
       public int ID_VETERINARIO { get; set; }
       [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
       public DateTime FECHA_HORA { get; set; }
       [Required(ErrorMessage = "El canal de origen es obligatorio.")]
       public string? ORIGEN_CONTACTO { get; set; }
       public string? ESTADO_CITA { get; set; }

    }
}
