using System;
using System.Collections.Generic;

namespace Turnos.Common.Models.DB
{
    public partial class Dias
    {
        public int DiaId { get; set; }
        public string DiaNombre { get; set; }
        public string DiaEstado { get; set; }
        public DateTime DiaFalta { get; set; }
    }
}
