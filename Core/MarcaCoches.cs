using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene la lista de todas las marcas de coche y su respectivo identificador </summary>
  public class MarcaCoches
    {
    /// <summary> Lista de todas las marcas de coches que hay en la base de datos </summary>
    public List<IdName> Items { get; set; } = new List<IdName>();

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene las marcas de la base de datos </summary>
    public MarcaCoches( HttpContext HttpCtx )
      {
      var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD

      Items = DbCtx.Marcas.Select( x => new IdName(x.Id, x.Nombre) ).ToList();
      }

    }
  }