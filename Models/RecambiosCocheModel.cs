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
  public class RecambiosCocheModel : BaseModel
    {
    public List<GrupoItems> Grupos { set; get; } = new List<GrupoItems>();
    public string Marca { set; get; }
    public string Modelo { set; get; }
    public string Motor { set; get; }
    public int MarcaId { set; get; }
    public int ModeloId { set; get; }
    public int MotorId { set; get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public RecambiosCocheModel( int cocheId, HttpContext HttpCtx ) : base( HttpCtx )
      {
      if( ! fillCocheDatos( cocheId, HttpCtx ) ) return;

      var catTable = new Categorias( HttpCtx );
      var Items    = new Recambios( HttpCtx ).FindByDatos( $"Mar-{MarcaId}/Mod-{ModeloId}/Mot-{MotorId}/orden-categoria/rango-0-10000" ).Items;

      GrupoItems grp = null;
      int lastCat = -1;
      foreach( var item in Items )
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

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Busca el coche con id 'cocheId' y toma sus datos </summary>
    private bool fillCocheDatos( int cocheId, HttpContext HttpCtx )
      {
      var coche = new Coches(HttpCtx).Find( cocheId );
      if( coche==null ) return false;

      MarcaId = coche.Marca;
      Marca   = coche.MarcaNavigation.Nombre;

      ModeloId = coche.Modelo;
      Modelo   = coche.ModeloNavigation.Nombre;

      MotorId = coche.Motor;
      Motor   = coche.MotorNavigation.Nombre;

      return true;
      }
    }

  }
  