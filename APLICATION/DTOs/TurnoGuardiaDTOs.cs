using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APLICATION.DTOs
{
    public class TurnoGuardiaDTOs
    {
        public int ID_TURNO {  get; set; }
        public int ID_VETERINARIO {  get; set; }
        public DateTime FECHA {  get; set; }
        public TimeOnly HORA_INICIO { get; set; }
        public TimeOnly HORA_FIN {  get; set; }
        public string? Estado {  get; set; }

    }
}
