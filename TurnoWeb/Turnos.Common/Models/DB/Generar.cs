using System;
using System.Collections.Generic;

namespace Turnos.Common.Models.DB
{
    public partial class Generar
    {
        public int GenId { get; set; }
        public int TurId { get; set; }
        public int DiaId { get; set; }
        public int BoxId { get; set; }
        public string GenHoraini { get; set; }
        public string GenHorafin { get; set; }
        public short GenInterva { get; set; }
        public string GenEstado { get; set; }
        public DateTime GenFalta { get; set; }
    }
}
