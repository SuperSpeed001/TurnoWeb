using System;
using System.Collections.Generic;

namespace Turnos.Common.Models.DB
{
    public partial class Personas
    {
        public int PerId { get; set; }
        public string PerNombre { get; set; }
        public string PerDni { get; set; }
        public string PerEmail { get; set; }
        public string PerTelef { get; set; }
        public string PerEstado { get; set; }
        public DateTime PerFalta { get; set; }
    }
}
