using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
    public class TurnoGuardia
    {
        public int ID_TURNO { get; set; }
        public int ID_VETERINARIO { get; set; }
        public DateTime FECHA { get; set; }
        public TimeOnly HORA_INICIO { get; set; }
        public TimeOnly HORA_FIN { get; set; }
        public string? Estado { get; set; }
    }
}
