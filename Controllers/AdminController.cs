using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Piezas2;
using Piezas2.Core;
using Piezas2.Core.Model;
using Piezas2.Models;

namespace Piezas2.Controllers
  {
  //=======================================================================================================================================
  // Controlador que sirve como punto de entrada a todas las páginas de administración del sitio
  [ApiExplorerSettings( IgnoreApi = true )]
  public class AdminController : Controller
    {
    public AdminController()
      {
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página principal para la parte de administración </summary>
    [HttpGet( "/admin/opciones" )]
    [HttpGet( "/admin/index" )]
    [HttpGet( "/admin/" ), AdminOnly]
    public IActionResult Index()
      {
      return View( new BaseModel( HttpContext ) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página principal para la parte de administración </summary>
    [HttpGet( "/admin/api-doc" ), AdminOnly]
    public IActionResult ApiDoc()
      {
      return View( new BaseModel( HttpContext ) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para vincular/desvincualar los recambios a los coches que lo usan </summary>
    [HttpGet( "/admin/recambio-coche/{id:int}/{name?}" )]
    [HttpGet( "/admin/recambio-coche/{id:int?}" ), AdminOnly]
    public IActionResult RecambioUso( int id )
      {
      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.All );
      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los recambios. </summary>
    [HttpGet( "/admin/recambio-edicion/{id:int?}" ), AdminOnly]
    public IActionResult RecambioEdit( int id )
      {
      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.Items );
      return View( model ); 
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los recambios. </summary>
    [HttpGet( "/admin/coche-edicion/{id:int?}" ), AdminOnly]
    public IActionResult CocheEdit( int id )
      {
      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.Coches );
      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de marcas de coches. </summary>
    [HttpGet( "/admin/marca-edicion/{id:int?}" ), AdminOnly]
    public IActionResult MarcaEdit( int id )
      {
      return View( new BaseModel( HttpContext, id ) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los fabricantes de recambios. </summary>
    [HttpGet( "/admin/fabricante-edicion/{id:int?}" ), AdminOnly]
    public IActionResult FabricanteEdit( int id )
      {
      return View( new BaseModel( HttpContext, id ) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los modelos de coches. </summary>
    [HttpGet( "/admin/modelo-edicion/{id:int?}" ), AdminOnly]
    public IActionResult ModeloEdit( int id )
      {
      ViewData["Marcas"] = new Marcas(HttpContext).ListIdName();  
      return View( new BaseModel( HttpContext, id ) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los motores de coches. </summary>
    [HttpGet( "/admin/motor-edicion/{id:int?}" ), AdminOnly]
    public IActionResult MotorEdit( int id )
      {
      return View( new BaseModel( HttpContext, id ) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de categorias de recambios. </summary>
    [HttpGet( "/admin/categoria-edicion/{id:int?}" ), AdminOnly]
    public IActionResult CategoriaEdit( int id )
      {
      return View( new BaseModel( HttpContext, id ) );
      }

    }
  }
