using Microsoft.AspNetCore.Http;
using Piezas2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Piezas2.Core.Model;

namespace Piezas2.Models
  {
  //=======================================================================================================================================
  /// <summary> Obtiene todos los datos necesarios para el cuadro de busqueda de recambios </summary>
  public class RecambiosFabricantesModel
    {
    public List<GrupoItems> Grupos { set; get; } = new List<GrupoItems>();
    public string Maker { set; get; }
    public int MakerId { set; get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public RecambiosFabricantesModel( string maker, HttpContext HttpCtx )
      {
      Maker = maker.Replace('-',' ') .ToUpper();

      var found = new Recambios( HttpCtx );
      found.FindByDatos( $"fab-{Maker}/orden-categoria/rango-0-10000" );

      MakerId = found.Filters.Fabricante.Id;

      var catTable = new Categorias( HttpCtx );

      GrupoItems grp = null;
      int lastCat = -1;
      foreach( var item in found.Items )
        {
        if( item.Categoria != lastCat )
          {
          if( grp != null ) Grupos.Add( grp );
          grp = new GrupoItems( item.Categoria, catTable.findNombre( item.Categoria ) );

          lastCat = item.Categoria;
          }

        if( grp.Items.Count<4 ) grp.Items.Add( item );
        }

      if( grp != null ) Grupos.Add( grp );
      }
    }

  //---------------------------------------------------------------------------------------------------------------------------------------
  /// <summary> Mantiene un conjunto de items agrupados bajo un nombre </summary>
  public class GrupoItems
    {
    public int Id { set; get; }
    public string Nombre { set; get; }
    public List<Item> Items { set; get; }

    public GrupoItems( int id, string nombre="" )
      {
      Id = id;
      Nombre = nombre;
      Items = new List<Item>();
      }
    }

  }
  