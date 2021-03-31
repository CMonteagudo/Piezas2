using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piezas2.Core;
using Piezas2.Core.Model;
using static Piezas2.Core.Coches;

// Comando para obtener las clases que mapean a la base de datos
// dotnet ef dbcontext scaffold "Server=(LocalDB)\MSSQLLocalDB;Database=D:\01_Proyectos\_Web\Piezas2\Core\DataBase\Piezas.mdf;Trusted_Connection=True" Microsoft.EntityFrameWorkCore.SqlServer -o Core\Models

namespace Piezas2
  {
  //=========================================================================================================================================
  /// <summary> Atiende todas las consultas a la base de datos realizadas por el API, esta parte del API en la encagada de suministar toda
  /// la información necesaria para el sitio web y cualquier otra interface que se quiera emplear, su nivel de seguridad es baja, porque no 
  /// se altera la base de datos</summary>
  [ApiController]
  public class DbQuerys : ControllerBase
    {
    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Contruye el objero y guarda el contexto a la base de datos para su uso posterior</summary>
    public DbQuerys()
      {
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos pasado a la URL y los utiliza como filtros para obtener los recambios que se ajusten a esos datos</summary>
    [HttpGet( "/api/recambios/{*datos}" )]
    public ActionResult<Recambios> FindRecambios( string datos )
      {
      return new Recambios( HttpContext ).FindByDatos( datos );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los recambios que tengan el código dado </summary>
    [HttpGet( "/api/recambios-con-codigo/{code:int}/{*datos}" )]
    public ActionResult<Recambios> FindRecambiosByCode( int code )
      {
      return new Recambios( HttpContext ).FindByCode( code );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los recambios que tengan el código dado </summary>
    [HttpGet( "/api/recambios-con-nombre/{name}/{*datos}" )]
    public ActionResult<Recambios> FindRecambiosByName( string name, string datos )
      {
      return new Recambios( HttpContext ).FindByName( name, datos );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los recambios que no sean usados por ningún coche </summary>
    [HttpGet( "/api/recambios-no-usados" )]
    public ActionResult< List<IdName> > FindRecambiosNoUsados( )
      {
      return new Recambios( HttpContext ).FindNoUsed();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los recambios son usados por al menos un coche </summary>
    [HttpGet( "/api/recambios-usados" )]
    public ActionResult<List<IdName>> FindRecambiosUsados()
      {
      return new Recambios( HttpContext ).FindUsed();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Retorna una lista con el código y el nombre de todas las marcas, </summary>
    [HttpGet( "/api/marcas-de-coches" )]
    public ActionResult<List<IdName>> GetMarcaCoches()
      {
      return new Marcas( HttpContext ).ListIdName();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Retorna una lista de todas las marcas, con todos sus datos </summary>
    [HttpGet( "/api/marcas" )]
    public ActionResult<List<Marca>> GetMarcas()
      {
      return new Marcas( HttpContext ).ListMarcas();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los datos de la marca solicitada </summary>
    [HttpGet( "/api/marca/{Id:int}" )]
    public ActionResult<Marca> GetMarca( int Id )
      {
      var marca = new Marcas( HttpContext ).Find( Id );
      if( marca==null ) marca = new Marca { Id=0 };

      return marca;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todos los fabricantes usados en el sistema </summary>
    [HttpGet( "/api/fabricantes" )]
    public ActionResult<List<Fabricante>> GetFabricantes()
      {
      return new Fabricantes( HttpContext ).ListFabricantes();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todos los motores de coches usados en el sistema </summary>
    [HttpGet( "/api/motores" )]
    public ActionResult<List<Motor>> GetMotores()
      {
      return new Motores( HttpContext ).All();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todos los fabricantes usados en el sistema </summary>
    [HttpGet( "/api/modelos" )]
    public ActionResult< List<Modelo> > GetModelos()
      {
      return new Modelos( HttpContext ).ListModelos();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los datos del fabricante solicitado </summary>
    [HttpGet( "/api/fabricante/{Id:int}" )]
    public ActionResult<Fabricante> GetFabricante( int Id )
      {
      var maker = new Fabricantes( HttpContext ).Find( Id );
      if( maker == null ) maker = new Fabricante { Id = 0 };

      return maker;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los modelos para una marca especificada, si no se especifica la marca se retornan todos los modelos </summary>
    [HttpGet( "/api/modelos-por-marca/{marca?}" )]
    public ActionResult< List<Modelo> > GetModelosMarca( string marca )
      {
      return new Modelos( HttpContext ).ListModelos( marca );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos del motor con identificador 'id' o null si no se encuentra </summary>
    [HttpGet( "/api/motor/{id:int?}" )]
    public ActionResult<Motor> FindMotor( int id )
      {
      var motor = new Motores( HttpContext ).Find( id );
      if( motor == null ) motor = new Motor { Id = 0 };

      return motor;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un listado de todos los motores que hay en la base de datos </summary>
    [HttpGet( "/api/motores/all" )]
    public ActionResult< List<Motor> > GetAllMotores()
      {
      return new Motores( HttpContext ).All();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los motores en función de la marca o el modelo </summary>
    [HttpGet( "/api/motores/{marca?}" )]
    [HttpGet( "/api/motores/{marca}/{modelo}" )]
    public ActionResult<List<MotorCoche>> GetMotor( string marca = null, string modelo = null )
      {
      return new Motores( HttpContext ).FindForCoche( marca, modelo ) ;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todas las categorias usadas en el sistema </summary>
    [HttpGet( "/api/categorias" )]
    public ActionResult<List<Categorium> > GetCategorias()
      {
      return new Categorias( HttpContext ).ListCategorias();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todas las sub-categorias usadas en el sistema </summary>
    [HttpGet( "/api/sub-categorias" )]
    public ActionResult<List<IdName>> GetSubCategorias()
      {
      var cat = new Categorias( HttpContext );
      return cat.getSubCategorias();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los datos del recambo solicitado, 'cocheInfo' indica si se obtiene la lista de coches que lo usan o no </summary>
    [HttpGet( "/api/recambio/{Id:int}/{coheInfo:int?}" )]
    public ActionResult<Recambio> GetRecambio( int Id, int coheInfo=1 )
      {
      return new Recambio( Id, HttpContext, coheInfo );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene los datos pasado a la URL y los utiliza como filtros para obtener los coches que se ajusten a esos datos</summary>
    [HttpGet( "/api/coches/{*datos}" )]
    public ActionResult<List<CocheDescFull>> FindCoches( string datos )
      {
      return new Coches( HttpContext ).FullDesc( datos );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene todos los datos del coche solicitado </summary>
    [HttpGet( "/api/coche/{Id:int}" )]
    public ActionResult<Coche> GetCoche( int Id )
      {
      var coche = new Coches( HttpContext ).Find( Id );
      if( coche == null ) coche = new Coche { Id = 0 };

      return coche;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene una lista con la descripcion de todos los coches en la base de datos </summary>
    [HttpGet( "/api/coches-desc" )]
    public ActionResult<List<CocheDesc>> GetCochesDesc()
      {
      return new Coches( HttpContext ).AllDesc();
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de coches que usan el recambio dado </summary>
    [HttpGet( "/api/recambio-uso/{Id:int}" )]
    public ActionResult<CochesInUso> GetRecambioUso( int Id )
      {
      return new RecambioUsos( Id, HttpContext ).UsoInCoches();
      }

    }//=========================================================================================================================================
  }
