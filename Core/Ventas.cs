using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
    ///           Estado => 0- En el carrito, 1- Pagado</summary>
    public List<Ventum> ListVentas( int userId, int Estado )
      {
      var Admin = (Session.GetInt32( "Admin" ) == 1);

      if( userId < 0 && Estado < 0 )                                // Obtiene el listado de todas las ventas
        {
        if( Admin )
          return DbCtx.Ventas.Include( v => v.Item ).ToList();      // Es posible si el usuario es un administrador

        throw new Exception( "AdminAccess" );                       // Si no es administrador emite una excepción
        }

      if( !Admin ) checkUser( userId );                             // Solo administrador o el usuario logueado

      if( Estado < 0 ) return DbCtx.Ventas.Where( v => v.UsuarioId == userId ).Include( v => v.Item ).ToList();             // Todas la ventas para un usuario

      return DbCtx.Ventas.Where( v => v.UsuarioId == userId && v.Estado == Estado ).Include( v => v.Item ).ToList();        // Todas la ventas para un usuario en un estado especifico
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la cantidad de productos que hay en el carrito </summary>
    internal int CarritoCount( int userId )
      {
      if( !checkUser( userId, false ) ) return 0;                   // Si no es el usuario loguedo retorna 0

      return DbCtx.Ventas.Where( v => v.UsuarioId == userId && v.Estado==0 ).Count();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos de todos los item que hay en el carrito para el usuario logueado </summary>
    internal List<carDatos> CarritoDatos( int userId )
      {
      checkUser( userId );

      return DbCtx.Ventas.Where( v => v.UsuarioId == userId ).Include( v => v.Item ).Select( v => new carDatos( v ) ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona una venta de 'cant' articulos con identificador 'id' al usuario 'userId' </summary>
    internal int Add( int cant, int itemId, int userId )
      {
      checkUser( userId );

      var item  = DbCtx.Items.Find( itemId );
      if( item == null ) throw new Exception( "NoItemExist" );

      var precio = item.Precio.HasValue? item.Precio.Value : 0m;
      var venta = new Ventum{ ItemId=itemId, UsuarioId=userId, Cantidad=cant, Precio=precio };

      var newItem = DbCtx.Ventas.Add( venta );

      DbCtx.SaveChanges();
      return newItem.Entity.Id;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica los datos de una venta ya existente </summary>
    internal int Modify( Ventum venta )
      {
      checkUser( venta.UsuarioId );

      var edVenta =  DbCtx.Ventas.Find( venta.Id );                   // Busca la venta indicada
      if( edVenta == null ) throw new Exception( "NoVentaExist" );

      edVenta.ItemId = venta.ItemId;
      edVenta.UsuarioId = venta.UsuarioId;
      edVenta.Cantidad = venta.Cantidad;
      edVenta.Precio = venta.Precio;
      edVenta.Estado = venta.Estado;
      edVenta.FechaPago = venta.FechaPago;

      DbCtx.SaveChanges();
      return venta.Id;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica las cantidades de una lista de ventas, las lista es expresa por una cadena JSON [{id:__, cant:__}, {id:__, cant:__}, ....] </summary>
    internal int ModifyCounts( string ventas, int userId )
      {
      checkUser( userId );

      var pru  = JsonSerializer.Deserialize<List<(int id, int cant)>>(ventas);
      var spru = JsonSerializer.Serialize(pru);

      var lst = JsonSerializer.Deserialize<List<IdCantPar>>(ventas);

      int count = 0;
      foreach( var venta in lst )
        {
        var edVenta =  DbCtx.Ventas.Find( venta.id );                   // Busca la venta indicada
        if( edVenta == null ) throw new Exception( "NoVentaExist" );

        edVenta.Cantidad = venta.cant;

        DbCtx.SaveChanges();
        ++count;
        }

      return count;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Pone todas la ventas que hay en el carrito como pagadas </summary>
    public int SetPagadas( int userId, DateTime fecha )
      {
      var lst = DbCtx.Ventas.Where( v => v.UsuarioId == userId && v.Estado == 0 ).ToList();                                 // Todas la ventas que estan en el carrito
      foreach( var venta in lst )
        {
        venta.Estado = 1;
        venta.FechaPago = fecha;

        DbCtx.SaveChanges();
        }

      return lst.Count();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra la venta indicada </summary>
    internal bool Delete( int ventaId, int userId )
      {
      checkUser( userId );

      var venta = DbCtx.Ventas.Find( ventaId );
      if( venta == null ) return false;

      DbCtx.Ventas.Remove( venta );

      DbCtx.SaveChanges();
      return true;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Checkea que el usuario dado esta logueado, 'Excep' define si se genera una excepción o no en caso de no cumplirse </summary>
    internal bool checkUser( int userId, bool Excep = true )
      {
      var logUser = Session.GetInt32( "UserId" );                 // Si no es el usuario loguedo retorna 0
      if( logUser != userId )
        {
        if( Excep ) throw new Exception( "UserAccess" );
        else return false;
        }

      return true;
      }

    }


  public class IdCantPar
    {
    public int id { get; set; }
    public int cant { get; set; }
    }

  //---------------------------------------------------------------------------------------------------------------------------------------
  /// <summary> Mantiene los datos que se muestran en el carrito </summary>
  internal class carDatos
    {
    public int Id { get; set; }
    public string Item { get; set; }
    public int Cant { get; set; }
    public decimal Prec { get; set; }

    public carDatos( Ventum v )
      {
      Id = v.Id;
      Item = v.Item.Nombre;
      Cant = v.Cantidad;
      Prec = v.Precio;
      }
    }

  }
