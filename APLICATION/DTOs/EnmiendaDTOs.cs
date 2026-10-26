using System;
using System.Collections.Generic;
using System.Text;

namespace APLICATION.DTOs
{
    public class EnmiendaDTOs
    {
        /// <summary>
        /// DTO (Data Transfer Object): Objeto plano utilizado para transferir datos entre la presentación (API/Frontend) y la aplicación. 
        /// Sirve para estructurar la información exacta que requiere o envía el cliente, ocultando la estructura real de las entidades de base de datos.
        /// </summary>
        public int ID_ENMIENDA { get; set; }
        public int ID_EXPEDIENTE { get; set; }
        public int ID_VETERINARIO { get; set; }
        public string? NOTA_ADICIONAL { get; set; }
        public DateTime FECHA_ENMIENDA { get; set; }


    }

}
