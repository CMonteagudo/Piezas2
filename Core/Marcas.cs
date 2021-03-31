using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Maneja todas las acciones relacionadas con las marcas de los coches </summary>
  public class Marcas
    {
    DbPiezasContext DbCtx;            // Conecto para acceder a la base de datos

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto con informacion se la conexion </summary>
    public Marcas( HttpContext HttpCtx )
      {
      DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todas las marcas, donde se obtiene el id y el nombre de cada marca </summary>
    public List<IdName> ListIdName()
      {
      return DbCtx.Marcas.Select( x => new IdName( x.Id, x.Nombre ) ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todas las marcas, donde se obtienen todos los datos para cada marca </summary>
    public List<Marca> ListMarcas()
      {
      return DbCtx.Marcas.ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra la marca con el Id dado y retorna sus datos, sino un objeto marca vacio </summary>
    public Marca Find( int id )
      {
      var marca = DbCtx.Marcas.Find( id );
      if( marca == null ) marca = new Marca { Id = 0 };

      return marca;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra la marca con una cadeda se caracteres que puede ser su nombre o su id, sino lo puede encontrat retorna un objeto marca vacio </summary>
    public Marca Find( string sMarca )
      {
      if( int.TryParse( sMarca, out int idMarca ) )
        return Find( idMarca );

      var marca = DbCtx.Marcas.FirstOrDefault( x => x.Nombre == sMarca );
      if( marca == null ) marca = new Marca { Id = 0 };

      return marca;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia los datos de una marca existente o inserta una nueva </summary>
    public int Change( Marca marca )
      {
      if( marca.Id == 0 )
        {
        var newItem = DbCtx.Marcas.Add( marca );

        DbCtx.SaveChanges();
        return newItem.Entity.Id;
        }
      else
        {
        var edtMarca =  DbCtx.Marcas.Find( marca.Id );
        if( edtMarca == null ) return 0;

        edtMarca.Nombre = marca.Nombre;
        edtMarca.Logo = marca.Logo;
        edtMarca.Descripcion = marca.Descripcion;

        DbCtx.SaveChanges();
        return marca.Id;
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra la marca con identificador 'id' </summary>
    public void Delete( int id )
      {
      var marca = DbCtx.Marcas.Find( id );
      if( marca == null ) return;

      DbCtx.Marcas.Remove( marca );

      DbCtx.SaveChanges();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia el logo de la marca con identificador 'Id', por el dado en 'fName' </summary>
    public void ChangeLogo( int id, string fName )
      {
      DbCtx.Marcas.Find( id ).Logo = fName;

      DbCtx.SaveChanges();
      }

    }
  }