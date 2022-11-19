namespace ConsultorioMVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Usuarios
    {
        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string email { get; set; }

        [Required]
        [StringLength(60)]
        public string password { get; set; }
    }
}
