using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace Piezas2.Models
  {
  //=======================================================================================================================================
  // Datos que se utilizan en la vista de error en el sistema
  public class ErrorViewModel
    {
    public string LocalIp { get; private set; }
    public string RemoteIp { get; private set; }
    public int ErrorCode { get; private set; }
    public string ErrorMsg { get; private set; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construlle el objeto con el codigo del error y el contexto de la conección </summary>
    public ErrorViewModel( string errorCode, HttpContext httpContext )
      {
      LocalIp  = httpContext.Connection.LocalIpAddress.ToString();
      RemoteIp = httpContext.Connection.LocalIpAddress.ToString();

      if( !int.TryParse( errorCode, out int code ) ) code=-1;

      ErrorCode = code;
      ErrorMsg  = getHttpCodeDesc(); 
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene descripción de los códigos de errores HTTP mas utilizados </summary>
    private string getHttpCodeDesc()
      {
      switch( ErrorCode )                                        // La conexión no termino
        {
        case 400: return "La solicitud realizada es errorea";
        case 401: return "El recurda solicitado requiere autorizacion";
        case 402: return "Acceso denegado";
        case 404: return "La Url solicitada no fue encontrada";
        case 405: return "El método empleado en la petición no esta permitido";
        case 406: return "El servidor no puede dar una de las respuestas aceptadas";
        case 408: return "Se agotó el tiempo para responder a la solicitud";
        case 410: return "El recurso ya no esta disponible en el servidor";
        case 500: return "Ocurrio un error interno en el servidor";
        case 501: return "Al menos hay una funcionalidad que no esta implementada en el servidor";
        case 505: return "Versión de HTTP no soportada";
        case 507: return "No hay espacio sificiente en el servidor";
        case 509: return "Se ha excesido en ancho de banda disponible";
        case 511: return "Se requiere la atenticación del navegador";
        default:
          if( ErrorCode >= 500 ) return "El servidor fallo al completar la solicitud";
          else if( ErrorCode >= 400 ) return "La solicitud tiene una sintaxis incorrecta o no puede procesarse";
          else if( ErrorCode >= 300 ) return "El cliente debe tomar una acción adicional para completar la solicitud";
          else if( ErrorCode >= 200 ) return "La petición fue atendida correctamete";
          else if( ErrorCode >= 100 ) return "La patición fue recibida y continua en proceso";
          break;
        }

      return "";
      }

    }
  }
