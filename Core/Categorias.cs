using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene la lista de todas las categorias de recambio disponibles en la base de datos </summary>
  public class Categorias
    {
    /// <summary> Lista de todas las categorias de recambios que hay en la base de datos </summary>
    public List<IdName> Items { get; set; } = new List<IdName>();

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene las categorias de la base de datos </summary>
    public Categorias( HttpContext HttpCtx )
      {
      var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD

      Items = DbCtx.Categoria.OrderBy(x=>x.Nombre).Select( x => new IdName( x.Id, x.Nombre ) ).ToList();
      }

    }

  }
