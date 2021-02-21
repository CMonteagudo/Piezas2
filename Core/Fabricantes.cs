using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene la lista de todas los fabricantes de recambio disponibles en la base de datos </summary>
  public class Fabricantes
    {
    /// <summary> Lista de todos los fabricantes de recambios que hay en la base de datos </summary>
    public List<IdName> Items { get; set; } = new List<IdName>();

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los fabricantes de la base de datos </summary>
    public Fabricantes( HttpContext HttpCtx )
      {
      var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD

      Items = DbCtx.Fabricantes.OrderBy( x => x.Nombre ).Select( x => new IdName( x.Id, x.Nombre ) ).ToList();
      }
    }

  }
