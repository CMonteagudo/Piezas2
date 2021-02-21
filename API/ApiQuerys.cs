using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piezas2.Core;
using Piezas2.Core.Model;

// Comando para obtener las clases que mapean a la base de datos
// dotnet ef dbcontext scaffold "Server=(LocalDB)\MSSQLLocalDB;Database=D:\01_Proyectos\_Web\Piezas2\Core\DataBase\Piezas.mdf;Trusted_Connection=True" Microsoft.EntityFrameWorkCore.SqlServer -o Core\Models

namespace Piezas2
  {
  //=========================================================================================================================================
  /// <summary> Atiende todas las consultas a la base de datos realizadas por el API</summary>
  [ApiController]
  public class ApiQuerys : ControllerBase
    {
    private readonly DbPiezasContext _context;

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Contruye el objero y guarda el contexto a la base de datos para su uso posterior</summary>
    public ApiQuerys( DbPiezasContext context )
      {
      _context = context;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos pasado a la URL y los utiliza como filtros para obtener los recambios que se ajusten a esos datos</summary>
    [HttpGet( "/Api/Recambios/{*Datos}" )]
    public ActionResult<Recambios> GetCoche( string datos )
      {
      return new Recambios( datos, HttpContext );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Retorna una lista de todas las marcas y su codigo, que hay en la base de datos </summary>
    [HttpGet( "/Api/MarcaCoches" )]
    public ActionResult<MarcaCoches> GetMarcaCoches( string datos )
      {
      return new MarcaCoches( HttpContext );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los modelos para una marca especificada, si no se especifica la marca se retornan todos los modelos </summary>
    [HttpGet( "/Api/ModelosMarca/{marca?}" )]
    public ActionResult<ModelosMarca> GetModelosMarca( string marca )
      {
      return new ModelosMarca( marca, HttpContext );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los motores en función de la marca o el modelo </summary>
    [HttpGet( "/Api/Motores/{marca?}" )]
    [HttpGet( "/Api/Motores/{marca}/{modelo}" )]
    public ActionResult<Motores> GetMotor( string marca = null, string modelo = null )
      {
      return new Motores( marca, modelo, HttpContext );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todas las categorias usadas en el sistema </summary>
    [HttpGet( "/Api/Categorias" )]
    public ActionResult<Categorias> GetCategorias()
      {
      return new Categorias( HttpContext );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todos los fabricantes usados en el sistema </summary>
    [HttpGet( "/Api/Fabricantes" )]
    public ActionResult<Fabricantes> GetFabricantes()
      {
      return new Fabricantes( HttpContext );
      }

    }//=========================================================================================================================================
  }
