using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace Piezas2.Core.Model
{
    public partial class Item
    {
        public Item()
        {
            ItemCoches = new HashSet<ItemCoche>();
        }

        public int Id { get; set; }
        public int Fabricante { get; set; }
        public int Categoria { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Foto { get; set; }
        public decimal? Precio { get; set; }
        public string Descripcion { get; set; }

        [JsonIgnore]
        public virtual Categorium CategoriaNavigation { get; set; }
        [JsonIgnore]
        public virtual Fabricante FabricanteNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<ItemCoche> ItemCoches { get; set; }
    }
}
