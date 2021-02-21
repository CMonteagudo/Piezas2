using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Piezas2.Core.Model
  {
  //=======================================================================================================================================
  /// <summary> Para guardar los datos de los motores y a los coches que pertenecen </summary>
  public partial class MotorCoche
    {
    public MotorCoche()
      {
      }

    public MotorCoche( MotorCoche x )
      {
      Id     = x.Id;
      Nombre = x.Nombre;
      Marca  = x.Marca;
      Modelo = x.Modelo;
      }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Marca { get; set; }
    public int Modelo { get; set; }

    [JsonIgnore]
    public virtual Marca MarcaNavigation { get; set; }
    [JsonIgnore]
    public virtual Modelo ModeloNavigation { get; set; }
    }

  }
