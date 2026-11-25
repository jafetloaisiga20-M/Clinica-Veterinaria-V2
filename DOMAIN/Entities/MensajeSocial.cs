using System;

namespace DOMAIN.Entities
{
    public class MensajeSocial
    {
        public int ID_MENSAJE { get; set; }
        public string CANAL { get; set; } = "WhatsApp";
        public string REMITENTE { get; set; } = "";
        public string TEXTO { get; set; } = "";
        public DateTime FECHA_RECEPCION { get; set; } = DateTime.UtcNow;
        public string ESTADO { get; set; } = "Nuevo";
        public int? ID_CITA { get; set; }
    }
}