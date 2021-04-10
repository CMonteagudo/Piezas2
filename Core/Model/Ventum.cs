using System;
using System.Collections.Generic;

#nullable disable

namespace Piezas2.Core.Model
{
    public partial class Ventum
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UsuarioId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public int Estado { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal? Monto { get; set; }

        public virtual Item Item { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
