namespace ConsultorioMVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HistoriasClinicas
    {
        public int id { get; set; }

        public int paciente_id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha { get; set; }

        [StringLength(1500)]
        public string descripcion { get; set; }

        public virtual Pacientes Pacientes { get; set; }
    }
}
