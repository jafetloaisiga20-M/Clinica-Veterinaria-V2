using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
    public class Paciente
    {
        public int ID_PACIENTE { get; set; }
        public int ID_CLIENTE { get; set; }
        public string? NOMBRE { get; set; }
        public string? ESPECIE { get; set; }
        public string? RAZA { get; set; }
        public string? SEXO { get; set; }
        public DateTime FECHA_NACIMIENTO { get; set; }
        public decimal PESO_ACTUAL { get; set; }
        public string? ESTADO { get; set; }
    }
}
