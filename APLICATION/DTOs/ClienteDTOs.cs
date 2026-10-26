using System;
using System.Collections.Generic;
using System.Text;

namespace APLICATION.DTOs
{
    public class ClienteDTOs
    {
        /// <summary>
        /// DTO (Data Transfer Object): Objeto plano utilizado para transferir datos entre la presentación (API/Frontend) y la aplicación. 
        /// Sirve para estructurar la información exacta que requiere o envía el cliente, ocultando la estructura real de las entidades de base de datos.
        /// </summary>
        public int ID_CLIENTE {  get; set; }
        public string? NOMBRE_COMPLETO {  get; set; }
        public string? TELEFONO { get; set; }
        public string? WHATSAPP { get; set; }
        public string? EMAIL {  get; set; }
        public string? DIRECCION { get; set; }
        public string? ESTADO { get; set; }

    }
}
