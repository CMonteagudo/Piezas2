using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piezas2.Core.Model;
using Piezas2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piezas2.Core
  {
  enum DbTable { Marca, Modelo, Motor, Categoria, Fabricante }

  //=======================================================================================================================================
  /// <summary> Agrupa los datos de un nomenclador que esta compuesto por un identificador y un nombre o descripción </summary>
  public class IdName
    {
    public IdName( int Id, string Name )
      {
      this.Id   = Id;
      this.Name = Name;
      }

    public int Id { get; set; }
    public string Name { get; set; }
    }

  //=======================================================================================================================================
  /// <summary> Maneja los datos que se van a utilizar para buscar recambios </summary>
  public class ItemsFilters
    {
    /// <summary> Marca del coche para la que se quieren buscar recambios </summary>
    public IdName Marca      { get; set; } = new IdName( 0, "" );         // El Id igual a 0 implica no tener en cuenta la marca

    /// <summary> Model del coche para el que se quieren buscar recambios </summary>
    public IdName Modelo     { get; set; } = new IdName( 0, "" );         // El Id igual a 0 implica no tener en cuenta el modelo

    /// <summary> Motor del coche para buscar recambo </summary>
    public IdName Motor      { get; set; } = new IdName( 0, "" );         // El Id igual a 0 implica no tener en cuenta el motor

    /// <summary> Categoria a la que pertenece el recambio que se esta buscando </summary>
    public IdName Categoria  { get; set; } = new IdName( 0, "" );         // El Id igual a 0 implica no tener en cuenta la categoria

    /// <summary> Fabricante del recambio que se esta buscando </summary>
    public IdName Fabricante { get; set; } = new IdName( 0, "" );         // El Id igual a 0 implica no tener en cuenta el fabricante

    /// <summary> El número del primer registor a mostrar </summary>
    public int RegFirst { get; set; } = 0;                                // Por defecto se muestra desde el primer registro

    /// <summary> Cantidad maxima de registros a mostrar </summary>
    public int RegCount { get; set; } = 30;                               // Por defecto son 30 registros como maximo

    /// <summary> Indice del campo por donde hay que ordenar, 0 => No ordenar, Negativo => Orden desendiente </summary>
    public int Orden { get; set; }

    /// <summary> Campos de la tabla que se pueden usar para el ordenamiento </summary>
    public static string[] OrdenFields = { "codigo", "categoria", "fabricante", "nombre", "precio" };

    /// <summary> Da acceso a los datos de la solicitud </summary>
    private HttpContext HttpCtx;

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto, e interpreta los datos suministrados en la cadena sFiltros. los datos deben estar separados por '/', 
    /// el tipo de dato es establecido según el orden en la cadena (Ver GetFilterByPos), o por un prefijo que indica el tipo de dato (ver 
    /// GetFilterByPos), los valores pueden ser Identicadores o los valores de los campos directamente</summary>
    public ItemsFilters( string sFiltros, HttpContext httpCtx )
      {
      HttpCtx = httpCtx;

      if( string.IsNullOrWhiteSpace( sFiltros ) ) return;           // Si no hay datos de entrada deja todos los valores por defecto

      var segments = sFiltros.ToLower().Split('/');                 // Obtiene los segmentos de la cadena de entrada, separados por /

      for( int i = 0; i < segments.Length; ++i )                    // Analiza todos los segmentos
        {
        var segm = segments[i];

        if( GetFilterByPrefix( segm ) ) continue;                   // Trata de interpretar el segmento según un prefijo

        GetFilterByPos( i, segm );                                  // Interpreta el segmento según la posición que ocupa
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Si el segmento contiene uno de los siguientes prefijo, obtiene su valor y retorna verdadero si es un segmento valido
    ///  Mar- Marca del coche
    ///  Mod- Modelo del coche 
    ///  Mot- Motor que usa el coche
    ///  Cat- Categoria categoria del recambio
    ///  Fab- Fabricante del recambio
    ///  Ran- Rango de registros a mostrar en el formato: Primer registro - Numero de registros (Si no aparece la segunda parte se toma 30 por defecto)
    ///  Ord- Orden que van a terner los registros según el campo especificado, si persede con + ascendente y con - desendente
    ///  </summary>
    private bool GetFilterByPrefix( string segm )
      {
      var pre_val = segm.Split('-', 2);                                               // Divide el segmento en Prefijo-Valor
      if( pre_val.Length != 2 ) return false;                                         // Si no tiene los 2 elementos, ignora el segmento

      var prefix = pre_val[0].Trim();                                                 // Toma el prefijo y le quita los espacios
      var value  = pre_val[1].Trim();                                                 // Toma el valor y le quita los espacios

      if( prefix.Length>3 ) prefix = prefix.Substring( 0, 3 );                        // Solo tiene en cuanta las 3 primeras letra del prefijo

      switch( prefix )
        {
        case "mar": (Marca     ) = GetId( value, DbTable.Marca      ); break;         // Obtiene el Id y le Nombre de la tabla Marca
        case "mod": (Modelo    ) = GetId( value, DbTable.Modelo     ); break;         // Obtiene el Id y le Nombre de la tabla Modelo
        case "mot": (Motor     ) = GetId( value, DbTable.Motor      ); break;         // Obtiene el Id y le Nombre de la tabla Motor
        case "cat": (Categoria ) = GetId( value, DbTable.Categoria  ); break;         // Obtiene el Id y le Nombre de la tabla Categoria
        case "fab": (Fabricante) = GetId( value, DbTable.Fabricante ); break;         // Obtiene el Id y le Nombre de la tabla Fabricante
        case "ran": GetRango( value ); break;                                         // Obtiene los datos del rango de registro a entregar
        case "ord": GetOrden( value ); break;                                         // Obtiene los datos del tipo de ordenamiento a emplear

        default: return false;
        }

      return true;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el tipo de dato, de acuerdo a la posición encontarda, segun el siguiente orden:
    ///  Marca/Modelo/Motor/Categoria/Fabricante/Rango/Orden
    ///  </summary>
    private bool GetFilterByPos( int pos, string segm )
      {
      var value = segm.Trim();

      switch( pos )
        {
        case 0: (Marca     ) = GetId( value, DbTable.Marca      ); break;
        case 1: (Modelo    ) = GetId( value, DbTable.Modelo     ); break;
        case 2: (Motor     ) = GetId( value, DbTable.Motor      ); break;
        case 3: (Categoria ) = GetId( value, DbTable.Categoria  ); break;
        case 4: (Fabricante) = GetId( value, DbTable.Fabricante ); break;
        case 5: GetRango( value ); break;
        case 6: GetOrden( value ); break;

        default: return false;
        }

      return true;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Busca un Id a partir de 'sValue', si 'sValue' es un entero lo toma directamente,  sino lo busca en la 
    /// tabla 'table' de la base de datos, retorna el valor obtenido y si es correcto o no </summary>
    private IdName GetId( string sValue, DbTable table )
      {
      var Ret = new IdName( 0, "" );                                    // Valor de retorno por defecto que implica no tener en cuenta la informacion

      if( int.TryParse( sValue, out int Id  ) )                         // Trata leer el valor como un número
        {
        if( Id !=0 )                                                    // Si es 0, Explicitamente se desea no tener en cuenta la información
          Ret = FindName( table, Id );                                  // Valida el Id y busca el nombre del filtro
        }
      else
        { 
        if( sValue.Length!=0 && sValue != "todo" )                      // Explicitamente se desea no tener en cuenta la información
          Ret = FindId( table, sValue );                                // Valida el nombre y Busca el Id del filtro
        }

      return Ret;                                                       // Retorna datos obtenidos
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el Nombre del regitro con identificador 'Id' en la table 'table', si lo encuentra retorna el 
    /// identificador y el nombre, sino retorna -1 y "" </summary>
    private IdName FindName( DbTable table, int id )
      {
      var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));

      var name = table switch
        {
          DbTable.Marca      => DbCtx.Find<Marca>     ( id )?.Nombre ?? "",
          DbTable.Modelo     => DbCtx.Find<Modelo>    ( id )?.Nombre ?? "",
          DbTable.Motor      => DbCtx.Find<Motor>     ( id )?.Nombre ?? "",
          DbTable.Categoria  => DbCtx.Find<Categorium>( id )?.Nombre ?? "",
          DbTable.Fabricante => DbCtx.Find<Fabricante>( id )?.Nombre ?? "",
          _ => "",
          };

      if( name.Length==0 ) id = -1;
      return new IdName(id, name);
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Busca el Id del record con nombre 'Name' en la tabla 'table', si no lo encuentra retorna -1 </summary>
    private IdName FindId( DbTable table, string name )
      {
      var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));

      var id = table switch
        {
          DbTable.Marca      => DbCtx.Marcas     .FirstOrDefault( x => x.Nombre == name )?.Id ?? -1,
          DbTable.Modelo     => DbCtx.Modelos    .FirstOrDefault( x => x.Nombre == name )?.Id ?? -1,
          DbTable.Motor      => DbCtx.Motors     .FirstOrDefault( x => x.Nombre == name )?.Id ?? -1,
          DbTable.Categoria  => DbCtx.Categorias  .FirstOrDefault( x => x.Nombre == name )?.Id ?? -1,
          DbTable.Fabricante => DbCtx.Fabricantes.FirstOrDefault( x => x.Nombre == name )?.Id ?? -1,
          _ => -1,
          };

      if( id==-1 ) name = "";
      return new IdName( id, name );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el rango de registros a mostrar, el formato es 'primer régistro'-'número de registros' </summary>
    private bool GetRango( string sValue )
      {
      var ini_num = sValue.Split('-', 2);                           // Divide el valor en 2 partes

      if( !int.TryParse( ini_num[0].Trim(), out int ini ) )         // Trata de convertir la primera pate en un número
        return false;                                               // Si no puede retorn error

      RegFirst = ini;

      var sNum = "";
      if( ini_num.Length > 1 ) sNum = ini_num[1].Trim();

      if( sNum.Length>0 && int.TryParse( sNum, out int num ) )         // Trata de convertir la primera pate en un número
        RegCount = num;
      else
        RegCount = 30;

      return true;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos de como se va a ordenar el resultado, el formato es 'Nombre de la columna[-]' el signo al final indica 
    /// que el ordenamiento es desendiente, los campos permitidos se indican en 'OrdenFields' </summary>
    private bool GetOrden( string sValue )
      {
      var nFlds = OrdenFields.Length;                               // Obtiene la cantidad de campos que se pueden ordenar
      if( int.TryParse( sValue, out int idx ) )                     // Trata de ver si el valor es un número
        {
        var idxAbs = Math.Abs( idx );                               // Toma el valor absoluto
        if( idxAbs>=0 && idxAbs<nFlds )                             // Si el indice esta dentro del rango de campos disponible
          {
          Orden = idx;                                              // Si el orden es desendiente cambia el signo
          return true;                                              // Retorna OK
          }
        }
      else
        {
        bool ordDesc = false;
        if( sValue.Last() == '-' )                                    // Si el valor termina con -
          {
          sValue = sValue.TrimEnd( ' ', '-' );                        // Quita marca y espacios del final
          ordDesc = true;                                             // Pone bandera de orden desendiente
          }

        for( int i=0; i<nFlds; ++i )                                // Busca por todos los campos disponible
          if( OrdenFields[i] == sValue )                            // Si el valor coincide con el campo
            {
            ++i;                                                    // Toma el indice a partir de 1
            Orden = ordDesc ? -i : i;                               // Le cambia el signo si es desendiente
            return true;                                            // Retorna
            }
        }

      Orden = 0;
      return false;                                                 // No se pudo interpretar correctamente las info del orden, no ordena
      }


    }  // Fin de la clase 
  }  // Fin del namespace  
