using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Piezas2.Models;

namespace Piezas2.Controllers
  {
  //=======================================================================================================================================
  // Controlador que sirve como punto de entrada a todas las páginas del sitio
  [ApiExplorerSettings(IgnoreApi = true)]
  public class SiteController : Controller
    {
    //private readonly ILogger<SiteController> _logger;

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Crea el objeto antes de ejecutar cualquier acción </summary>
    public SiteController( /*ILogger<SiteController> logger*/ )
      {
      //_logger = logger;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página principal del sitio </summary>
    [HttpGet( "/recambios" )]
    [HttpGet( "/" )]
    public IActionResult Index()
      {
      var model = new FindRecambioModel( HttpContext );
      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para mostrar los datos de un recambio, Nota: el nombre se ignora </summary>
    [HttpGet( "/recambio/{id:int}/{name?}" )]
    public IActionResult Recambio( int id )
      {
      var model = new RecambioModel( id, HttpContext );
      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para mostrar los datos de un recambio </summary>
    [HttpGet( "/buscar-recambio/{tipo?}" )]
    public IActionResult Busqueda( string tipo="" )
      {
      ViewData["FindType"] = tipo;

      var model = new FindRecambioModel( HttpContext );

      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para mostrar los datos de un recambio </summary>
    [HttpGet( "/modelos-de-coches/{marca?}" )]
    public IActionResult Modelos( string marca = "" )
      {
      var model = new ModelosModel( marca, HttpContext );

      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para mostrar la lista de recambios para un fabricante dado </summary>
    [HttpGet( "/fabricantes-de-recambios/{maker?}" )]
    public IActionResult RecambiosFabricantes( string maker = "" )
      {
      var model = new RecambiosFabricantesModel( maker, HttpContext );

      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para mostrar los datos de un recambio </summary>
    [HttpGet( "/recambios-del-coche/{cocheId}/{nombre?}" )]
    public IActionResult RecambiosCoche( int cocheId )
      {
      var model = new RecambiosCocheModel( cocheId, HttpContext );

      return View( model );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página página para manejar las compras pendientes (carrito) </summary>
    [HttpGet( "/pagos-pendientes" )]
    public IActionResult VentasPendientes()
      {
      return View( new VentasModel( HttpContext ) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para mostrar el mapa del sitio </summary>
    [HttpGet( "/mapa-del-sitio" )]
    public IActionResult MapaDelSitio( )
      {
      return View( new BaseModel(HttpContext) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para mostrar la información de contacto </summary>
    [HttpGet( "/informacion-de-contacto" )]
    public IActionResult ContactInfo( )
      {
      return View( new BaseModel( HttpContext ) );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página para saber a cerca del sitio </summary>
    [HttpGet( "/acerca-de" )]
    public IActionResult About( )
      {
      return View( new BaseModel( HttpContext ) );
      }
                                                    
    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Página que muestra los errores que se produzcan en el sitio </summary>
    [HttpGet( "/error/{code?}" )]
    [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
    public IActionResult Error( string code )
      {
      var model = new ErrorViewModel( code, HttpContext );

      return View( model );
      }
    }

  }
