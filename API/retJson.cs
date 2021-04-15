using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Piezas2
  {
  //=======================================================================================================================================
  /// <summary> Maneja las cadenas retorno en formato JSon para todo el API, fundamentalmente para retorno de errores </summary>
  public static class retJson
    {
    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON que indica que todo estuvo bien, y se incluye un valor de retorno  </summary>
    public static string Ok( string name, int retVal )
      {
      return $"{{\"Error\":0, \"{name}\":{retVal} }}";
      }

    public static JsonResult Ok()
      {
      return new JsonResult( new { Error = 0 } );
      }

    public static JsonResult OkId( int Id )
      {
      return new JsonResult( new { Error=0, Id } );
      }

    public static JsonResult OkId( int Id, int Confirm )
      {
      return new JsonResult( new { Error=0, Id, Confirm } );
      }

    public static JsonResult OkCount( int Count )
      {
      return new JsonResult( new { Error=0, Count } );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con el código y el mensaje de cuando un record no se puede modificar o adicionar  </summary>
    public static JsonResult NoModify( int id, string name, Exception e = null )
      {
      if( id == 0 ) return ErrorObj( 1008, $"Error al adicionar el regístro con nombre '{name}'", e );
      else          return ErrorObj( 1007, $"Error al modificar el regístro con Id={id} y nombre '{name}'", e );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con el código y el mensaje que no se puede enviar el email  </summary>
    public static JsonResult NoSendMail( Exception e = null )
      {
      var sErr = "Hubo un problema al enviar el correo.";
      return ErrorObj( 1006, sErr, e );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con el código y el mensaje de cuando un record no se puede borrar  </summary>
    public static JsonResult NoDelete( int id, string name, Exception e=null )
      {
      var sErr = $"El regístro con ID={id} y nombre '{name}' no se puede borrar";

      if( e?.GetType().Name == "DbUpdateException" ) sErr += $", porque se esta utlizando.";

      return ErrorObj( 1005, sErr );
      }

    ///<summary> Retorna el error de acceso denegado  </summary>
    public static JsonResult NoAccess()
      {
      var sErr = $"Acceso denegado para el usuario actual.";
      return ErrorObj( 1004, sErr );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con el código y el mensaje de cuando un record no se encuentra en la base de datos  </summary>
    public static JsonResult NoExist( int id, string name )
      {
      var sErr = $"No se encontro en la base de datos, el registro con Id={id} y nombre '{name}'.";
      return ErrorObj( 1003, sErr );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con cuando se esta pridiendo el listado de ventas  </summary>
    internal static JsonResult NoVentas( int userId, Exception e )
      {
      var sErr = $"Error al obtener el listado de ventas para el usuario '{userId}'";
      return ErrorObj( 1002, sErr, e );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna un error al tratar de obtener las ventas perndientes del usuario  </summary>
    internal static JsonResult NoPendVentas( int userId, Exception e )
      {
      var sErr = $"Error al obtener las compras pendientes para el usuario '{userId}'";
      return ErrorObj( 1001, sErr, e );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna un error al tratar de obtener las ventas perndientes del usuario  </summary>
    internal static JsonResult ErrorObj( int Error, string sError, Exception e=null )
      {
      if( e != null )
        { 
        var (Err, Msg) = ExceptionError( e );

        if( Error != 0 )
          { 
          Error   = Err;
          sError += "; " + Msg;
          }
        }

      return new JsonResult( new { Error, sError } );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Mira la excepción y trata de traducirla en un mensaje más entendible para el usuario  </summary>
    public static (int, string) ExceptionError( Exception e )
      {
      if( e == null ) return (0, "");

      switch( e.Message )
        {
        case "UserExist": return (2001, $"Ya exite un usuario con ese nombre.");
        case "MailExist": return (2002, $"Ya exite un usuario con ese correo.");
        case "UserNoExist": return (2003, $"El usuario indicado no existe.");
        case "AdminAccess": return (2004, $"Solo es accesible por administradore.");
        case "UserAccess": return (2005, $"Solo es accesible para el usuario logueado.");
        }

      return (0, "");
      }

    }
  }
