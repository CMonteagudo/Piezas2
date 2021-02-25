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
  /// <summary> Esta parte del API contiene todas la funciones para modificar la base de datos, se supone que solo sea usada por la interfaces
  /// que se hagan para la administracion de los datos, debe haber un nivel de seguridad alto y va a ser utilizada por un reducido número de
  /// personas </summary>
  [ApiController]
  public class DbAtions : ControllerBase
    {
    private readonly DbPiezasContext _context;

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Contruye el objeto y guarda el contexto a la base de datos para su uso posterior</summary>
    public DbAtions( DbPiezasContext context )
      {
      _context = context;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Quita el uso del item 'ItemId' por los coches dados en la lista 'CochesList' </summary>
    [HttpGet( "/Api/DelRecambioUsos/{ItemId:int}/{CochesList}" )]
    public ActionResult<string> DelRecambioUsos( int ItemId, string CochesList )
      {
      var uso    = new RecambioUsos( ItemId, HttpContext );
      var numDel = uso.DeleteCoches( CochesList );

      return $"{{\"Count\":{numDel}}}";
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona el uso del item 'ItemId' por los coches dados en la lista 'CochesList' </summary>
    [HttpGet( "/Api/AddRecambioUsos/{ItemId:int}/{CochesList}" )]
    public ActionResult<string> AddRecambioUsos( int ItemId, string CochesList )
      {
      var uso = new RecambioUsos( ItemId, HttpContext );
      var numAdd = uso.AddCoches( CochesList );

      return $"{{\"Count\":{numAdd}}}";
      }

    }//=========================================================================================================================================
  }
