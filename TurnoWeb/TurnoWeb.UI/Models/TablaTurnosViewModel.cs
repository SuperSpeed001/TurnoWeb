namespace TurnoWeb.UI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TablaTurnosViewModel
    {
        [Required]
        public int hor_id { get; set; }
        [Display(Name = "Hora")]
        public string hor_hora { get; set; }
        [Display(Name = "Día")]
        public string hor_dia { get; set; }
        [Display(Name = "Fecha")]
        public string hor_fecha { get; set; }
        [Display(Name = "Reservar")]
        public bool hor_reserv { get; set; }
        [Display(Name = "Nombre")]
        public string tur_nombre { get; set; }
        [Display(Name = "Box")]
        public string box_nombre { get; set; }
        public int HoraSeleccionada { get; set; }
    }
}
