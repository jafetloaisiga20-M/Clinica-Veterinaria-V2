using System;
using System.Collections.Generic;
using System.Text;

namespace APLICATION.DTOs
{
    public class VeterinarioDTOs
    {
        public int ID_VETERINARIO {  get; set; }
        public int ID_SUCURSAL {  get; set; }
        public string? NOMBRE_COMPLETO {  get; set; }
        public string? ESPECIALIDAD { get; set; }
        public string? CV_DESCRIPCION { get; set; }
        public string? FOTO_URL {  get; set; }
        public string? TELEFONO { get; set; }
        public string? ESTADO { get; set; }
    }
}
