using System;
using System.ComponentModel.DataAnnotations;

namespace Turnos.Common.Models.DB
{
    public partial class Turnos
    {
        [Display(Name = "Id Turno")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Turno")]
        public int TurId { get; set; }
        public string TurNombre { get; set; }
        public string TurEstado { get; set; }
        public DateTime TurFalta { get; set; }
    }
}
