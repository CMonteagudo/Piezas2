using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static Piezas2.Core.Coches;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene los datos de un recammbio especifico desde la base de datos </summary>
  public class RecambioUsos
    {
    readonly DbPiezasContext DbCtx;
    readonly int             itemId;
    readonly HttpContext     HttpCtx;

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public RecambioUsos( int id, HttpContext httpCtx )
      {
      HttpCtx = httpCtx;
      itemId  = id;
      DbCtx = (DbPiezasContext) httpCtx.RequestServices.GetService( typeof( DbPiezasContext ) );                                // Obtiene contexto a la BD
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los coches que usan el recambio, si 'InUso' es distinto de 0 se obtienen todos los coches y el dato 'UseItem'
    /// se coloca en 0 ó 1 si se usa o no, en otro caso se retornan solo los coches donde se usa el recambio</summary>
    public CochesInUso UsoInCoches()
      {
      var datos = new CochesInUso();

      if( itemId != 0 )
        { 
        var Recamb = DbCtx.Items.Find( itemId );
        if( Recamb == null ) return datos;

        datos.Nombre = Recamb.Nombre;
        }
      else
        datos.Nombre = "Todos";

      datos.Coches = new Coches( HttpCtx ).ForItem( itemId );

      return datos;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra todos los coches de la lista que estan asociados al recambio, lo que significa que no usan el recambio </summary>
    public int DeleteCoches( string lstCoches )
      {
      int numDel = 0;
      var sCoches = lstCoches.Split(' ');
      foreach( var sCoche in sCoches )
        {
        if( int.TryParse( sCoche, out int cocheId ) )
          { 
          var coche = DbCtx.ItemCoches.Find( itemId, cocheId );
          if( coche != null )
            { 
            DbCtx.ItemCoches.Remove( coche  );
            ++numDel;
            }
          }
        }

      DbCtx.SaveChanges();
      return numDel;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Asocia todos los coches de la lista al recambio para que se considere que usan el recambio </summary>
    public int AddCoches( string lstCoches )
      {
      int numAdd = 0;
      var sCoches = lstCoches.Split(' ');
      foreach( var sCoche in sCoches )
        {
        if( int.TryParse( sCoche, out int cocheId ) )
          {
          var coche = new ItemCoche{ IdItem=itemId, IdCoche=cocheId } ;

          DbCtx.ItemCoches.Add( coche );
          ++numAdd;
          }
        }

      try { DbCtx.SaveChanges();  }
      catch( Exception ) { numAdd = 0; }

      return numAdd;
      }

    } //=======================================================================================================================================

  //=======================================================================================================================================
  /// <summary> Matiene la información de los coches donde se usa un item determinado </summary>
  public class CochesInUso
    {
    public string Nombre { get; set; } = "";
    public List<CocheDesc> Coches { get; set; } = new List<CocheDesc>();
    }

  }