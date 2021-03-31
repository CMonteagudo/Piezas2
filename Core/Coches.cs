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
  /// <summary> Obtiene los datos de los coches desde la base de datos </summary>
  public class Coches
    {
    private DbPiezasContext DbCtx;
    private HttpContext HttpCtx;
    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public Coches( HttpContext HttpCtx )
      {
      this.HttpCtx = HttpCtx;

      DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene una lista de coches de acuerdo a los firtros definidos en 'datos' (Marca, Modelo o Motor) </summary>
    public List<CocheDescFull> FullDesc( string datos )
      {
      var Items = new List<CocheDescFull>();

      var parms = new ItemsFilters( datos, HttpCtx );
      if( parms.Marca.Id < 0 || parms.Modelo.Id < 0 || parms.Motor.Id < 0 ) return Items;

      var pass1 = DbCtx.Coches;
      var pass2 = parms.Marca.Id  > 0? pass1.Where( c => c.Marca  == parms.Marca.Id  ) : pass1;
      var pass3 = parms.Modelo.Id > 0? pass2.Where( c => c.Modelo == parms.Modelo.Id ) : pass2;
      var pass4 = parms.Motor.Id  > 0? pass3.Where( c => c.Motor  == parms.Motor.Id  ) : pass3;

      var coches = pass4.Include( c => c.MarcaNavigation ).Include( c => c.ModeloNavigation ).Include( c => c.MotorNavigation );

      foreach( var coche in coches )
        Items.Add( new CocheDescFull( coche ) );

      return Items;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos de un coche conociendo su ID </summary>
    public Coche Find( int id )
      {
      if( DbCtx.Coches.Find(id) == null ) return null;

      return DbCtx.Coches.Where( c => c.Id==id )?.Include( c => c.MarcaNavigation ).Include( c => c.ModeloNavigation ).Include( c => c.MotorNavigation )?.First(); 
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de todos los coches, donde solo se retornan los datos principales (Id,Marca,Modelo y Motor) </summary>
    public List<CocheDesc> AllDesc()
      {
      var coches  = DbCtx.Coches.Include( c => c.MarcaNavigation )
                                .Include( c => c.ModeloNavigation )
                                .Include( c => c.MotorNavigation )
                                .Select( c => new CocheDesc(c) );

      return coches.ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene una cadena que representa el nombre del coche  </summary>
    public string CocheName( Coche coche, bool motor=false  )
      {
      var marca = coche.MarcaNavigation !=null? coche.MarcaNavigation.Nombre  : DbCtx.Marcas.Find(coche.Marca  )?.Nombre ?? "";
      var model = coche.ModeloNavigation!=null? coche.ModeloNavigation.Nombre : DbCtx.Modelos.Find(coche.Modelo)?.Nombre ?? "";

      var name = $"{marca} {model}".Trim();
      if( motor )
        { 
        var moto = coche.MotorNavigation!=null? coche.MotorNavigation.Nombre : DbCtx.Motors.Find(coche.Motor)?.Nombre ?? "";

        if( moto.Length >0 ) name += " para " + moto;
        }

      return name;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene una lista de los coches que usan el item dado por 'idItem' si items es igual a cero los ratorna todos </summary>
    public List<CocheDesc> ForItem( int idItem )
      {
      string sql;

      if( idItem != 0 )
        sql = "SELECT c.* FROM Coche AS c INNER JOIN  ItemCoche AS ic ON c.Id = ic.IdCoche WHERE ic.IdItem = " + idItem;
      else
        sql = "SELECT c.* FROM Coche AS c";

      var coches  = DbCtx.Coches.FromSqlRaw( sql ).Include( c => c.MarcaNavigation ).Include( c => c.ModeloNavigation ).Include( c => c.MotorNavigation ).ToList();

      var lst = new List<CocheDesc>();
      foreach(var coche in coches )
        lst.Add( new CocheDesc( coche) );

      return lst;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Guarda los cambios ralizados a un coche o crea uno nuevo (Id==0) </summary>
    public int Change( Coche coche )
      {
      if( coche.Id == 0 )
        {
        var newItem = DbCtx.Coches.Add( coche );

        DbCtx.SaveChanges();
        return newItem.Entity.Id;
        }
      else
        {
        var edtCoche =  DbCtx.Coches.Find( coche.Id );
        if( edtCoche==null ) return 0;

        edtCoche.Marca       = coche.Marca;
        edtCoche.Modelo      = coche.Modelo;
        edtCoche.Motor       = coche.Motor;
        edtCoche.Foto        = coche.Foto;
        edtCoche.Caja        = coche.Caja;
        edtCoche.Carroceria  = coche.Carroceria;
        edtCoche.Descripcion = coche.Descripcion;

        DbCtx.SaveChanges();
        return coche.Id;
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra el coche con identificado id </summary>
    public void Delete( int id )
      {
      var coche = DbCtx.Coches.Find( id );
      if( coche == null ) return;

      DbCtx.Coches.Remove( coche );

      DbCtx.SaveChanges();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Garda los cambios ralizados a un coche o crea uno nuevo (Id==0) </summary>
    public void ChangeFoto( int id, string fName )
      {
      DbCtx.Coches.Find( id ).Foto = fName;

      DbCtx.SaveChanges();
      }
    }

  //---------------------------------------------------------------------------------------------------------------------------------------
  /// <summary> Mantiene los datos un coche, de una manera resumida y mas descriptiva  </summary>
  public class CocheDesc
    {
    public CocheDesc( Coche coche )
      {
      Id = coche.Id;
      Marca = coche.MarcaNavigation.Nombre;
      Modelo = coche.ModeloNavigation.Nombre;
      Motor = coche.MotorNavigation.Nombre;
      }

    public int Id { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Motor { get; set; }
    }

  //---------------------------------------------------------------------------------------------------------------------------------------
  /// <summary> Todos los datos del coche más los mombres de la marca, el modelo y el motor  </summary>
  public class CocheDescFull
    {
    public CocheDescFull( Coche coche )
      {
      Id          = coche.Id;
      IdMarca     = coche.Marca;
      IdModelo    = coche.Modelo;
      IdMotor     = coche.Motor;
      Caja        = coche.Caja;
      Carroceria  = coche.Carroceria;
      Foto        = coche.Foto;
      Description = coche.Descripcion;

      Marca  = coche.MarcaNavigation.Nombre;
      Modelo = coche.ModeloNavigation.Nombre;
      Motor  = coche.MotorNavigation.Nombre;

      Combustible = coche.MotorNavigation.Combustible == 0 ? "gasolina" : "petroleo";
      Capacidad   = coche.MotorNavigation.Capacidad;
      Potencia    = coche.MotorNavigation.Potencia;

      if( Foto        == null ) Foto = "";
      if( Description == null ) Description = "";
      if( Caja        == null ) Caja = "";
      if( Carroceria  == null ) Carroceria = "";
      }

    public int Id { get; set; }
    public int IdMarca { get; set; }
    public int IdModelo { get; set; }
    public int IdMotor { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Motor { get; set; }
    public string Caja { get; set; }
    public string Carroceria { get; set; }
    public string Foto { get; set; }
    public string Description { get; set; }

    public string Combustible { get; set; }
    public int Capacidad { get; set; }
    public int Potencia { get; set; }
    }

  }