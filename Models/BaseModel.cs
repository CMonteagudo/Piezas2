using Microsoft.AspNetCore.Http;
using Piezas2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Html;

namespace Piezas2.Models
  {
  //=======================================================================================================================================
  /// <summary> Modelo base para todas las vistas que usan el layout principal </summary>
  public class BaseModel
    {
    public string _User   { set; get; } = "{\"Id\":0, \"nBuy\":0}";       // Una cadena Json con los datos del usuario activo
    public int    _UserId { set; get; } = 0;                              // Identificador del usuario activo
    public int    _Widget { set; get; } = 0;                              // Indica si la página se va a mostrar en modo Widget
    public int     _Admin { set; get; } = 0;                              // Número de compras que hay para el usuario actual
    public int     Id     { set; get; } = 0;                              // Identificador del objeto que se muestra en la vista

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public BaseModel( HttpContext HttpCtx, int id = 0 )
      {
      Id = id;

      var sVal = HttpCtx.Session?.GetString( "User");

      if( !string.IsNullOrWhiteSpace( sVal ) ) 
        {
        _User   = sVal;
        _UserId = (int) HttpCtx.Session.GetInt32( "UserId" );
        _Admin  = (int) HttpCtx.Session.GetInt32( "Admin" );
        }

      _Widget = HttpCtx.Request.Query.ContainsKey( "Widget" )? 1: 0;    //  Si la página se llamo en modo Widget
      }
    }
  }
  