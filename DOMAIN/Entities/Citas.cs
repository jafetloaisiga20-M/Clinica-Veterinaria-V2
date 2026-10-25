using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
    public class Citas
    {

        public int ID_CITAS { get; set; }
        public int ID_PACIENTE { get; set; }
        public int ID_SUCURSAL { get; set; }
        public int ID_VETERINARIO { get; set; }
        public DateTime FECHA_HORA { get; set; }
        public string? ORIGEN_CONTACTO { get; set; }
        public string? ESTADO_CITA { get; set; }
    }
}
