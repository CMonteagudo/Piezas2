using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace Piezas2.Core.Model
  {
  public partial class Motor
    {
    public Motor()
      {
      Coches = new HashSet<Coche>();
      }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Combustible { get; set; }
    public int Capacidad { get; set; }
    public int Potencia { get; set; }

    [JsonIgnore]
    public virtual ICollection<Coche> Coches { get; set; }
    }
  }
