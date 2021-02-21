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
    /// <summary> Lista de motores encontrados según los datos suministrados </summary>
    public List<MotorCoche> Items { get; set; } = new List<MotorCoche>();

    /// <summary> Contexto para acceso a la base de datos </summary>
    DbPiezasContext DbCtx;

    /// <summary> Indica que se busque solo los motores que se estan usando en algun coche </summary>
    bool enUso = false;           // Solo es necesario si no se usa 'marca' o 'modelo'

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los modelos de la base de datos </summary>
    public Motores( string marca, string modelo, HttpContext HttpCtx )
      {
      DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));      // Obtiene contexto a la BD

      int IdMarca  = GetIdMarca ( marca  );
      int IdModelo = GetIdModelo( modelo );

      if( IdMarca==-1 || IdModelo==-1 ) return;                                     // La marca o el modelo no se encuentran en la BD

      var hasCoche = IdMarca>0 || IdModelo>0;                                       // Si hay que tener en cuenta el coche

      var sTable  = hasCoche || enUso? "Motor AS m INNER JOIN Coche AS c ON m.Id=c.Motor" : "Motor as m";
      var sFields = hasCoche || enUso? "m.Id, m.Nombre, c.Marca, c.Modelo"                : "m.Id, m.Nombre, Marca=-1, Modelo=-1";

      StringBuilder sWhere = new StringBuilder("");
      if( hasCoche )                                                                // Si hay algun tipo de filtro
        {
        sWhere.Append( "WHERE " );

        if( IdMarca  > 0 ) sWhere.Append( $"c.Marca = {IdMarca}" );                 // Filtra por la marca de coche
        if( IdModelo > 0 )                                                          // Filtra por el modelo del coche
          {
          if( IdMarca > 0 ) sWhere.Append( " AND " );
          sWhere.Append( $"c.Modelo = {IdModelo}" ); 
          }                 
        }

      var sql = $"SELECT DISTINCT {sFields} FROM {sTable} {sWhere}";

      Items = DbCtx.MotorCoches.FromSqlRaw( sql ).Select( x => new MotorCoche(x) ).ToList();                                  // Ejecuta la secuencia SQL
      Items.Sort( ( s1, s2 ) => s1.Nombre.CompareTo(s2.Nombre) );
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