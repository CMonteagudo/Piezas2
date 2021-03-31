using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piezas2.Core
  {//=======================================================================================================================================
  /// <summary> Obtiene la lista de motores en la base de datos, se puede filtar la lista incluyendo una marca o el modelo </summary>
  public class Motores
    {
    /// <summary> Contexto para acceso a la base de datos </summary>
    readonly DbPiezasContext DbCtx;

    /// <summary> Indica que se busque solo los motores que se estan usando en algun coche </summary>
    bool enUso = false;           // Solo es necesario si no se usa 'marca' o 'modelo'

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto para manejar las operaciones con los motores de los coches </summary>
    public Motores( HttpContext HttpCtx )
      {
      DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));      // Obtiene contexto a la BD
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todos los coches que hay en la base de datos </summary>
    internal List<Motor> All()
      {
      return DbCtx.Motors.ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene de los datos del motor indicado con 'id' </summary>
    public Motor Find( int id )
      {
      var motor = DbCtx.Motors.Find( id );
      if( motor == null ) motor = new Motor { Id = 0 };

      return motor;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra el motor con el Id dado y retorna sus datos, sino un objeto marca vacio </summary>
    public Motor Find( string sMotor )
      {
      if( int.TryParse( sMotor, out int idMotor ) )
        return Find( idMotor );

      var motor = DbCtx.Motors.FirstOrDefault( x => x.Nombre == sMotor );
      if( motor == null ) motor = new Motor { Id = 0 };

      return motor;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia los datos de un motor existente o inserta uno nuevo </summary>
    public int Change( Motor motor )
      {
      if( motor.Id == 0 )
        {
        var newItem = DbCtx.Motors.Add( motor );

        DbCtx.SaveChanges();
        return newItem.Entity.Id;
        }
      else
        {
        var mot =  DbCtx.Motors.Find( motor.Id );
        if( mot == null ) return 0;

        mot.Nombre      = motor.Nombre;
        mot.Capacidad   = motor.Capacidad;
        mot.Combustible = motor.Combustible;
        mot.Potencia    = motor.Potencia;
        mot.Descripcion = motor.Descripcion;

        DbCtx.SaveChanges();
        return motor.Id;
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra el motor con identificador 'id' </summary>
    public void Delete( int id )
      {
      var motor = DbCtx.Motors.Find( id );
      if( motor == null ) return;

      DbCtx.Motors.Remove( motor );

      DbCtx.SaveChanges();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene una lista de los motores asociados a la marca y modelo de coches dados, si alguno es NULL no se tiene en cuenta </summary>
    public List<MotorCoche> FindForCoche( string marca, string modelo )
      {
      List<MotorCoche> Items = new List<MotorCoche>();

      int IdMarca  = GetIdMarca ( marca  );
      int IdModelo = GetIdModelo( modelo );

      if( IdMarca == -1 || IdModelo == -1 ) return Items;                                           // La marca o el modelo no se encuentran en la BD

      var hasCoche = IdMarca>0 || IdModelo>0;                                                       // Si hay que tener en cuenta el coche

      var sTable  = hasCoche || enUso? "Motor AS m INNER JOIN Coche AS c ON m.Id=c.Motor" : "Motor as m";
      var sFields = hasCoche || enUso? "m.Id, m.Nombre, c.Marca, c.Modelo"                : "m.Id, m.Nombre, Marca=-1, Modelo=-1";

      StringBuilder sWhere = new StringBuilder("");
      if( hasCoche )                                                                                // Si hay algun tipo de filtro
        {
        sWhere.Append( "WHERE " );

        if( IdMarca > 0 ) sWhere.Append( $"c.Marca = {IdMarca}" );                                  // Filtra por la marca de coche
        if( IdModelo > 0 )                                                                          // Filtra por el modelo del coche
          {
          if( IdMarca > 0 ) sWhere.Append( " AND " );
          sWhere.Append( $"c.Modelo = {IdModelo}" );
          }
        }

      var sql = $"SELECT DISTINCT {sFields} FROM {sTable} {sWhere}";                                // Sentencia SQL

      Items = DbCtx.MotorCoches.FromSqlRaw( sql ).Select( x => new MotorCoche( x ) ).ToList();      // Ejecuta la secuencia SQL
      Items.Sort( ( s1, s2 ) => s1.Nombre.CompareTo( s2.Nombre ) );

      return Items;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el Id de la marca según la cadena 'sMarca', si es un número lo toma directamente y valida que existe, si no busca 
    /// en la base de datos una marca con ese nombre y toma el Id, si no encuentra un id valido retorna -1 </summary>
    private int GetIdMarca( string sMarca )
      {
      if( string.IsNullOrWhiteSpace( sMarca ) ) return 0;                           // No lo tiene en cuenta

      if( int.TryParse( sMarca, out int Id ) )                                      // Si es un número
        {
        if( Id == -1 ) return 0;                                                    // Explicitamente se idica que no use el campo
        return DbCtx.Marcas.Find( Id ) == null ? -1 : Id;                           // Verifica que existe en la BD y lo retorna
        }

      sMarca = sMarca.ToLower();
      if( sMarca =="todo"    ) return 0;                                            // Explicitamente no se usa la marca
      if( sMarca == "en uso" ) { enUso=true; return 0; }                            // Idica que se deben busca los coches donde se usa

      return DbCtx.Marcas.FirstOrDefault( x => x.Nombre == sMarca )?.Id ?? -1;      // Busca el Id por el nombre del modelo
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el Id del modelo según la cadena 'sModelo', si es un número lo toma directamente y valida que existe, si no busca 
    /// en la base de datos un modelo con ese nombre y toma el Id, si no encuentra un id valido retorna -1 </summary>
    private int GetIdModelo( string sModelo )
      {
      if( string.IsNullOrWhiteSpace( sModelo ) ) return 0;                          // No lo tiene en cuenta

      if( int.TryParse( sModelo, out int Id ) )                                     // Si es un número
        { 
        if( Id==-1 ) return 0;                                                      // Explicitamente se idica que no use el campo
        return DbCtx.Modelos.Find(Id)==null? -1 : Id;                               // Verifica que existe en la BD y lo retorna
        }

      sModelo = sModelo.ToLower();
      if( sModelo == "todo" ) return 0;                                             // Explicitamente no se usa el modelo
      return DbCtx.Modelos.FirstOrDefault( x => x.Nombre == sModelo )?.Id ?? -1;    // Busca el Id por el nombre del modelo
      }

    }//=======================================================================================================================================

  }