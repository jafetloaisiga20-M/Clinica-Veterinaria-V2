using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace APLICATION.DTOs
{
    public class PacienteDTOs
    {
       public int ID_PACIENTE { get; set; }
       [Range(1, int.MaxValue, ErrorMessage = "El cliente es obligatorio.")]
       public int ID_CLIENTE {  get; set; }
       [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
       public string? NOMBRE {  get; set; }
       public string? ESPECIE { get; set; }
       public string? RAZA { get; set; }
       public string? SEXO { get; set; }
       public DateTime FECHA_NACIMIENTO {  get; set; }
       public decimal PESO_ACTUAL { get; set; }
       public string? ESTADO { get; set; }

    }
}
