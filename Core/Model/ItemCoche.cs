using System;
using System.Collections.Generic;

#nullable disable

namespace Piezas2.Core.Model
{
    public partial class ItemCoche
    {
        public int IdItem { get; set; }
        public int IdCoche { get; set; }

        public virtual Coche IdCocheNavigation { get; set; }
        public virtual Item IdItemNavigation { get; set; }
    }
}
