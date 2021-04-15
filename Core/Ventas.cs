using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Maneja todas las acciones relacionadas con las ventas del sistema </summary>
  internal class Ventas
    {
    readonly DbPiezasContext DbCtx;            // Conecto para acceder a la base de datos
    readonly ISession        Session;

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto con informacion se la conexion </summary>
    public Ventas( HttpContext HttpCtx )
      {
      DbCtx = (DbPiezasContext)HttpCtx.RequestServices.GetService( typeof( DbPiezasContext ) );         // Obtiene contexto a la BD

      Session = HttpCtx.Session;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra la venta con el Id dado y retorna sus datos, sino un objeto usuario vacio </summary>
    public Ventum Find( int id )
      {
      var venta = DbCtx.Ventas.Find( id );
      if( venta == null ) venta = new Ventum { Id = 0 };

      return venta;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todas las ventas, donde se obtienen todos los datos para cada venta 
    ///           Estado => 0- En el carrito, 1- En proceso de pago, 100- Pagado</summary>
    public List<Ventum> ListVentas( int UserId, int Estado )
      {
      var Admin = (Session.GetInt32( "Admin" ) == 1);

      if( UserId<0 && Estado<0 )                              // Obtiene el listado de todas las ventas
        {
        if( Admin ) return DbCtx.Ventas.ToList();             // Es posible si el usuario es un administrador

        throw new Exception("AdminAccess");                   // Si no es administrador emite una excepción
        }

      var logUser = Session.GetInt32( "UserId" );             // Identificador del usuario logueado
      if( logUser != UserId && !Admin )                       // Solo si es el usurio loguedo o el administrador
        throw new Exception( "UserAccess" );                 

      if( Estado<0 ) return DbCtx.Ventas.Where( v=> v.UsuarioId==UserId ).ToList();             // Todas la ventas para un usuario

      return DbCtx.Ventas.Where( v => v.UsuarioId==UserId && v.Estado==Estado ).ToList();       // Todas la ventas para un usuario en un estado especifico
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la cantidad de productos que hay en el carrito 
    internal int CarritoCount( int userId )
      {
      var logUser = Session.GetInt32( "UserId" );             // Si no es el usuario loguedo retorna 0
      if( logUser != userId ) return 0;                     

      return DbCtx.Ventas.Where( v => v.UsuarioId == userId ).Count();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos de todos los item que hay en el carrito para el usuario logueado
    internal List<carDatos> CarritoDatos( int userId )
      {
      var logUser = Session.GetInt32( "UserId" );             // Si no es el usuario loguedo retorna 0
      if( logUser != userId ) throw new Exception("UserAccess");

      return DbCtx.Ventas.Where( v => v.UsuarioId == userId ).Include( v => v.Item ).Select( v => new carDatos( v ) ).ToList();
      }

    }

  //---------------------------------------------------------------------------------------------------------------------------------------
  /// <summary> Mantiene los datos que se muestran en el carrito
  internal class carDatos
    {
    public int Id { get; set; }
    public string Item { get; set; }
    public int Cant { get; set; }
    public decimal Prec { get; set; }

    public carDatos( Ventum v )
      {
      Id   = v.Id;
      Item = v.Item.Nombre;
      Cant = v.Cantidad;
      Prec = v.Precio;
      }
    }
  }