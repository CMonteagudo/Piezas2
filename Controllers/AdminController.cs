﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Piezas2.Models;

namespace Piezas2.Controllers
  {
  //=======================================================================================================================================
  // Controlador que sirve como punto de entrada a todas las páginas de administración del sitio
  public class AdminController : Controller
    {

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
      var model = new RecambioUsoModel( id, HttpContext );
      return View( model );
      }

    }
  }
