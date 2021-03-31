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
    ///<summary> Página principal para la parte de administración </summary>
    [Route( "/admin/opciones" )]
    [Route( "/admin/index" )]
    [HttpGet( "/admin/" )]
    public IActionResult Index()
      {
      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página principal para la parte de administración </summary>
    [Route( "/admin/api-doc" )]
    public IActionResult ApiDoc()
      {
      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para vincular/desvincualar los recambios a los coches que lo usan </summary>
    [Route( "/admin/recambio-coche/{id:int}/{name?}" )]
    [Route( "/admin/recambio-coche/{id:int?}" )]
    public IActionResult RecambioUso( int id )
      {
      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.All );
      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los recambios. </summary>
    [HttpGet( "/admin/recambio-edicion/{id:int?}" ) ]
    public IActionResult RecambioEdit( int id )
      {
      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.Items );
      return View( model ); 
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los recambios. </summary>
    [HttpGet( "/admin/coche-edicion/{id:int?}" )]
    public IActionResult CocheEdit( int id )
      {
      var model = new RecambiosDatosModel( id, HttpContext, DataInfo.Coches );
      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de marcas de coches. </summary>
    [HttpGet( "/admin/marca-edicion/{id:int?}" )]
    public IActionResult MarcaEdit( int id )
      {
      ViewData["Id"]= id;
      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los fabricantes de recambios. </summary>
    [HttpGet( "/admin/fabricante-edicion/{id:int?}" )]
    public IActionResult FabricanteEdit( int id )
      {
      ViewData["Id"] = id;
      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los modelos de coches. </summary>
    [HttpGet( "/admin/modelo-edicion/{id:int?}" )]
    public IActionResult ModeloEdit( int id )
      {
      ViewData["Id"] = id;
      ViewData["Marcas"] = new Marcas(HttpContext).ListIdName();
      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de los motores de coches. </summary>
    [HttpGet( "/admin/motor-edicion/{id:int?}" )]
    public IActionResult MotorEdit( int id )
      {
      ViewData["Id"] = id;
      return View();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Interface para la modificación, creación y eliminacion de categorias de recambios. </summary>
    [HttpGet( "/admin/categoria-edicion/{id:int?}" )]
    public IActionResult CategoriaEdit( int id )
      {
      ViewData["Id"] = id;
      return View();
      }

    }
  }
