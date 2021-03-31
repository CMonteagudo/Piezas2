using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene una lista los modelos para una marca determinada, o si no se suministra la marca, retorna todos los modelos </summary>
  public class Modelos
    {
    readonly DbPiezasContext DbCtx;            // Conecto para acceder a la base de datos
    readonly HttpContext     HttpCtx;

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los modelos de la base de datos </summary>
    public Modelos( HttpContext httpCtx )
      {
      HttpCtx = httpCtx;
      DbCtx = (DbPiezasContext) httpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todos los modelos, donde se obtiene el id y el nombre de cada marca </summary>
    public List<IdName> ListIdName()
      {
      return DbCtx.Modelos.OrderBy( x => x.Nombre ).Select( x => new IdName( x.Id, x.Nombre ) ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todos los modelos para una marca dada, donde se obtiene el id y el nombre de cada marca </summary>
    public List<IdName> ListIdName( int idMarca)
      {
      return DbCtx.Modelos.Where( x => x.Marca == idMarca ).OrderBy( x => x.Nombre ).Select( x => new IdName( x.Id, x.Nombre ) ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todos los modelo, donde se obtienen todos los datos para cada modelo </summary>
    public List<Modelo> ListModelos()
      {
      return DbCtx.Modelos.OrderBy( x => x.Nombre ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todos los modelos para una marca dada, donde se obtienen todos los datos para cada modelo </summary>
    public List<Modelo> ListModelos( int idMarca )
      {
      return DbCtx.Modelos.Where( x => x.Marca == idMarca ).OrderBy( x => x.Nombre ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todos los modelos para una marca dada, donde se obtienen todos los datos para cada modelo </summary>
    public List<Modelo> ListModelos( string sMarca )
      {
      var marca = new Marcas(HttpCtx).Find(sMarca);
      if( marca.Id>0 ) return ListModelos( marca.Id );

      return new List<Modelo>();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra la marca con el Id dado y retorna sus datos, sino un objeto marca vacio </summary>
    public Modelo Find( int id )
      {
      var modelo = DbCtx.Modelos.Find( id );
      if( modelo == null ) modelo = new Modelo { Id = 0 };

      return modelo;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra la marca con el Id dado y retorna sus datos, sino un objeto marca vacio </summary>
    public Modelo Find( string sModelo )
      {
      if( int.TryParse( sModelo, out int idModelo ) )
        return Find( idModelo );

      var modelo = DbCtx.Modelos.FirstOrDefault( x => x.Nombre == sModelo );
      if( modelo == null ) modelo = new Modelo { Id = 0 };

      return modelo;
      }


    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia los datos de un modelo existente o inserta uno nuevo </summary>
    public int Change( Modelo modelo )
      {
      if( modelo.Id == 0 )
        {
        var newItem = DbCtx.Modelos.Add( modelo );

        DbCtx.SaveChanges();
        return newItem.Entity.Id;
        }
      else
        {
        var mod =  DbCtx.Modelos.Find( modelo.Id );
        if( mod == null ) return 0;

        mod.Nombre = modelo.Nombre;
        mod.Marca  = modelo.Marca;
        mod.Descripcion = modelo.Descripcion;

        DbCtx.SaveChanges();
        return modelo.Id;
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra el modelo con identificador 'id' </summary>
    public void Delete( int id )
      {
      var modelo = DbCtx.Modelos.Find( id );
      if( modelo == null ) return;

      DbCtx.Modelos.Remove( modelo );

      DbCtx.SaveChanges();
      }

    }
  }