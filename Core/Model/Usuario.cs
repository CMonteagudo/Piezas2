using System;
using System.Collections.Generic;

#nullable disable

namespace Piezas2.Core.Model
{
    public partial class Usuario
    {
        public Usuario()
        {
            Venta = new HashSet<Ventum>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefonos { get; set; }
        public string PassWord { get; set; }
        public int Confirmado { get; set; }
        public int NLogin { get; set; }
        public int Admin { get; set; }

    public virtual ICollection<Ventum> Venta { get; set; }
    }
}
