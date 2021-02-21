using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Piezas2.Models;

namespace Piezas2.Controllers
  {
  public class HomeController : Controller
    {
    private readonly ILogger<HomeController> _logger;

    public HomeController( ILogger<HomeController> logger )
      {
      _logger = logger;
      }

    public IActionResult Index()
      {
      var model = new FindRecambioModel( HttpContext );
      return View( model );
      }

    [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
    public IActionResult Error( string id )
      {
      var model = new ErrorViewModel( id, HttpContext );

      return View( model );
      }
    }
  }
