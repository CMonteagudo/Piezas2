using Microsoft.AspNetCore.Http;
using Piezas2.Core;
using System.Collections.Generic;

namespace Piezas2.Models
  {
  //=======================================================================================================================================
  /// <summary> Obtiene todos los datos de un recambio en especifico  </summary>
  public class RecambioModel
    {
    private Recambio Item { get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public RecambioModel( int id, HttpContext HttpCtx )
      {
      Item = new Recambio( id, HttpCtx );
      }

    public int Id { get { return Item.IdItem; } }
    public int IdFab { get { return Item.IdFabricante; } }
    public string NameFab { get { return Item.Fabricante; } }
    public int IdCat { get { return Item.IdCategoria; } }
    public string NameCat{ get { return Item.Categoria; } }
    public string Nombre { get { return Item.Nombre; } }
    public string Codigo { get { return Item.Codigo; } }
    public string Foto { get { 
                             var foto = Item.Foto.Length==0? "NoImagen.jpg" : Item.Foto;
                             return $"/images/{foto}";
                             } } 
    public string Precio { get { return Item.Precio; } }
    public string Descripcion { get { return Item.Descripcion; } }
    public List<CocheDesc> Coches { get { return Item.Coches; } }
    }
  }