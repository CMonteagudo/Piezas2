using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene los datos de un recammbio especifico desde la base de datos </summary>
  public class RecambioUsos
    {
    //public string Nombre { get; set; }
    //public List<CocheDesc> Coches { get; set; } = new List<CocheDesc>();

    DbPiezasContext DbCtx;
    int             itemId;

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public RecambioUsos( int id, HttpContext HttpCtx )
      {
      itemId = id;
      DbCtx = (DbPiezasContext)HttpCtx.RequestServices.GetService( typeof( DbPiezasContext ) );                                // Obtiene contexto a la BD
      }

    //public RecambioUso( int id, int InUso, HttpContext HttpCtx )
    //  {
    //  var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD

    //  var Recamb = DbCtx.Items.Find( id );
    //  if( Recamb==null ) return;

    //  Nombre = Recamb.Nombre;

    //  //var sql = "SELECT c.* FROM Coche AS c INNER JOIN  ItemCoche AS ic ON c.Id = ic.IdCoche WHERE ic.IdItem = {0}";
    //  //var coches  = DbCtx.Coches.FromSqlRaw(sql,id).Include( c => c.MarcaNavigation ).Include( c => c.ModeloNavigation ).Include( c => c.MotorNavigation ).ToList();

    //  var coches  = DbCtx.Coches.Include( c => c.MarcaNavigation )
    //                            .Include( c => c.ModeloNavigation )
    //                            .Include( c => c.MotorNavigation )
    //                            .Include( c => c.ItemCoches.Where( ic=> ic.IdItem==id ) ).ToList();

    //  foreach( var coche in coches )
    //    if( InUso==0 || coche.ItemCoches.Count>0  )
    //      Coches.Add( new CocheDesc(coche) );
    //  }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los coches que usan el recambio, si 'InUso' es distinto de 0 se obtienen todos los coches y el dato 'UseItem'
    /// se coloca en 0 ó 1 si se usa o no, en otro caso se retornan solo los coches donde se usa el recambio</summary>
    public CochesInUso UsoInCoches( int InUso )
      {
      var datos = new CochesInUso();

      var Recamb = DbCtx.Items.Find( itemId );
      if( Recamb == null ) return datos;

      datos.Nombre = Recamb.Nombre;

      var coches  = DbCtx.Coches.Include( c => c.MarcaNavigation )
                                .Include( c => c.ModeloNavigation )
                                .Include( c => c.MotorNavigation )
                                .Include( c => c.ItemCoches.Where( ic=> ic.IdItem==itemId ) ).ToList();

      foreach( var coche in coches )
        if( InUso==0 || coche.ItemCoches.Count > 0 )
          datos.Coches.Add( new CocheDesc( coche ) );

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

  public class CochesInUso
    {
    public string Nombre { get; set; } = "";
    public List<CocheDesc> Coches { get; set; } = new List<CocheDesc>();
    }

  }