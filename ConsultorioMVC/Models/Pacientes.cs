namespace ConsultorioMVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Pacientes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pacientes()
        {
            HistoriasClinicas = new HashSet<HistoriasClinicas>();
        }

        public int id { get; set; }

        [StringLength(60)]
        public string nombre { get; set; }

        [StringLength(60)]
        public string apellido { get; set; }

        [StringLength(60)]
        public string telefono { get; set; }

        [StringLength(100)]
        public string direccion { get; set; }

        [StringLength(60)]
        public string localidad { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha_nacimiento { get; set; }

        public int? obra_social_id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistoriasClinicas> HistoriasClinicas { get; set; }
    }
}
