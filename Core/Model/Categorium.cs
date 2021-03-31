using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace Piezas2.Core.Model
  {
  public partial class Categorium
    {
    public Categorium()
      {
      Items = new HashSet<Item>();
      }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Logo { get; set; }
    public string Descripcion { get; set; }

    [JsonIgnore]
    public virtual ICollection<Item> Items { get; set; }
    }

  public partial class SubCategoria
    {
    public int Id { get; set; }
    public string Nombre { get; set; }
    }
  }
