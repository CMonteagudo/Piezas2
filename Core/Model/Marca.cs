using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace Piezas2.Core.Model
  {
  public partial class Marca
    {
    public Marca()
      {
      Coches = new HashSet<Coche>();
      }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Logo { get; set; }
    public string Descripcion { get; set; }

    [JsonIgnore]
    public virtual ICollection<Coche> Coches { get; set; }
    [JsonIgnore]
    public virtual ICollection<MotorCoche> MotorCoches { get; set; }

    }
  }
