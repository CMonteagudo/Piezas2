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
    private readonly DbPiezasContext DbCtx;                                // Mantiene contexto a la BD

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los fabricantes de la base de datos </summary>
    public Fabricantes( HttpContext HttpCtx )
      {
      DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todos los fabricantes, donde se obtiene el id y el nombre de cada fabricante </summary>
    public List<IdName> ListIdName()
      {
      return DbCtx.Fabricantes.Select( x => new IdName( x.Id, x.Nombre ) ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todos los fabricantes, donde se obtienen todos los datos para cada fabricante </summary>
    public List<Fabricante> ListFabricantes()
      {
      return DbCtx.Fabricantes.ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra el fabricante con el Id dado y retorna sus datos, sino retorna un objeto vacio </summary>
    public Fabricante Find( int id )
      {
      var fab = DbCtx.Fabricantes.Find( id );
      if( fab == null ) fab = new Fabricante{ Id=0 };

      return fab; 
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia los datos de un fabricante existente o inserta una nueva </summary>
    public int Change( Fabricante fab )
      {
      if( fab.Id == 0 )
        {
        var newItem = DbCtx.Fabricantes.Add( fab );

        DbCtx.SaveChanges();
        return newItem.Entity.Id;
        }
      else
        {
        var edtMaker =  DbCtx.Fabricantes.Find( fab.Id );
        if( edtMaker == null ) return 0;

        edtMaker.Nombre = fab.Nombre;
        edtMaker.Logo = fab.Logo;
        edtMaker.Descripcion = fab.Descripcion;

        DbCtx.SaveChanges();
        return fab.Id;
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra el fabricante con identificador 'id' </summary>
    public void Delete( int id )
      {
      var maker = DbCtx.Fabricantes.Find( id );
      if( maker == null ) return;

      DbCtx.Fabricantes.Remove( maker );

      DbCtx.SaveChanges();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia el logo del fabricante con identificador 'Id', por el dado en 'fName' </summary>
    public void ChangeLogo( int id, string fName )
      {
      DbCtx.Fabricantes.Find( id ).Logo = fName;

      DbCtx.SaveChanges();
      }

    }
  }
