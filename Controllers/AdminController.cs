using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Piezas2.Core;
using Piezas2.Core.Model;
using Piezas2.Models;

namespace Piezas2.Controllers
  {
  //=======================================================================================================================================
  // Controlador que sirve como punto de entrada a todas las páginas de administración del sitio
  public class AdminController : Controller
    {
    public AdminController()
      {
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Determina si el usuario logueado es un administrador o no </summary>
    public bool Admin { get{ return ( HttpContext.Session.GetInt32( "Admin" ) == 1 ); } }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página principal para la parte de administración </summary>
    [Route( "/admin/opciones" )]
    [Route( "/admin/index" )]
    [HttpGet( "/admin/" )]
    public IActionResult Index()
      {
      if( !Admin ) return new EmptyResult();

      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página principal para la parte de administración </summary>
    [Route( "/admin/api-doc" )]
    public IActionResult ApiDoc()
      {
      if( !Admin ) return new EmptyResult();

      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para vincular/desvincualar los recambios a los coches que lo usan </summary>
    [Route( "/admin/recambio-coche/{id:int}/{name?}" )]
    [Route( "/admin/recambio-coche/{id:int?}" )]
    public IActionResult RecambioUso( int id )
      {
      if( !Admin ) return new EmptyResult();

      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.All );
      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los recambios. </summary>
    [HttpGet( "/admin/recambio-edicion/{id:int?}" ) ]
    public IActionResult RecambioEdit( int id )
      {
      if( !Admin ) return new EmptyResult();

      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.Items );
      return View( model ); 
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los recambios. </summary>
    [HttpGet( "/admin/coche-edicion/{id:int?}" )]
    public IActionResult CocheEdit( int id )
      {
      if( !Admin ) return new EmptyResult();

      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.Coches );
      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de marcas de coches. </summary>
    [HttpGet( "/admin/marca-edicion/{id:int?}" )]
    public IActionResult MarcaEdit( int id )
      {
      if( !Admin ) return new EmptyResult();

      ViewData["Id"]= id;
      ViewData["Widget"] = HttpContext.Request.Query.ContainsKey( "Widget" ) ? 1 : 0;         //  Si la página se llamo en modo Widget

      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los fabricantes de recambios. </summary>
    [HttpGet( "/admin/fabricante-edicion/{id:int?}" )]
    public IActionResult FabricanteEdit( int id )
      {
      if( !Admin ) return new EmptyResult();

      ViewData["Id"] = id;
      ViewData["Widget"] = HttpContext.Request.Query.ContainsKey( "Widget" ) ? 1 : 0;         //  Si la página se llamo en modo Widget

      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los modelos de coches. </summary>
    [HttpGet( "/admin/modelo-edicion/{id:int?}" )]
    public IActionResult ModeloEdit( int id )
      {
      if( !Admin ) return new EmptyResult();

      ViewData["Id"] = id;
      ViewData["Widget"] = HttpContext.Request.Query.ContainsKey( "Widget" ) ? 1 : 0;         //  Si la página se llamo en modo Widget

      ViewData["Marcas"] = new Marcas(HttpContext).ListIdName();  
      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los motores de coches. </summary>
    [HttpGet( "/admin/motor-edicion/{id:int?}" )]
    public IActionResult MotorEdit( int id )
      {
      if( !Admin ) return new EmptyResult();

      ViewData["Id"] = id;
      ViewData["Widget"] = HttpContext.Request.Query.ContainsKey( "Widget" ) ? 1 : 0;         // Si la página se llamo en modo Widget

      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de categorias de recambios. </summary>
    [HttpGet( "/admin/categoria-edicion/{id:int?}" )]
    public IActionResult CategoriaEdit( int id )
      {
      if( !Admin ) return new EmptyResult();

      ViewData["Id"] = id;
      ViewData["Widget"] = HttpContext.Request.Query.ContainsKey( "Widget" ) ? 1 : 0;         // Si la página se llamo en modo Widget

      return View();
      }

    }
  }
