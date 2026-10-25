using System;
using System.Collections.Generic;
using System.Text;

namespace DOMAIN.Entities
{
    public class Usuario
    {
        public int ID_USUARIO { get; set; }
        public int ID_ROL { get; set; }
        public int ID_SUCURSAL { get; set; }
        public string? NOMBRE_USUARIO { get; set; }
        public string? PASSWORD_HASH { get; set; }
        public string? ESTADO { get; set; }
    }
}
