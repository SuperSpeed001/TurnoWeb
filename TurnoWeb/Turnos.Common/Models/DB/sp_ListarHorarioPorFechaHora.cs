namespace Turnos.Common.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class sp_ListarHorarioPorFechaHora
    {
        public int hor_id { get; set; }
        public string hor_hora { get; set; }
        public string hor_dia { get; set; }
        public string hor_fecha { get; set; }
        public bool hor_reserv { get; set; }
        public string tur_nombre { get; set; }
        public string box_nombre { get; set; }
        

    }
}
