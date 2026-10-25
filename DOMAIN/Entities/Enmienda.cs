using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
    public class Enmienda
    {
        public int ID_ENMIENDA { get; set; }
        public int ID_EXPEDIENTE { get; set; }
        public int ID_VETERINARIO { get; set; }
        public string? NOTA_ADICIONAL { get; set; }
        public DateTime FECHA_ENMIENDA { get; set; }
    }
}
