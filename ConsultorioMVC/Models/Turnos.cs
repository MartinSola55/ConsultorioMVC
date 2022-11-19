namespace ConsultorioMVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Turnos
    {
        public int id { get; set; }

        public int persona_id { get; set; }

        public int dia_horario_id { get; set; }

        public virtual DiaHorario DiaHorario { get; set; }

        public virtual Personas Personas { get; set; }
    }
}
