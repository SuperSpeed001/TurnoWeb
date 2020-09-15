namespace TurnoWeb.UI.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ListadoTurnoReservadosViewModel
    {
        //T.tur_nombre, H.hor_fecha, H.hor_dia, H.hor_hora, P.per_dni, P.per_nombre, H.hor_obser, B.box_nombre, H.hor_reserv from Horarios As H
        
        //public string NombreTurno { get; set; }
        public DateTime Fecha { get; set; }
        public string Dia { get; set; }
        public string Hora { get; set; }
        public string Dni { get; set; }
        [Display(Name = "Nombre")]
        public string NombrePersona { get; set; }
        public string Observacion { get; set; }
        [Display(Name = "Box")]
        public string NombreBox { get; set; }
       // public bool Estado { get; set; }
        public int IdHora { get; set; }
        [Display(Name = "Email")]
        public string EmailPersona { get; set; }
        [Display(Name = "Teléfono")]
        public string TelefonoPersona { get; set; }
    }
}
