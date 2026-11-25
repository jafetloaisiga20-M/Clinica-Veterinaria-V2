using System.ComponentModel.DataAnnotations;

namespace APLICATION.DTOs
{
    public class LoginDTOs
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "La contrasena es obligatoria.")]
        public string? Password { get; set; }
    }
}