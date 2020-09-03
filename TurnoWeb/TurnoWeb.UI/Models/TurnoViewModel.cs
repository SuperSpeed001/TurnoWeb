namespace TurnoWeb.UI.Models
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class TurnoViewModel 
    {
        public TurnoViewModel() { }
        [Display(Name = "Seleccione Servicio...")]

        public int TurId { get; set; }

        public List<SelectListItem> ListadoTurnos { get; set; }

        public int Dia { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }

        public int Hora { get; set; }
        public int Minutos { get; set; }        

        public int PerId { get; set; }
        [Display(Name = "Nombre")]
        public string PerNombre { get; set; }

        [Display(Name = "D.N.I.")]
        [Required]
        [StringLength(8, MinimumLength = 7, ErrorMessage ="Debe ingresar D.N.I. valido")]
        public string PerDni { get; set; }

        [EmailAddress(ErrorMessage = "Email invalido")]
        [Display(Name = "Email")]
        public string PerEmail { get; set; }

        [Display(Name = "Teléfono")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Debes ingresar números")]
        public string PerTelef { get; set; }

        [Display(Name = "Seleccione")]
        public string PerEstado { get; set; }

        [Display(Name = "Fecha")]
        [Required]
        public DateTime PerFalta { get; set; }

        public List<TablaTurnosViewModel> TablaTurnosViewModels { get; set; }
    }
}
