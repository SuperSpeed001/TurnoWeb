using System;
using System.Collections.Generic;

namespace Turnos.Common.Models.DB
{
    public partial class Horarios
    {
        public int HorId { get; set; }
        public string HorDia { get; set; }
        public string HorHora { get; set; }
        public bool HorReserv { get; set; }
        public string HorEstado { get; set; }
        public DateTime HorFecha { get; set; }
        public DateTime HorFalta { get; set; }
        public int TurId { get; set; }
        public int BoxId { get; set; }
        public int PerId { get; set; }
    }
}
