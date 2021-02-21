using System;
using System.Collections.Generic;

#nullable disable

namespace Piezas2.Core.Model
{
    public partial class Coche
    {
        public Coche()
        {
            ItemCoches = new HashSet<ItemCoche>();
        }

        public int Id { get; set; }
        public int Marca { get; set; }
        public int Modelo { get; set; }
        public int Motor { get; set; }
        public string Caja { get; set; }
        public string Carroceria { get; set; }
        public string Foto { get; set; }
        public string Description { get; set; }

        public virtual Marca MarcaNavigation { get; set; }
        public virtual Modelo ModeloNavigation { get; set; }
        public virtual Motor MotorNavigation { get; set; }
        public virtual ICollection<ItemCoche> ItemCoches { get; set; }
    }
}
