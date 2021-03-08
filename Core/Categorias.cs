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
    private DbPiezasContext DbCtx;
    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto obtener categorias y subcategorias de la base de datos </summary>
    public Categorias( HttpContext HttpCtx )
      {
      DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                        // Obtiene contexto a la BD

      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todas las categorias que hay en la base de dataos </summary>
    public List<IdName> getCategorias()
      {
      return DbCtx.Categoria.OrderBy( x => x.Nombre ).Select( x => new IdName( x.Id, x.Nombre ) ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el nombre de una categoria, conociendo su Id </summary>
    public string findNombre( int id)
      {
      return DbCtx.Categoria.Find(id)?.Nombre ?? "";
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todas las sub-categorias que hay en la base de dataos </summary>
    public List<IdName> getSubCategorias()
      {
      var Items = new  List<IdName>();

      foreach( var cat in DbCtx.SubCategoria )
        if( cat.Id % 10000 != 0 )
          Items.Add( new IdName( cat.Id, cat.Nombre )  );

      return Items;
      }

    }

  }
